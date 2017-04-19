using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    readonly public static string boardSceneName = "BoardScene";
    readonly public static string battleSceneName = "BattleScene";

    public static Loader instance = null;
    public BoardManager boardManager;
    public BattleManager battleManager;


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
            FadeManager.Execute(battleSceneName);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FadeManager.Execute(boardSceneName);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            FadeManager.Destroy();
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
