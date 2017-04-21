using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    //trueならPlayerターン, falseならEnemyターン
    public bool isTurn = true;

    [Range(0, 1)]
    public float panelMoveTime;

    [Range(-10, 10)]
    public float panelMoveValue;
    
    public bool isPushed = false;

    [HideInInspector]
    public RectTransform mainCommand;

    [HideInInspector]
    public RectTransform subCommand;



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
        Initialized();
    }


    void Update()
    {
        if (instance.isPushed)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) && !FindObjectOfType<iTween>())
            {
                instance.isPushed = false;
                iTween.MoveTo(instance.mainCommand.gameObject, iTween.Hash("x", instance.mainCommand.position.x - ConvertAspect.GetWidth(instance.panelMoveValue), "time", instance.panelMoveTime));
                subCommand.gameObject.SetActive(false);
            }
        }

        SelectArrow selectArrow = FindObjectOfType<SelectArrow>();
        if (Input.GetKeyDown(KeyCode.T))
        {
            selectArrow.StopSelect();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectArrow.StartSelect();
        }
    }

    void Initialized()
    {
        instance.isPushed = false;
        instance.mainCommand = GameObject.Find("Canvas/Command/Main").GetComponent<RectTransform>();
        instance.subCommand = GameObject.Find("Canvas/Command/Detail").GetComponent<RectTransform>();
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

        Initialized();
    }


    /*以下ボタン関数*/
    public void OnCommandPushed()
    {
        if (!instance.isPushed && !FindObjectOfType<iTween>())
        {
            instance.isPushed = true;
            iTween.MoveTo(instance.mainCommand.gameObject, iTween.Hash("x", instance.mainCommand.position.x + ConvertAspect.GetWidth(instance.panelMoveValue), "time", instance.panelMoveTime));
            GlobalCoroutine.Go(WaitTime(), panelMoveTime);
        }
    }



    IEnumerator WaitTime()
    {
        GameObject subCommand = GameObject.Find("Canvas/Command/Detail");
        subCommand.SetActive(true);

        yield break;
    }
}
