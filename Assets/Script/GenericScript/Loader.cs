using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    readonly public static string boardSceneName = "BoardScene";
    readonly public static string battleSceneName = "BattleScene";

    public static Loader instance = null;
    public BoardManager boardManager;
    public BattleManager battleManager;

    public static string[] popMonster = new string[3] { "SkeletonKnight", "Demon", "Ponee" };
    readonly private static string rabbit = "Rabbit";
    readonly private static string demon = "Demon";
    readonly private static string ponee = "Ponee";
    readonly private static string skeletonKnight = "SkeletonKnight";
    readonly private static string giant = "Giant";
    readonly private static string dragon = "Dragon";

    public string[] randomPop = new string[] { rabbit, demon, ponee, skeletonKnight };

    public static Vector2 boardPlayerPos = new Vector2(-1, -1);
    public static int level = 0;
    public static bool isForceEvent = false;

    //0は使わない
    public static bool[] isUseEscape = new bool[6];


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }


    void Start()
    {
        //instance.boardPlayerPos = new Vector2(-1, -1);
        //instance.level = -1;
    }


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    FadeSceneManager.Execute(battleSceneName);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    FadeSceneManager.Execute(boardSceneName);
        //}
        //else if (Input.GetKey(KeyCode.Alpha3))
        //{
        //    FadeSceneManager.Destroy();
        //}

        switch (level)
        {
            case 1:
            case 2:
                for (int i = 0; i < 3; i++)
                    popMonster[i] = randomPop[Random.Range(0, randomPop.Length)];
                break;

            case 3:
                popMonster[0] = randomPop[Random.Range(0, randomPop.Length)];
                popMonster[1] = giant;
                popMonster[2] = randomPop[Random.Range(0, randomPop.Length)];
                break;

            case 4:
                popMonster[0] = null;
                popMonster[1] = dragon;
                popMonster[2] = null;
                break;
        }
    }


    //シーンロード時のデリゲート
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (BoardManager.instance == null && SceneManager.GetActiveScene().name == boardSceneName)
            DontDestroyOnLoad(Instantiate(boardManager));

        if (BattleManager.instance == null && SceneManager.GetActiveScene().name == battleSceneName)
            DontDestroyOnLoad(Instantiate(battleManager));
    }
}
