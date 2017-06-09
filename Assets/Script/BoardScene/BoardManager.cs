using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    //public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static BoardManager instance = null;
    private MapGenerator mapGenerator;

    private int level = 1;

    [HideInInspector]
    public int playerHP;
    [HideInInspector]
    public int braverHP;

    public BoardBraver braver;
    private bool braverMoving = false;

    private List<BoardEnemy> enemies;
    private bool enemiesMoving = false;


    public enum WhoseTurn
    {
        player,
        braver,
        enemy
    };
    private WhoseTurn whoseTurn = WhoseTurn.player;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        enemies = new List<BoardEnemy>();
        mapGenerator = GetComponent<MapGenerator>();
        InitGame();
    }


    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name != Loader.boardSceneName)
        {
            //デリゲートを削除しないとこのメソットも実行されエラーが発生するため
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }

        InitGame();
    }


    void InitGame()
    {
        enemies.Clear();
        mapGenerator.SetupScene(4);
    }


    void Update()
    {
        if(whoseTurn == WhoseTurn.braver && !braverMoving)
            StartCoroutine(MoveBraver());

        if (whoseTurn == WhoseTurn.enemy && !enemiesMoving)
            StartCoroutine(MoveEnemies());
    }

    public WhoseTurn GetWhoseTurn()
    {
        return instance.whoseTurn;
    }

    //Playerのターンにする+初期化
    public void ChangeTurnPlayer()
    {
        instance.whoseTurn = WhoseTurn.player;
    }

    //Braverのターンにする+初期化
    public void ChangeTurnBraver()
    {
        instance.whoseTurn = WhoseTurn.braver;
    }

    //Enemyのターンにする+初期化
    public void ChangeTurnEnemy()
    {
        instance.whoseTurn = WhoseTurn.enemy;
    }


    public void AddBraver(BoardBraver script)
    {
        braver = script;
    }


    IEnumerator MoveBraver()
    {
        braverMoving = true;
        yield return new WaitForSeconds(turnDelay);
        braver.MoveBraver();
        braverMoving = false;
    }


    public void AddEnemy(BoardEnemy script)
    {
        enemies.Add(script);
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
            yield break;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        enemiesMoving = false;
    }
}
