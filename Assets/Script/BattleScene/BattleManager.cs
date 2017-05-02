using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    //trueならPlayerターン, falseならEnemyターン
    [SerializeField]
    private bool isTurn;

    [Range(0, 1)]
    public float panelMoveTime;

    [Range(-10, 10)]
    public float panelMoveValue;

    public bool isPushed;

    [HideInInspector]
    public RectTransform mainCommand;

    [HideInInspector]
    public RectTransform subCommand;

    private EventSystem eventSystem;



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
        //シーン遷移時は十字キーで動作させない
        if (FadeSceneManager.IsFading())
            instance.eventSystem.enabled = false;
        else
            instance.eventSystem.enabled = true;

        if (instance.isPushed && FadeSceneManager.IsFadeFinished())
        {
            if (Input.GetKeyDown(KeyCode.Backspace) && !FindObjectOfType<iTween>())
            {
                OnCommandBaack();
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
        instance.isTurn = true;
        instance.isPushed = false;
        instance.mainCommand = GameObject.Find("Canvas/Command/Main").GetComponent<RectTransform>();
        instance.mainCommand.gameObject.SetActive(true);
        instance.subCommand = GameObject.Find("Canvas/Command/Detail").GetComponent<RectTransform>();
        instance.eventSystem = FindObjectOfType<EventSystem>();
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


    public bool GetTurn()
    {
        return instance.isTurn;
    }

    //Playerのターンにする+初期化
    public IEnumerator ChangeTurnPlayer(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.isTurn = true;
        instance.mainCommand.gameObject.SetActive(true);
    }

    //Enemyのターンにする+初期化
    public IEnumerator ChangeTurnEnemy(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.isTurn = false;
    }


    //サブコマンドからメインコマンドに戻るとき
    public void OnCommandBaack()
    {
        instance.isPushed = false;
        iTween.MoveTo(instance.mainCommand.gameObject, iTween.Hash("x", instance.mainCommand.position.x - ConvertAspect.GetWidth(instance.panelMoveValue), "time", instance.panelMoveTime));
        subCommand.gameObject.SetActive(false);
    }


    /*以下ボタン関数*/

    //メインコマンドが押されたら
    public void OnCommandPushed()
    {
        if (!instance.isPushed && !FindObjectOfType<iTween>() && FadeSceneManager.IsFadeFinished())
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
