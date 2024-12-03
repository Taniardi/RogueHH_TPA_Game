using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask decorationLayerMask;

    [SerializeField] public GameObject AlertText;
    [SerializeField] public GameObject AgroText;
    [SerializeField] public GameObject Sword;


    [SerializeField] public Animator animator;
    [SerializeField] private Slider HpBarSlider;
    private GameObject gameOverScreen;

    private Rigidbody rigitBody;
    private Tile playerPreAlertTile;

    public Tile currentTile;
    public EnemyState enemyState;

    private AStar astar;
    public EnemyStatus enemyStatus;

    public bool IsInAction;
    public int enemyType;

    [SerializeField] private GameObject clothes;
    [SerializeField] private GameObject armor;
    [SerializeField] private Text nameText;

    private void Awake()
    {
        IsInAction = false;
        astar = new AStar();
        rigitBody = GetComponent<Rigidbody>();  
        currentTile = null;
        enemyState = EnemyState.IDLE;
    }

    public void EnemyRefreshPosition()
    {
        Vector3 currTilePosition = currentTile.tileGameObject.transform.position;
        transform.position = new Vector3(currTilePosition.x, 1.8f, currTilePosition.z);
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.FindWithTag("Player");
        gameManager.listEnemy.Add(this);
        gameManager.mazeGenerator.DistributeEnemy(gameManager.GetEnemy(), gameManager.GetEnemy().Count, this, player.GetComponent<Player>());
    }

    void Update()
    {
        if (gameManager.Turn != GameManager.TURN.ENEMYTURN) return;
        if (enemyState != EnemyState.AGRO) return;
         
        //playerPreAlertTile = player.GetComponent<Player>().currentTile;

        //astar.Initiate(gameManager.mazeGenerator.Map, currentTile, player.GetComponent<Player>().currentTile);

        //List<Tile> tile = astar.Solve();
        //gameManager.AddEnemeyAction(EnemyMove, tile.Where(x => x == tile.LastOrDefault()).ToList(), true);
    }

    public List<Tile> getTileAround(Tile tile)
    {
        List<int> moveX = new List<int>() { 1, -1, 0, 0};
        List<int> moveY = new List<int>() { 0, 0, 1, -1 };

        List<Tile> result = new List<Tile>();

        for (int i = 0; i < 4; i++)
        {
            int newX = (int)tile.position.x + moveX[i];
            int newY = (int)tile.position.y + moveY[i];

            if (newX < 0 || newX >= gameManager.mazeGenerator.widthMap) continue;

            if (newY < 0 || newY >= gameManager.mazeGenerator.heigthMap) continue;

            if (gameManager.mazeGenerator.Map[newY][newX] == null) continue;

            Tile.TileCondition tileCondition = gameManager.mazeGenerator.Map[newY][newX].tileCondition;
            if (tileCondition != Tile.TileCondition.CLEAR && tileCondition != Tile.TileCondition.PATTERN && tileCondition != Tile.TileCondition.GATE) continue;

            result.Add(gameManager.mazeGenerator.Map[newY][newX]);
        }

        return result; 
    }

    public Tile getNearestPlayerTile(List<Tile> tile)
    {
        Debug.Log("MASUK sini");
        tile = tile.OrderBy(x => Math.Abs(x.tileGameObject.transform.position.x - player.GetComponent<Player>().currentTile.tileGameObject.transform.position.x)
        + Math.Abs(x.tileGameObject.transform.position.z - player.GetComponent<Player>().currentTile.tileGameObject.transform.position.z)).ToList();

        foreach (var item in tile)
        {
            if(gameManager.listEnemy.Where(x => x != this).Any(x => x.currentTile == item))
            {
                return null;
            }
            if (player.GetComponent<Player>().currentTile == item)
            {
                return null;
            }
            if (item == currentTile) return null;
            return item;
        }


        return null;
    }

    public void EnemyMoveAction()
    {
        if (IsInAction) return;  // Jika sedang bergerak, jangan jalankan lagi

        IsInAction = true;

        playerPreAlertTile = player.GetComponent<Player>().currentTile;
        astar.Initiate(gameManager.mazeGenerator.Map, currentTile, player.GetComponent<Player>().currentTile, gameManager.listEnemy.Where(x => x != this).Select(x => x.currentTile).ToList());

        List<Tile> tile = astar.Solve(this);

        if (tile.Count == 0)
        {
            List<Tile> listTile = getTileAround(currentTile);
             if(listTile.Count > 0)
            {
                Tile nearestPlayerTile = getNearestPlayerTile(listTile);
                if(nearestPlayerTile != null)
                {
                    EnemyMove(nearestPlayerTile);
                }
                else
                {
                    IsInAction = false;
                }
            }
            else
            {
                IsInAction = false;
            }
            return;
        }

        if (!Utils.GetInstance().CheckPositionXZ(tile.LastOrDefault(), playerPreAlertTile))
        {
            EnemyMove(tile.LastOrDefault());
        }
        else if (tile.Count == 1)
        {
            EnemyAttack(); 
        }
    }

    public void SetupEnemyUI(string name)
    {
        this.nameText.text = name;

        if(enemyType == 1)
        {
            clothes.SetActive(true);
            this.nameText.color = Color.yellow;
        }else if(enemyType == 2)
        {
            armor.SetActive(true);  
            this.nameText.color = Color.red;
        }
    }

    public void SetupEnemyStat(int floor)
    {
        if(floor == 0)
        {
            this.enemyStatus = new EnemyStatus(500, 250, 200, 85, 300);
        }
        else
        {
            this.enemyStatus = new EnemyStatus((10 + floor) * (enemyType + 1) / 2, (5 + floor) * (enemyType + 1) / 2, (3 + floor) * (enemyType + 1) / 2, Mathf.Min(90, 5 + floor),100 + (floor) * (enemyType / 2 + 1));
        }
    }

    public void EnemyAttack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        Vector3 playerTargetPosition = new Vector3(this.transform.position.x, player.transform.position.y, this.transform.position.z);
        player.transform.Find("HumanMale_Character_FREE").LookAt(playerTargetPosition);

        Vector3 enemyTargetPosition = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
        this.transform.LookAt(enemyTargetPosition);

        if (enemyType != 0)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.SwordSlash);
            Sword.SetActive(true);
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Punch);
        }
        animator.Play("Attack");


        //int dmg = Utils.GetInstance().CalculateDamage(enemyStatus.attack, player.GetComponent<Player>().userStatus.Defense, player.GetComponent<Player>().userStatus.CurrentFloor);
        System.Random rand = new System.Random();
        int critChange = rand.Next(1, 101);
        int dmg = 0;
        Debug.Log(critChange);
        Debug.Log(enemyStatus.criticalRate);
        if (critChange <= enemyStatus.criticalRate)
        {
            CameraShake.GetInstance().Shake();
            Debug.Log(enemyStatus.attack * enemyStatus.criticalDamage / 100);
            dmg = Utils.GetInstance().CalculateDamage(enemyStatus.attack * enemyStatus.criticalDamage / 100, player.GetComponent<Player>().userStatus.Defense, player.GetComponent<Player>().userStatus.CurrentFloor);
            Debug.Log(dmg);
            DamagePopUpGenerator.instance.CreatePopUp(player.transform.position, dmg.ToString(), new Color(1f, 0.271f, 0f), 15);  
        }
        else
        {
            dmg = Utils.GetInstance().CalculateDamage(enemyStatus.attack, player.GetComponent<Player>().userStatus.Defense, player.GetComponent<Player>().userStatus.CurrentFloor);
            DamagePopUpGenerator.instance.CreatePopUp(player.transform.position, dmg.ToString(), Color.white);
        }

        player.GetComponent<Player>().userStatus.CurrentHealth -= dmg;
        player.GetComponent<Player>().HpEventChannel.RaiseEvent(dmg);


        player.GetComponent<Player>().animator.Play("GetHit");
        yield return new WaitForSeconds(0.5f);
        Sword.SetActive(false);
        player.GetComponent<Player>().animator.Play("Idle");
        animator.Play("Idle");

        if (player.GetComponent<Player>().userStatus.CurrentHealth <= 0)
        {
            //Debug.Log($"{player.GetComponent<Player>().userStatus.CurrentHealth}");
            gameManager.ChangeAction();
            player.GetComponent<Player>().animator.Play("Death");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Death);
            yield return new WaitForSeconds(1.5f);
            player.GetComponent<Player>().ShowGameOverScreen();
        }

        IsInAction = false;
    }

    public void EnemyMove(Tile tile)
    {
        StartCoroutine(MoveCoroutine(tile));
    }

    private IEnumerator MoveCoroutine(Tile tile)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.FootStep, true);
        currentTile = tile;
        Vector3 targetPosition = tile.tileGameObject.transform.position;

        targetPosition.y = 1.8f;
        Vector3 movementDelta = targetPosition - transform.position;

        Vector3 direction = new Vector3(movementDelta.x, 0, movementDelta.z).normalized;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            gameObject.transform.rotation = targetRotation;
        }

        rigitBody.velocity = movementDelta * 2.8f;
        animator.Play("RunForward");
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 0.1f);
        audioManager.StopSFX();
        rigitBody.velocity = Vector3.zero;
        animator.Play("Idle");

        // Setelah selesai bergerak, tandai isMoving menjadi false
        IsInAction = false;
    }


    //public void EnemyMoveAction()
    //{
    //    playerPreAlertTile = player.GetComponent<Player>().currentTile;

    //    astar.Initiate(gameManager.mazeGenerator.Map, currentTile, player.GetComponent<Player>().currentTile, gameManager.listEnemy.Where(x => x != this).Select(x => x.currentTile).ToList());

    //    List<Tile> tile = astar.Solve(this);

    //    if(!Utils.CheckPositionXZ(tile.LastOrDefault(), playerPreAlertTile))
    //    {
    //        Debug.Log("nice"); 
    //        EnemyMove(tile.LastOrDefault());
    //    }else if(tile.Count == 1)
    //    {
    //        player.GetComponent<Player>().HpEventChannel.RaiseEvent(5);
    //    }
    //}

    //private bool breakCoroutine = false;
    //public void EnemyMove(Tile tile)
    //{
    //    StartCoroutine(MoveCoroutine(tile));
    //}

    //private IEnumerator MoveCoroutine(Tile tile)
    //{
    //        currentTile = tile;
    //        Vector3 targetPosition = tile.tileGameObject.transform.position;

    //        targetPosition.y = 1.8f;
    //        Vector3 movementDelta = targetPosition - transform.position;

    //        Vector3 direction = new Vector3(movementDelta.x, 0, movementDelta.z).normalized;
    //        if (direction.magnitude > 0.1f)
    //        {
    //            Quaternion targetRotation = Quaternion.LookRotation(direction);
    //            gameObject.transform.rotation = targetRotation;
    //        }

    //        rigitBody.velocity = movementDelta * 2.8f;
    //        animator.Play("RunForward");
    //        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPosition) < 0.1f);

    //        if (gameManager.GetEnemy().Any(x => x.enemyState != Enemy.EnemyState.IDLE))
    //        {
    //            //stateMachine.ChangeState(idleState);
    //            breakCoroutine = true;
    //        }

    //        rigitBody.velocity = Vector3.zero;
    //        animator.Play("Idle");


    //        if (breakCoroutine)
    //        {
    //            yield break;
    //        }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (playerPreAlertTile == null) return;

        if (enemyState == EnemyState.AGRO) return;

        if (enemyState == EnemyState.PREALERT)
        {
            if (player.GetComponent<Player>().currentTile != playerPreAlertTile)
            {
                enemyState = EnemyState.ALERT;
            }
        }

        Vector3 direction = player.GetComponent<Player>().currentTile.tileGameObject.transform.position - gameObject.transform.position;
        direction.y += 1.3f;
        Debug.DrawRay(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.6f, gameObject.transform.position.z), direction, Color.red);
        
        if (player.GetComponent<Player>().rigitBody.velocity.magnitude <= 0f || player.GetComponent<Player>().currentTile == playerPreAlertTile)
        {
            return;
        }

        if (enemyState == EnemyState.ALERT)
        {
            CheckArgo();
        }
    }

    public void CheckArgo()
    {
        Vector3 direction = player.GetComponent<Player>().currentTile.tileGameObject.transform.position - gameObject.transform.position;
        direction.y += 1.3f;

        playerPreAlertTile = player.GetComponent<Player>().currentTile;
        direction = playerPreAlertTile.tileGameObject.transform.position - gameObject.transform.position;
        direction.y += 1.3f;
        if (Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.6f, gameObject.transform.position.z), direction, out RaycastHit hit, 8.4f, decorationLayerMask))
        {
            Debug.Log(hit.transform.gameObject.tag);
            if (hit.transform.gameObject.TryGetComponent<Player>(out Player p))
            {
                enemyState = EnemyState.AGRO;
                Vector3 enemyTargetPosition = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
                this.transform.LookAt(enemyTargetPosition);
                AlertText.SetActive(false);
                DamagePopUpGenerator.instance.CreatePopUp(transform.position, "!!", Color.red);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.EnemyAgro);
            }
            else
            {
                if (Vector3.Distance(gameObject.transform.position, hit.transform.position) > Vector3.Distance(gameObject.transform.position, player.transform.position))
                {
                    enemyState = EnemyState.AGRO;
                    AlertText.SetActive(false);
                    //AgroText.SetActive(true);
                    Vector3 enemyTargetPosition = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
                    this.transform.LookAt(enemyTargetPosition);
                    DamagePopUpGenerator.instance.CreatePopUp(transform.position, "!!", Color.red);
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.EnemyAgro);
                }
                else
                {
                    //Debug.Log($"Line of sight blocked by {hit.collider.name}");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player outPlayer))
        {
            if(enemyState != EnemyState.AGRO)
            {
                AlertText.SetActive(true);
                enemyState = EnemyState.PREALERT;
                playerPreAlertTile = outPlayer.currentTile;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player outPlayer))
        {
            if (enemyState != EnemyState.AGRO)
            {
                AlertText.SetActive(false);
                if(enemyState != EnemyState.AGRO) enemyState = EnemyState.IDLE;
            }
        }
    }

    public void SetupHp()
    {
        HpBarSlider.maxValue = enemyStatus.maxHealth;
        HpBarSlider.value = enemyStatus.maxHealth;
    }

    public void DecreaseHP(int damage)
    {
        enemyStatus.currentHealth -= damage;
        HpBarSlider.value -= damage;

        if(enemyStatus.currentHealth <= 0)
        {
            gameManager.RemoveEnemy(this);
        }
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public enum EnemyState
    {
        IDLE,
        PREALERT,
        ALERT,
        AGRO
    }
}
