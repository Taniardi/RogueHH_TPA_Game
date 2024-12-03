using RPGCharacterAnims.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Enemy;

public class GameManager : MonoBehaviour
{
    [SerializeField] public MazeGenerator mazeGenerator;
    [SerializeField] public GameObject PlayerGameObject;
    [SerializeField] private GameObject EnemyGameObject;
    [SerializeField] private GameObject EnemyBossGameObject;
    [SerializeField] private LayerMask LOSlayerMask;
    [SerializeField] private Player player;
    [SerializeField] private UserStatus userstatus;
    [SerializeField] private GameObject NextFloorContainer;

    public List<Tuple<Action<List<Tile>>, List<Tile>>> UserAction;
    public List<Tuple<Action<List<Tile>>, List<Tile>>> EnemyAction;
    public List<Enemy> listEnemy;
    public int a = 10;

    public TURN Turn { get; set; }
    public bool IsChanged { get; set; }
    public bool IsStartCoroutine { get; set; }

    [SerializeField] private IntEventChannel enemyLeftEventChannel;

    private List<String> _angkatan = new List<string>()
    {
        "AC",
        "AS",
        "BD",
        "BT",
        "CT",
        "FO",
        "GN",
        "GY",
        "HO",
        "KH",
        "MM",
        "MR",
        "MV",
        "NS",
        "OV",
        "PL",
        "RU",
        "TI",
        "VD",
        "VM",
        "WS",
        "WW",
        "YD"
    };

    public void AddEnemy(Enemy enemy)
    {
        listEnemy.Add(enemy);
    }

    public List<Enemy> GetEnemy()
    {
        return listEnemy;
    }

    private void Awake()
    {
        listEnemy = new List<Enemy>();
        Turn = TURN.PLAYERTURN;
        IsStartCoroutine = true;
        IsChanged = false;
        UserAction = new List<Tuple<Action<List<Tile>>, List<Tile>>>();
        EnemyAction = new List<Tuple<Action<List<Tile>>, List<Tile>>>();
    }

    public void AddUserAction(Action<List<Tile>> action, List<Tile> tile, bool ChangeState)
    {
        UserAction.Add(new Tuple<Action<List<Tile>>, List<Tile>>(action, tile));
        this.IsChanged = ChangeState;
    }

    public void AddEnemeyAction(Action<List<Tile>> action, List<Tile> tile, bool ChangeState)
    {
        EnemyAction.Add(new Tuple<Action<List<Tile>>, List<Tile>>(action, tile));
        this.IsChanged = ChangeState;
    }

    public void ChangeAction()
    {
        if(Turn == TURN.PLAYERTURN)
        {
            Turn = TURN.ENEMYTURN;
        }
        else
        {
            Turn = TURN.PLAYERTURN;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && player.GetComponent<Player>().stateMachine.currentState == player.GetComponent<Player>().idleState && Turn == TURN.PLAYERTURN)
        {
            HealSkill.instance.RefreshUpdate();
            FixCritSkill.instance.RefreshUpdate();
        }

        if (Turn == TURN.PLAYERTURN && Input.GetKeyDown(KeyCode.Space) && listEnemy.Any(x =>  x.enemyState == EnemyState.AGRO))
        {
            ChangeAction();
        }

        if (Turn == TURN.PLAYERTURN && Input.GetKeyDown(KeyCode.Space) && listEnemy.Any(x =>  x.enemyState == EnemyState.PREALERT))
        {
            List<Enemy> listTemp = listEnemy.Where(x => x.enemyState == EnemyState.PREALERT).ToList();
            foreach (var item in listTemp)
            {
                item.enemyState = EnemyState.ALERT;
            }
        }
        if (Turn == TURN.PLAYERTURN && Input.GetKeyDown(KeyCode.Space) && listEnemy.Any(x =>  x.enemyState == EnemyState.ALERT))
        {
            List<Enemy> listTemp = listEnemy.Where(x => x.enemyState == EnemyState.ALERT).ToList() ;
            foreach (var item in listTemp)
            {
                item.CheckArgo();
            }
        }
    }


