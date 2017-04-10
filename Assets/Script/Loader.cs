using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    readonly public static string boardSceneName = "BoardScene";
    readonly public static string battleSceneName = "BattleScene";

    public BoardManager boardManager;
    public BattleManager battleManager;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(battleSceneName);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(boardSceneName);
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
