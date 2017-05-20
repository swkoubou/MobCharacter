using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    private BattlePlayer player;
    private BattleBraver braver;
    private BattlePrincess princess;

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

    [Range(-10, 10)]
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
    public GameObject playerCommand;
    [HideInInspector]
    public GameObject braverCommand;
    [HideInInspector]
    public GameObject princessCommand;

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
        instance.playerCommand = GameObject.Find(commandPanelPath + "Player");
        instance.braverCommand = GameObject.Find(commandPanelPath + "Braver");
        instance.princessCommand = GameObject.Find(commandPanelPath + "Princess");
        instance.whoseNameText = GameObject.Find(whoseNameTextPath).GetComponent<Text>();
        instance.mainArrow = instance.mainCommand.transform.FindChild("MainArrow").GetComponent<SelectArrow>();
        instance.subArrow = instance.commandPanel.transform.FindChild("SubArrow").GetComponent<SubArrow>();
        instance.eventSystem = FindObjectOfType<EventSystem>();

        //BasePositionsを左上[0, 0]を起点に取得
        foreach (Transform obj in GameObject.Find("Canvas/BasePositions").GetComponent<Transform>())
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

    //グリッド配列にキャラクタを設置
    public void AddGridPos(GameObject obj, Vector2 pos)
    {
        gridPositions[(int)pos.x, (int)pos.y] = obj;
    }

    public void ChangeGridPos(GameObject obj, Vector2 pos)
    {
        Vector2 lastPos = GetGridPosVector2(obj);
        gridPositions[(int)lastPos.x, (int)lastPos.y] = null;
        gridPositions[(int)pos.x, (int)pos.y] = obj;
    }

    //指定したオブジェクトがどこに設置されているか調べて返す
    public Vector2 GetGridPosVector2(GameObject obj)
    {
        for (int i = 0; i < COUNT_BASE_POS; i++)
        {
            for (int j = 0; j < COUNT_BASE_POS; j++)
            {
                if (gridPositions[i, j] == obj)
                {
                    return new Vector2(i, j);
                }
            }
        }

        //一致しなかったら-1, -1を返す
        return new Vector2(-1, -1);
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
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = "勇者";
        instance.braver.SetOnClick();
        instance.mainArrow.StartSelect();
    }

    //Princesのターンにする+初期化
    public IEnumerator ChangeTurnPrincess(float time = 1f)
    {
        //instance.princess.SetOnClick();

        yield return new WaitForSeconds(time);
        instance.whoseTurn = WhoseTurn.princess;
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = "姫";
    }

    //Enemyのターンにする+初期化
    public IEnumerator ChangeTurnEnemy(float time = 1f)
    {
        yield return new WaitForSeconds(time);
        instance.whoseTurn = WhoseTurn.enemy;
    }


    //サブコマンドからメインコマンドに戻るとき
    public void OnCommandBaack()
    {
        instance.isPushed = false;
        iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x - ConvertAspect.GetWidth(instance.panelMoveValue), "time", instance.panelMoveTime));
        instance.subCommand.SetActive(false);
        instance.mainArrow.StartSelect();
        instance.subArrow.StopSelect();
    }


    /*以下ボタン関数*/

    //メインコマンドが押されたら
    public void OnCommandPushed()
    {
        if (!instance.isPushed && !FindObjectOfType<iTween>() && FadeSceneManager.IsFadeFinished())
        {
            instance.isPushed = true;
            iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x + ConvertAspect.GetWidth(instance.panelMoveValue), "time", instance.panelMoveTime));
            GlobalCoroutine.Go(WaitTime(), panelMoveTime);
        }
    }



    IEnumerator WaitTime()
    {
        instance.subCommand.gameObject.SetActive(true);

        yield break;
    }
}
