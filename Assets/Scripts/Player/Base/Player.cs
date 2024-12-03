using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Enemy;
using static GameManager;

public class Player : MonoBehaviour
{
    public PlayerIdleState idleState;
    public PlayerWalkingState walkingState;
    public PlayerStateMachine stateMachine;

    public bool breakCoroutine = false;

    #region Animator
    [SerializeField] public Animator animator;

    public readonly string PLAYER_IDLE_ANIMATION_NAME = "Idle";
    public readonly string PLAYER_MOVE_ANIMATION_NAME = "RunForward";
    #endregion


    [SerializeField] public LayerMask TileLayerMask;
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject playerModel;
    [SerializeField] protected GameObject TileFrefabs;

    public GameManager gameManager;
    public Rigidbody rigitBody;

    public MazeGenerator mazeGenerator;
    public AStar aStar;
    public Tile currentTile;

    public Color originalColor;
    public Color lighterColor;

    public List<Tile> ColoringTile;

    [SerializeField] public UserStatus userStatus;
    [SerializeField] public IntEventChannel HpSetupEventChannel;
    [SerializeField] public IntEventChannel HpEventChannel;
    [SerializeField] public IntEventChannel ExpSetupEventChannel;
    [SerializeField] public IntEventChannel ExpEventChannel;

    [SerializeField] public IntEventChannel playerLevelEventChannel;
    [SerializeField] public IntEventChannel floorEventChannel;
    [SerializeField] public IntEventChannel enemyLeftEventChannel;
    [SerializeField] public IntEventChannel zenEventChannel;

