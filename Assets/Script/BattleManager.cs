using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;
    public float panelMoveSpeed = 0.1f;
    public float panelMoveValue = -200f;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Update()
    {

    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name != Loader.battleSceneName)
        {
            //デリゲートを削除しないとこのメソットも実行されエラーが発生するため
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }
    }


    /*以下ボタン関数*/
    public void OnCommandPushed()
    {
        GameObject mainCommand = GameObject.Find("Canvas/Command/Main");
        iTween.MoveTo(mainCommand, iTween.Hash("x", mainCommand.transform.position.x + panelMoveValue, "time", panelMoveSpeed));
        GlobalCoroutine.Go(WaitTime(), panelMoveSpeed);
    }

    IEnumerator WaitTime()
    {
        GameObject subCommand = GameObject.Find("Canvas/Command/Detail");
        subCommand.SetActive(true);

        yield break;
    }
}
