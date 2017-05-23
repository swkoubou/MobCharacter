using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    public BattlePlayer player;
    public BattleBraver braver;
    public BattlePrincess princess;

    public enum WhoseTurn
    {
        player,
        braver,
        princess,
        enemy
    };
    private WhoseTurn whoseTurn;

    [Range(0, 1)]
    public float panelMoveTime;

    [Range(-30, 0)]
    public float panelMoveValue;

    [HideInInspector]
    public bool isPushed;

    [HideInInspector]
    public GameObject commandPanel;
    public string commandPanelPath;

    [HideInInspector]
    public GameObject mainCommand;

    [HideInInspector]
    public GameObject subCommand;

    [HideInInspector]
    public Text whoseNameText;
    public string whoseNameTextPath;

    [HideInInspector]
    public SelectArrow mainArrow;
    [HideInInspector]
    public SubArrow subArrow;

    private EventSystem eventSystem;

    public const int COUNT_BASE_POS = 3;
    public GameObject[,] basePositions = new GameObject[COUNT_BASE_POS, COUNT_BASE_POS];
    public GameObject[,] gridPositions = new GameObject[COUNT_BASE_POS, COUNT_BASE_POS];
    public string basePositionsPath;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //他のスクリプトからStart()でアクセスするため、準備をAwake()でする
        Initialized();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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

        if (GetWhoseTurn() != WhoseTurn.enemy)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                instance.commandPanel.SetActive(false);
            }
            else
            {
                instance.commandPanel.SetActive(true);
            }
        }

        //実験用
        if (Input.GetKeyDown(KeyCode.H))
        {
            //実験用
            string buff = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buff += "(" + i + "," + j + ")" + gridPositions[i, j] + "\n";
                }
            }
            print(buff);
        }
    }

    void Initialized()
    {
        instance.player = FindObjectOfType<BattlePlayer>();
        instance.braver = FindObjectOfType<BattleBraver>();
        instance.princess = FindObjectOfType<BattlePrincess>();

        instance.isPushed = false;
        instance.commandPanel = GameObject.Find(commandPanelPath);
        instance.mainCommand = GameObject.Find(commandPanelPath + "Main");
        instance.subCommand = GameObject.Find(commandPanelPath + "Sub");
        instance.whoseNameText = GameObject.Find(whoseNameTextPath).GetComponent<Text>();
        instance.mainArrow = instance.mainCommand.transform.FindChild("MainArrow").GetComponent<SelectArrow>();
        instance.subArrow = instance.commandPanel.transform.FindChild("SubArrow").GetComponent<SubArrow>();
        instance.eventSystem = FindObjectOfType<EventSystem>();

        //BasePositionsを左上[0, 0]を起点に取得
        foreach (Transform obj in GameObject.Find(basePositionsPath).GetComponent<Transform>())
        {
            int index = obj.transform.GetSiblingIndex();
            basePositions[index / COUNT_BASE_POS % COUNT_BASE_POS, index % COUNT_BASE_POS] = obj.gameObject;
        }

        //ターンをPlayerから始める
        StartCoroutine(ChangeTurnPlayer());
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

    //今誰のターンなのか取得
    public WhoseTurn GetWhoseTurn()
    {
        return instance.whoseTurn;
    }

    //Playerのターンにする+初期化
    public IEnumerator ChangeTurnPlayer(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.whoseTurn = WhoseTurn.player;
        instance.subArrow.RebootSelectButton(instance.player.buttonsObject);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = "モブ";
        instance.player.SetOnClick();
        instance.mainArrow.StartSelect();
    }

    //Braverのターンにする+初期化
    public IEnumerator ChangeTurnBraver(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.whoseTurn = WhoseTurn.braver;
        instance.subArrow.RebootSelectButton(instance.braver.buttonsObject);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = "勇者";
        instance.braver.SetOnClick();
        instance.mainArrow.StartSelect();
    }

    //Princesのターンにする+初期化
    public IEnumerator ChangeTurnPrincess(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.whoseTurn = WhoseTurn.princess;
        instance.subArrow.RebootSelectButton(instance.princess.buttonsObject);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = "姫";
    }

    //Enemyのターンにする+初期化
    public IEnumerator ChangeTurnEnemy(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.whoseTurn = WhoseTurn.enemy;
    }

    public void ChangeTurnNext()
    {
        switch (GetWhoseTurn())
        {
            case WhoseTurn.player:
                StartCoroutine(ChangeTurnBraver());
                break;

            case WhoseTurn.braver:
                StartCoroutine(ChangeTurnPrincess());
                break;

            case WhoseTurn.princess:
                StartCoroutine(ChangeTurnEnemy());
                break;

            case WhoseTurn.enemy:
                StartCoroutine(ChangeTurnPlayer());
                break;
        }
    }


    //サブコマンドからメインコマンドに戻るとき
    public void OnCommandBaack()
    {
        instance.isPushed = false;
        iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x - instance.panelMoveValue, "time", instance.panelMoveTime));
        instance.subCommand.SetActive(false);
        instance.mainArrow.StartSelect();
        instance.subArrow.StopSelect();
    }

    //メインコマンドが押されたら
    public void OnCommandPushed()
    {
        if (!instance.isPushed && !FindObjectOfType<iTween>() && FadeSceneManager.IsFadeFinished())
        {
            instance.isPushed = true;
            iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x + instance.panelMoveValue, "time", instance.panelMoveTime));
            GlobalCoroutine.Go(WaitTime(), panelMoveTime);
        }
    }

    IEnumerator WaitTime()
    {
        instance.subCommand.gameObject.SetActive(true);

        yield break;
    }

    //メインコマンド決定
    public void OnReady()
    {
        instance.OnCommandPushed();
        instance.mainArrow.StopSelect();
        instance.subArrow.StartSelect();
    }

    //準備ができているならtrue, できていないならfalsaeを返す
    public bool OnReadyDetails()
    {
        if (instance.isPushed && !FindObjectOfType<iTween>())
        {
            ChangeTurnNext();

            instance.OnCommandBaack();
            instance.mainCommand.SetActive(false);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnEscape()
    {

    }
}