    [SerializeField] public GameObject Sword;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject GameOverMenu;
    [SerializeField] private GameObject WinMenu;
    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine);
        walkingState = new PlayerWalkingState(this, stateMachine);
        stateMachine.Initiate(idleState);

        rigitBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        mazeGenerator = FindObjectOfType<MazeGenerator>();
        aStar = new AStar();
        currentTile = mazeGenerator.GetUserStartPosition();
        Vector3 currTilePosition = currentTile.tileGameObject.transform.position;
        player.transform.position = new Vector3(currTilePosition.x, 1.8f, currTilePosition.z);

        originalColor = currentTile.tileGameObject.GetComponentsInChildren<Renderer>().FirstOrDefault().material.color;
        lighterColor = originalColor + new Color(0.2f, 0.2f, 0.2f, 0);
        ColoringTile = new List<Tile>();

        userStatus.CurrentHealth = userStatus.Health;

        HpSetupEventChannel.RaiseEvent(userStatus.Health);
        ExpSetupEventChannel.RaiseEvent(20 + userStatus.Level * 5);
        ExpEventChannel.RaiseEvent(userStatus.CurrentExp);
        playerLevelEventChannel.RaiseEvent(userStatus.Level);
        floorEventChannel.RaiseEvent(userStatus.CurrentFloor);
        //enemyLeftEventChannel.RaiseEvent(gameManager.listEnemy.Count);
        zenEventChannel.RaiseEvent(userStatus.Zen);
    }

    void Update()
    {
        if (userStatus.CurrentHealth <= 0) return;
        stateMachine.currentState.Update();
    }

    //public void MoveUpdate()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, TileLayerMask))
    //    {
    //        Tile temp = mazeGenerator.GetTile(hit.transform.position);
    //        if (temp != null)
    //        {
    //            aStar.Initiate(mazeGenerator.Map, currentTile, temp);
    //            List<Tile> tempColor = aStar.Solve();
    //            ChangeColorOriginal(ColoringTile.Where(x => !tempColor.Any(y => y == x)).ToList());
    //            ColoringTile = tempColor;
    //            ChangeColor(ColoringTile);

    //            if (Input.GetMouseButtonDown(0) && tempColor.Count > 1)
    //            {
    //                tempColor.Reverse();
    //                List<Tile> loopMove = new List<Tile>(tempColor.Where(x => x != tempColor.FirstOrDefault()).ToList());
    //                gameManager.AddUserAction(PlayerMove, loopMove, false);
    //                gameManager.IsChanged = true;
    //                tempColor.Reverse();
    //            }

    //        }
    //    }
    //    else
    //    {
    //        ChangeColorOriginal(ColoringTile);
    //        ColoringTile.Clear();
    //    }
    //}
    public bool isInAction = false;
    public void PlayerMove(List<Tile> tile)
    {
        StartCoroutine(MoveCoroutine(tile));
    }

    private IEnumerator MoveCoroutine(List<Tile> tiles)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.FootStep, true);
        isInAction = true; 
        breakCoroutine = false;
        ChangeColorOriginal(ColoringTile);
        for (int i = 0; i < tiles.Count; i++)
        {
            FixCritSkill.instance.RefreshUpdate();
            HealSkill.instance.RefreshUpdate();
            var tile = tiles[i];
            currentTile = tile;
            Vector3 targetPosition = tile.tileGameObject.transform.position;

            targetPosition.y = 1.8f;
            Vector3 movementDelta = targetPosition - transform.position;

            Vector3 direction = new Vector3(movementDelta.x, 0, movementDelta.z).normalized;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                playerModel.transform.rotation = targetRotation;
            }

            rigitBody.velocity = movementDelta * 2.8f;

            yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 0.1f);

            if (gameManager.GetEnemy().Any(x => x.enemyState != Enemy.EnemyState.IDLE))
            {
                stateMachine.ChangeState(idleState);
                breakCoroutine = true;
            }

            rigitBody.velocity = Vector3.zero;
           
            ChangeColorOriginal(ColoringTile);

            if (breakCoroutine)
            {
                isInAction = false;
                audioManager.StopSFX();
                yield break;
            }
        }
        audioManager.StopSFX();
        isInAction = false;
    }

    public void PlayerAttack(Enemy enemy)
    {
        if (isInAction) return;
        StartCoroutine(AttackCoroutine(enemy));
    }

    private IEnumerator AttackCoroutine(Enemy enemy)
    {
        HealSkill.instance.RefreshUpdate();

        isInAction = true;
        // Make the player face the enemy (enemy's position)
        Vector3 playerTargetPosition = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z);
        transform.Find("HumanMale_Character_FREE").LookAt(playerTargetPosition);

        // Make the enemy face the player (player's position)
        Vector3 enemyTargetPosition = new Vector3(transform.position.x, enemy.transform.position.y, transform.position.z);
        enemy.transform.LookAt(enemyTargetPosition);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.SwordSlash);
        Sword.SetActive(true);
        System.Random rand = new System.Random();
        string animationText = "Attack" + rand.Next(0, 3).ToString();
        animator.Play(animationText);

        int critChange = rand.Next(1, 101);
        int dmg = 0;
        //Debug.Log(critChange);
        //Debug.Log(userStatus.CriticalRate);
        if (critChange <= userStatus.CriticalRate)
        {
            CameraShake.GetInstance().Shake();
            dmg = Utils.GetInstance().CalculateDamage(userStatus.Attack * userStatus.CriticalDamage / 100, enemy.enemyStatus.deffense, userStatus.CurrentFloor);
            DamagePopUpGenerator.instance.CreatePopUp(enemy.transform.position, dmg.ToString(), new Color(1f, 0.271f, 0f), 15);
        }
        else
        {
            dmg = Utils.GetInstance().CalculateDamage(userStatus.Attack, enemy.enemyStatus.deffense, userStatus.CurrentFloor);
            DamagePopUpGenerator.instance.CreatePopUp(enemy.transform.position, dmg.ToString(), Color.white);
        }
        
        enemy.DecreaseHP(dmg);
       
        
        enemy.animator.Play("GetHit");
        yield return new WaitForSeconds(1f);
        Sword.SetActive(false);
        animator.Play("Idle");
        if(enemy.enemyStatus.currentHealth > 0)
        {
            enemy.animator.Play("Idle");
        }
        else{
            AddZen(30 * (enemy.enemyType / 2 + 1) * (userStatus.CurrentFloor / 3 + 1));
            IncreaseExp(5 * (enemy.enemyType / 2 + 1) * (userStatus.CurrentFloor / 3 + 1));
            
            enemy.animator.Play("Death");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Death);
            yield return new WaitForSeconds(1.5f);
            enemy.DestroyEnemy();
        }


        if (gameManager.listEnemy.Any(x => x.enemyState == EnemyState.PREALERT))
        {
            List<Enemy> listTemp = gameManager.listEnemy.Where(x => x.enemyState == EnemyState.PREALERT).ToList();
            foreach (var item in listTemp)
            {
                item.enemyState = EnemyState.ALERT;
            }
        }
        if (gameManager.listEnemy.Any(x => x.enemyState == EnemyState.ALERT))
        {
            List<Enemy> listTemp = gameManager.listEnemy.Where(x => x.enemyState == EnemyState.ALERT).ToList();
            foreach (var item in listTemp)
            {
                item.CheckArgo();
            }
        }

        if(gameManager.listEnemy.Any(x => x.enemyState == EnemyState.AGRO))
        {
            gameManager.ChangeAction();
        }

        FixCritSkill.instance.UseSkill();
        FixCritSkill.instance.RefreshUpdate();

        isInAction = false;
        //if (enemy.enemyStatus.currentHealth <= 0) enemy.DestroyEnemy();
        //IsInAction = false;
        
    }

    public void ChangeColor(List<Tile> ListTile)
    {
        foreach (var item in ListTile)
        {
            Renderer[] childRenderers = item.tileGameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer objectRenderer in childRenderers)
            {
                lighterColor = new Color(
                    Mathf.Clamp01(lighterColor.r),
                    Mathf.Clamp01(lighterColor.g),
                    Mathf.Clamp01(lighterColor.b),
                    lighterColor.a
                );

                objectRenderer.material.color = lighterColor;
            }

        }
    }

    public void ChangeColorOriginal(List<Tile> ListTile)
    {
        foreach (var item in ListTile)
        {
            Renderer[] childRenderers = item.tileGameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer objectRenderer in childRenderers)
            {

                originalColor = new Color(
                    Mathf.Clamp01(originalColor.r),
                    Mathf.Clamp01(originalColor.g),
                    Mathf.Clamp01(originalColor.b),
                    originalColor.a
                );

                objectRenderer.material.color = originalColor;
            }

        }
    }
    public void DecreaseHP(int damage)
    {
        userStatus.CurrentHealth  -= damage;
        HpEventChannel.RaiseEvent(damage);
    }

    public void IncreaseExp(int exp)
    {
        userStatus.CurrentExp += exp;
        ExpEventChannel.RaiseEvent(exp);
        int maxExp = userStatus.Level * 5 + 20;
        if (userStatus.CurrentExp >= maxExp)
        {
            for (int i = 0; i < userStatus.CurrentExp / maxExp; i++)
            {
                IncreaseLevel();
            }
            userStatus.CurrentExp %= maxExp;
            ExpSetupEventChannel.RaiseEvent(20 + userStatus.Level * 5);
            ExpEventChannel.RaiseEvent(exp);
        }
    }

    public void IncreaseLevel()
    {
        DamagePopUpGenerator.instance.CreatePopUp(transform.position, "Level Up!", Color.yellow);
        userStatus.Level += 1;
        playerLevelEventChannel.RaiseEvent(userStatus.Level);
        FixCritSkill.instance.RefreshMinimumLevel();
        HealSkill.instance.RefreshMinimumLevel();   
    }

    public void DecreaseEnemyOnFloor()
    {
        enemyLeftEventChannel.RaiseEvent(gameManager.listEnemy.Count);
    }

    public void AddZen(int zen)
    {
        userStatus.Zen += zen;
        zenEventChannel.RaiseEvent(userStatus.Zen);
    }

    public void ShowGameOverScreen()
    {
        GameOverMenu.SetActive(true);   
    }

    public void ShowPauseScreen()
    {
        PauseMenu.SetActive(true);
    }

    public void ShowWinScreen()
    {
        WinMenu.SetActive(true);
    }
}
