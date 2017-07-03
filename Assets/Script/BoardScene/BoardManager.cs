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

    //private int level = 0;
    

    [HideInInspector]
    public int playerHP;
    [HideInInspector]
    public int braverHP;

    public BoardBraver braver;
    private bool braverMoving = false;

    private List<BoardEnemy> enemies;
    private bool enemiesMoving = false;

    //音楽系
    public AudioClass audioClass;
    public AudioSource soundBox;

    private Text[] logText;
    public string logTextPath;

    public GameObject textManager;
    public string textManagerPath;
    public Text nameText;
    public Text contentText;


    public Image levelImage;

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
        //if(Loader.boardPlayerPos == new Vector2(-1, -1))
            Loader.level++;
        print(Loader.level);
        Loader.isForceEvent = false;
        mapGenerator.SetupScene(Loader.level);
        instance.audioClass = FindObjectOfType<AudioClass>();
        instance.soundBox = FindObjectOfType<AudioClass>().gameObject.GetComponent<AudioSource>();
        instance.logText = GameObject.Find(logTextPath).GetComponentsInChildren<Text>();
        for (int i = 0; i < logText.Length; i++)
            logText[i].text = null;
        instance.textManager = GameObject.Find(textManagerPath);
        instance.nameText = instance.textManager.transform.FindChild("Name").GetComponentInChildren<Text>();
        instance.contentText = instance.textManager.transform.FindChild("Content").GetComponentInChildren<Text>();
        instance.levelImage = GameObject.Find("UI/LevelImage").GetComponent<Image>();
        Invoke("Fade", 2f);
        instance.levelImage.gameObject.GetComponentInChildren<Text>().text = "第" + Loader.level + "階層";

        switch (Loader.level)
        {
            case 3:
            case 4:
                if (!Loader.isUseEscape[Loader.level])
                {
                    Loader.isForceEvent = true;
                    FindObjectOfType<BoardStory>().Execute();
                    enabled = false;
                    instance.textManager.SetActive(true);
                }
                else
                    instance.textManager.SetActive(false);
                break;

            default:
                Loader.isForceEvent = false;
                instance.textManager.SetActive(false);
                break;
        }
    }

    void Fade()
    {
        instance.levelImage.gameObject.SetActive(false);
    }


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = GameObject.FindGameObjectWithTag("Exit").gameObject.transform.position;
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Loader.level = 2;
        //}
        //if(whoseTurn == WhoseTurn.braver && !braverMoving)
        //    StartCoroutine(MoveBraver());

        if (whoseTurn == WhoseTurn.enemy && !enemiesMoving)
            StartCoroutine(MoveEnemies());
    }

    //メッセージログ
    public void AddMessage(string log)
    {
        if (log != null)
        {
            for (int i = 1; i < logText.Length; i++)
            {
                logText[i - 1].text = logText[i].text;
                logText[i].text = null;
            }
            logText[logText.Length - 1].text = log;
        }
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


    //IEnumerator MoveBraver()
    //{
    //    braverMoving = true;
    //    yield return new WaitForSeconds(turnDelay);
    //    braver.MoveBraver();
    //    braverMoving = false;
    //}


    public void AddEnemy(BoardEnemy script)
    {
        enemies.Add(script);
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        //yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
            yield break;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();

        }
        yield return new WaitForSeconds(enemies[0].moveTime);
        ChangeTurnPlayer();
        enemiesMoving = false;
    }

    public void DamagedAnim<T>(T other)
    {
        FlashingManager.Execute(other, FlashingManager.Hash("minAlpha", 0.3f, "color", Color.red, "count", 2));
    }
}
