using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static BoardManager instance = null;
    public MapGenerator mapGenerator;
    public int playerFoodPoints = 100;
    [HideInInspector]
    public bool playersTurn = true;
    [HideInInspector]
    public bool braversTurn = false;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private bool doingSetup;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    public Braver braver;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        enemies = new List<Enemy>();
        mapGenerator = GetComponent<MapGenerator>();
        InitGame();
    }


    //void OnLevelWasLoaded(int index)
    //{
    //    level++;
    //    InitGame();
    //}


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

        level++;
        //Debug.Log(level);
        InitGame();
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("UI/LevelImage");
        levelText = GameObject.Find("UI/LevelImage/LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        mapGenerator.SetupScene(level);
    }


    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }


    public void GameOver()
    {
        levelText.text = "After " + level + " days, you started.";
        levelImage.SetActive(true);
        enabled = false;
    }


    void Update()
    {
        if (playersTurn || braversTurn || enemiesMoving || doingSetup)
            return;

        //if (!braversTurn)
            StartCoroutine(MoveBraver());

        //if(!enemiesMoving)
            StartCoroutine(MoveEnemies());
    }


    public void AddBraver(Braver script)
    {
        braver = script;
    }


    IEnumerator MoveBraver()
    {
        braversTurn = true;
        yield return new WaitForSeconds(turnDelay);
        braver.MoveBraver();
    }


    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        //if (enemies.Count == 0)
        //    yield break;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        braversTurn = false;
        enemiesMoving = false;
    }
}
