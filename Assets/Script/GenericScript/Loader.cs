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

    public static string[] popMonster = new string[3] { "Skeleton", "Giant" , "Rabbit" };
    readonly private string skeleton = "Skeleton";
    readonly private string rabbit = "Rabbit";
    readonly private string giant = "Giant";

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
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FadeSceneManager.Execute(battleSceneName);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FadeSceneManager.Execute(boardSceneName);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            FadeSceneManager.Destroy();
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            for (int i = 0; i < 3; i++)
                popMonster[i] = skeleton;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            for (int i = 0; i < 3; i++)
                popMonster[i] = rabbit;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            popMonster[0] = rabbit;
            popMonster[1] = giant;
            popMonster[2] = skeleton;
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
