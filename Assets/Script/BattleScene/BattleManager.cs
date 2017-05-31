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

    //誰のターンなのか
    public enum WhoseTurn
    {
        player,
        braver,
        princess,
        enemy
    };
    private WhoseTurn whoseTurn;

    //ターンを変更するとき何秒待つか
    [Range(0, 2)]
    public float changeTurnWaitTime;

    [Range(0, 1)]
    public float panelMoveTime;

    [Range(-30, 0)]
    public float panelMoveValue;

    [HideInInspector]
    public bool isPushed;

    //一番上にある、コマンド類が入った階層フォルダ
    [HideInInspector]
    public GameObject commandPanel;
    public string commandPanelPath;

    //メインとなるコマンドパネル
    [HideInInspector]
    public GameObject mainCommand;

    //今表示しているコマンドパネル
    [HideInInspector]
    public GameObject nowDetailCommand;

    [HideInInspector]
    public GameObject attackCommand;

    [HideInInspector]
    public GameObject toolCommand;

    [HideInInspector]
    public GameObject moveCommand;

    [HideInInspector]
    public GameObject escapeCommand;

    //今誰のコマンドを入力しているのか分かる、名前部分のテキスト
    [HideInInspector]
    public Text whoseNameText;
    public string whoseNameTextPath;

    private Text[] logText;
    public string logTextPath;

    [HideInInspector]
    public SelectArrow mainArrow;
    [HideInInspector]
    public SelectArrow subArrow;

    private EventSystem eventSystem;

    public const int COUNT_BASE_POS = 3;

    //黒い土台の場所
    public GameObject[,] basePositions = new GameObject[COUNT_BASE_POS, COUNT_BASE_POS];
    public string basePositionsPath;

    //このグリッドにオブジェクトを入れる
    public GameObject[,] gridPositions = new GameObject[COUNT_BASE_POS, COUNT_BASE_POS];

    //Enemyを格納
    public List<GameObject> enemies = new List<GameObject>();
    public string enemiesPath;

    //Enemyを全員攻撃させるためのカウンター+唯一変数にしたかったのでここに
    public int countEnemyTurn;

    //音楽系
    public AudioClass audioClass;
    private AudioSource soundBox;


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
        {
            instance.eventSystem.enabled = false;
            return;
        }
        else
            instance.eventSystem.enabled = true;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            OnCommandBaack();
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
            string buff = null;
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

    //読み込まれる毎に初期化
    void Initialized()
    {
        instance.player = FindObjectOfType<BattlePlayer>();
        instance.braver = FindObjectOfType<BattleBraver>();
        instance.princess = FindObjectOfType<BattlePrincess>();

        instance.isPushed = false;
        instance.commandPanel = GameObject.Find(commandPanelPath);
        instance.mainCommand = GameObject.Find(commandPanelPath + "Main");
        instance.attackCommand = GameObject.Find(commandPanelPath + "Attack");
        instance.toolCommand = GameObject.Find(commandPanelPath + "Tool");
        instance.moveCommand = GameObject.Find(commandPanelPath + "Move");
        instance.escapeCommand = GameObject.Find(commandPanelPath + "Escape");
        instance.whoseNameText = GameObject.Find(whoseNameTextPath).GetComponent<Text>();
        instance.logText = GameObject.Find(logTextPath).GetComponentsInChildren<Text>();
        instance.mainArrow = instance.mainCommand.transform.FindChild("MainArrow").GetComponent<SelectArrow>();
        instance.subArrow = instance.commandPanel.transform.FindChild("SubArrow").GetComponent<SelectArrow>();
        instance.eventSystem = FindObjectOfType<EventSystem>();
        instance.audioClass = FindObjectOfType<AudioClass>();
        instance.soundBox = FindObjectOfType<AudioClass>().gameObject.GetComponent<AudioSource>();

        for (int i = 0; i < logText.Length; i++)
            logText[i].text = null;

        //BasePositionsを左上[0, 0]を起点に取得
        foreach (Transform obj in GameObject.Find(basePositionsPath).GetComponent<Transform>())
        {
            int index = obj.transform.GetSiblingIndex();
            basePositions[index / COUNT_BASE_POS % COUNT_BASE_POS, index % COUNT_BASE_POS] = obj.gameObject;
        }

        countEnemyTurn = 0;
        enemies.Clear();
        var enemy = GameObject.Find(enemiesPath).GetComponentsInChildren<BattleEnemy>();
        foreach (var e in enemy)
            enemies.Add(e.gameObject);

        //最初は仮に入れておく
        instance.nowDetailCommand = instance.attackCommand;

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

    //メッセージログ
    public void AddMessage(string log) 
    {
        for (int i = 1; i < logText.Length; i++)
        {
            logText[i - 1].text = logText[i].text;
            logText[i].text = null;
        }
        logText[logText.Length - 1].text = log;
    }

    /********************
        * ターン関連 *
     ********************/

    //今誰のターンなのか取得
    public WhoseTurn GetWhoseTurn()
    {
        return instance.whoseTurn;
    }

    //Playerのターンにする+初期化
    public IEnumerator ChangeTurnPlayer()
    {
        yield return new WaitForSeconds(instance.changeTurnWaitTime);
        instance.whoseTurn = WhoseTurn.player;
        instance.subArrow.SetButtons(instance.player.buttonsObject);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = instance.player.objectName;
        instance.player.SetOnClick();
        instance.mainArrow.StartSelect();
    }

    //Braverのターンにする+初期化
    public IEnumerator ChangeTurnBraver()
    {
        yield return new WaitForSeconds(instance.changeTurnWaitTime);
        instance.whoseTurn = WhoseTurn.braver;
        instance.subArrow.SetButtons(instance.braver.buttonsObject);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = instance.braver.objectName;
        instance.braver.SetOnClick();
        instance.mainArrow.StartSelect();
    }

    //Princesのターンにする+初期化
    public IEnumerator ChangeTurnPrincess()
    {
        yield return new WaitForSeconds(instance.changeTurnWaitTime);
        instance.whoseTurn = WhoseTurn.princess;
        instance.subArrow.SetButtons(instance.princess.buttonsObject);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = instance.princess.objectName;
        instance.princess.SetOnClick();
        instance.mainArrow.StartSelect();
    }

    //Enemyのターンにする+初期化
    public IEnumerator ChangeTurnEnemy()
    {
        yield return new WaitForSeconds(instance.changeTurnWaitTime);
        instance.whoseTurn = WhoseTurn.enemy;
        countEnemyTurn = 0;
        StartCoroutine(NextEnemyTurn());
    }

    //次のターンに自動判断で切り替え
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

    /********************
        * Enemy関連 *
     ********************/

    //全てのEnemyを攻撃させる
    public IEnumerator NextEnemyTurn()
    {
        while (true)
        {
            yield return new WaitForSeconds((instance.changeTurnWaitTime+1f) / 2);

            //Enemyの数よりカウンターが回ったらプレイヤーのターンにする
            if (instance.countEnemyTurn >= enemies.Count)
            {
                instance.ChangeTurnNext();
                break;
            }

            enemies[instance.countEnemyTurn].GetComponent<BattleEnemy>().EnemyTurn();
        }
    }

    //なにもできないとき
    public void TurnSkip()
    {
        instance.countEnemyTurn++;
    }

    /********************
        * コマンド関連 *
     ********************/

    //サブコマンドからメインコマンドに戻るとき
    public void OnCommandBaack()
    {
        if (instance.isPushed && !FindObjectOfType<iTween>())
        {
            instance.isPushed = false;
            iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x - instance.panelMoveValue, "time", instance.panelMoveTime));
            instance.nowDetailCommand.SetActive(false);
            instance.mainArrow.StartSelect();
            instance.subArrow.StopSelect();
            soundBox.PlayOneShot(audioClass.cancel, 1f);
        }
    }

    //メインコマンドが押されたら
    public void OnCommandPushed()
    {
        if (!instance.isPushed && !FindObjectOfType<iTween>() && FadeSceneManager.IsFadeFinished())
        {
            instance.isPushed = true;
            iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x + instance.panelMoveValue, "time", instance.panelMoveTime));
            //GlobalCoroutine.Go(WaitTime(), panelMoveTime);
            Invoke("WaitTime", panelMoveTime);
            soundBox.PlayOneShot(audioClass.decide, 1f);
        }
    }

    void WaitTime()
    {
        instance.nowDetailCommand.gameObject.SetActive(true);
        instance.subArrow.StartSelect();
    }

    //IEnumerator WaitTime()
    //{
    //    instance.attackCommand.gameObject.SetActive(true);

    //    yield break;
    //}

    /********************
        * ボタン関連*
     ********************/

    //メインコマンド決定
    private void OnReady()
    {
        instance.OnCommandPushed();
        instance.mainArrow.StopSelect();
    }

    //準備ができているならtrue, できていないならfalsaeを返す
    public bool OnReadyDetails()
    {
        if (instance.isPushed && !FindObjectOfType<iTween>())
        {
            ChangeTurnNext();

            //instance.OnCommandBaack();
            instance.mainCommand.SetActive(false);

            instance.isPushed = false;
            iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x - instance.panelMoveValue, "time", instance.panelMoveTime));
            instance.nowDetailCommand.SetActive(false);
            instance.mainArrow.StartSelect();
            instance.subArrow.StopSelect();

            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnAttack()
    {
        instance.nowDetailCommand = instance.attackCommand;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons);

        OnReady();
    }

    public void OnTool()
    {
        instance.nowDetailCommand = instance.toolCommand;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons);

        OnReady();
    }

    public void OnMove()
    {
        instance.nowDetailCommand = instance.moveCommand;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons);

        OnReady();
    }

    public void OnMoveUp()
    {
        CommonBattleChara tmp = null;
        switch (GetWhoseTurn())
        {
            case WhoseTurn.player:
                tmp = instance.player.GetComponent<BattlePlayer>();
                break;

            case WhoseTurn.braver:
                tmp = instance.braver.GetComponent<BattleBraver>();
                break;

            case WhoseTurn.princess:
                tmp = instance.princess.GetComponent<BattlePrincess>();
                break;
        }

        tmp.OnMoveUp();
    }

    public void OnMoveDown()
    {
        CommonBattleChara tmp = null;
        switch (GetWhoseTurn())
        {
            case WhoseTurn.player:
                tmp = instance.player.GetComponent<BattlePlayer>();
                break;

            case WhoseTurn.braver:
                tmp = instance.braver.GetComponent<BattleBraver>();
                break;

            case WhoseTurn.princess:
                tmp = instance.princess.GetComponent<BattlePrincess>();
                break;
        }

        tmp.OnMoveDown();
    }

    public void OnEscape()
    {
        instance.nowDetailCommand = instance.escapeCommand;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons, new Vector3(-6, 0, 0));

        OnReady();
    }
}