    private void Start()
    {

        if(userstatus.CurrentFloor == 0)
        {
            enemyLeftEventChannel.RaiseEvent(1);
            Enemy enemy = Instantiate(EnemyBossGameObject, new Vector3(0, 1.8f, 0), Quaternion.identity, transform).GetComponent<Enemy>();
            enemy.enemyType = 2;
            enemy.SetupEnemyUI(_angkatan.TakeRandom());
            enemy.SetupEnemyStat(player.userStatus.CurrentFloor);
            enemy.SetupHp();

            AudioManager.Instance.ChangeBackground(AudioManager.Instance.DungeonBackground); 
            StartCoroutine(ProcessAction());
            return;
        }
        System.Random rand = new System.Random();
        for (int i = 0; i < 6 + Mathf.FloorToInt(userstatus.CurrentFloor / 3); i++)
        {
            Enemy enemy = Instantiate(EnemyGameObject, new Vector3(0, 1.8f, 0), Quaternion.identity, transform).GetComponent<Enemy>();
            int temp = rand.Next(0, 11); 
            if(temp <= Mathf.Floor(player.userStatus.CurrentFloor / 10) - 1)
            {
                enemy.enemyType = 2;
            }else if(temp <= Mathf.Floor(player.userStatus.CurrentFloor / 10))
            {
                enemy.enemyType = 1;
            }
            else
            {
                enemy.enemyType = 0;
            }

            enemy.SetupEnemyUI(_angkatan.TakeRandom());
            enemy.SetupEnemyStat(player.userStatus.CurrentFloor);
            enemy.SetupHp();
        }

        //RefreshEnemyCount();
        AudioManager.Instance.ChangeBackground(AudioManager.Instance.DungeonBackground);

        StartCoroutine(ProcessAction());
    }

    public void RemoveEnemy(Enemy enemy)
    {
        listEnemy.Remove(enemy);
        RefreshEnemyCount();
    }

    public void RefreshEnemyCount()
    {
        enemyLeftEventChannel.RaiseEvent(listEnemy.Count);
        if(listEnemy.Count <= 0)
        {
            NextFloorContainer.SetActive(true);
        } 
    }

    private IEnumerator ProcessAction()
    {

        if (UserAction.Count > 0 && Turn == TURN.PLAYERTURN)
        {
            UserAction.FirstOrDefault().Item1.Invoke(UserAction.FirstOrDefault().Item2);
            UserAction.Remove(UserAction.FirstOrDefault());
            if (IsChanged && UserAction.Count == 0) ChangeAction();
        }
        
        List<Enemy> listEnemyArgo = listEnemy.Where(x => x.enemyState == Enemy.EnemyState.AGRO).ToList();
        if (Turn == TURN.ENEMYTURN && listEnemyArgo.Count != 0)
        {
            if (AudioManager.Instance.BackgroundSource.clip != AudioManager.Instance.CombatBackground)
            {
                AudioManager.Instance.ChangeBackground(AudioManager.Instance.CombatBackground);
            }

            //EnemyAction.FirstOrDefault().Item1.Invoke(EnemyAction.FirstOrDefault().Item2);
            //EnemyAction.Remove(EnemyAction.FirstOrDefault());
            //if (IsChanged && EnemyAction.Count == 0) ChangeAction();

            yield return new WaitForSeconds(0.3f);
            foreach (var item in listEnemyArgo)
            {
                item.EnemyMoveAction();
                yield return new WaitUntil(() => !item.IsInAction);
            }
            ChangeAction();
            yield return new WaitUntil(() => Turn == TURN.PLAYERTURN);
        }
        else if(listEnemyArgo.Count == 0)
        {
            if(AudioManager.Instance.BackgroundSource.clip != AudioManager.Instance.DungeonBackground)
            {
                AudioManager.Instance.ChangeBackground(AudioManager.Instance.DungeonBackground);
            }
        }

        yield return null;
        StartCoroutine(ProcessAction());
    }

    //public void CheckLOS(Enemy enemy)
    //{
    //    Debug.Log("masuk");
    //    Vector3 direction = PlayerGameObject.GetComponent<Player>().currentTile.tileGameObject.transform.position - enemy.transform.position;
    //    direction.y += 1.3f;
    //    Debug.DrawRay(new Vector3(enemy.transform.position.x, gameObject.transform.position.y + 0.6f, enemy.transform.position.z), direction, Color.red);

    //    if (Physics.Raycast(new Vector3(enemy.transform.position.x, gameObject.transform.position.y + 0.6f, enemy.transform.position.z), direction, out RaycastHit hit, 8.4f, LOSlayerMask))
    //    {
    //        Debug.Log("masuk nih");
    //        Debug.Log(hit.transform.gameObject.tag);
    //        if (hit.transform.gameObject.TryGetComponent<Player>(out Player p))
    //        {
    //            Debug.Log("keliatan ehe");
    //            enemy.enemyState = EnemyState.AGRO;
    //            enemy.AlertText.SetActive(false);
    //            enemy.AgroText.SetActive(true);
    //        }
    //        else
    //        {
    //            Debug.Log($"Line of sight blocked by {hit.collider.name}");
    //        }
    //    }
    //}

    public enum TURN
    {
        PLAYERTURN,
        ENEMYTURN
    }

}
