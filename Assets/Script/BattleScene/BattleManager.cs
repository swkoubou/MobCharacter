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

    //操作キャラを格納
    public List<GameObject> players = new List<GameObject>();
    public string playersPath;

    //Enemyを格納
    public List<GameObject> enemies = new List<GameObject>();
    public string enemiesPath;

    //誰のターンなのか
    public enum WhoseTurn
    {
        braver,
        princess,
        player,
        enemy
    };
    private WhoseTurn whoseTurn;

    //なんのコマンドを操作しているのか
    public enum WhatCommand
    {
        none = -1,
        attack,
        idle,
        move,
        escape
    };
    private WhatCommand whatCommand;

    //ターンを変更するとき何秒待つか
    public const float CHANGE_TURN_WAIT_TIME = 1.2f;

    [Range(0, 1)]
    public float panelMoveTime;

    [Range(-30, 0)]
    public float panelMoveValue;

    [HideInInspector]
    public bool isPushed;

    private GameObject nowSelectedCharactor;

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
    public GameObject idleCommand;

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

    //Enemyを全員攻撃させるためのカウンター+唯一変数にしたかったのでここに
    public int countEnemyTurn;

    //音楽系
    public AudioClass audioClass;
    private AudioSource soundBox;

    //全キャラのコマンドをスタックするデリゲート
    public delegate void StackCommandPlayer();
    public StackCommandPlayer stackCommandPlayer;
    public delegate void StackCommandBraver();
    public StackCommandBraver stackCommandBraver;
    public delegate void StackCommandPrincess();
    public StackCommandPrincess stackCommandPrincess;


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

        if (GetWhoseTurn() != WhoseTurn.enemy && !FindObjectOfType<iTween>())
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                OnCommandBack(audioClass.cancel);
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                //CommandPanelにTraceLineを入れておくと予測線も消えてしまうため
                instance.commandPanel.SetActive(false);
                instance.subArrow.gameObject.GetComponent<Image>().enabled = false;
            }
            else if (instance.subArrow.enabled && (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)))
            {
                instance.subArrow.gameObject.GetComponent<Image>().enabled = true;
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            instance.player.DamagedAnim(10);
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
        instance.idleCommand = GameObject.Find(commandPanelPath + "Idle");
        instance.moveCommand = GameObject.Find(commandPanelPath + "Move");
        instance.escapeCommand = GameObject.Find(commandPanelPath + "Escape");
        instance.whoseNameText = GameObject.Find(whoseNameTextPath).GetComponent<Text>();
        instance.logText = GameObject.Find(logTextPath).GetComponentsInChildren<Text>();
        instance.mainArrow = instance.mainCommand.transform.FindChild("MainArrow").GetComponent<SelectArrow>();
        //instance.subArrow = instance.commandPanel.transform.FindChild("SubArrow").GetComponent<TraceLine>();
        instance.subArrow = FindObjectOfType<TraceLine>();
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

        players.Clear();
        var self = GameObject.Find(playersPath).GetComponentsInChildren<CommonBattleChara>();
        foreach (var e in self)
            players.Add(e.gameObject);

        enemies.Clear();
        var enemy = GameObject.Find(enemiesPath).GetComponentsInChildren<BattleEnemy>();
        foreach (var e in enemy)
            enemies.Add(e.gameObject);

        //最初は仮に入れておく
        instance.nowDetailCommand = instance.attackCommand;

        //ターンをBraverから始める
        StartCoroutine(ChangeTurnBraver());
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

    //Braverのターンにする+初期化
    public IEnumerator ChangeTurnBraver(float time = CHANGE_TURN_WAIT_TIME)
    {
        instance.whoseTurn = WhoseTurn.braver;
        if (instance.braver == null)
        {
            ChangeTurnNext();
            yield break;
        }

        yield return new WaitForSeconds(time);
        OnCommandBack(null);
        instance.whatCommand = WhatCommand.none;
        instance.subArrow.SetButtons(instance.braver.attackButtons);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = instance.braver.objectName;
        instance.braver.SetOnClickAttack();
        instance.braver.SetOnClickIdle();
        instance.mainArrow.StartSelect();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        Destroy(nowSelectedCharactor);
        nowSelectedCharactor = Instantiate(Resources.Load("SelectedCharactor"), instance.braver.gameObject.transform) as GameObject;
        nowSelectedCharactor.transform.parent = instance.braver.gameObject.transform;
    }

    //Princesのターンにする+初期化
    public IEnumerator ChangeTurnPrincess(float time = CHANGE_TURN_WAIT_TIME)
    {
        instance.whoseTurn = WhoseTurn.princess;
        if (instance.princess == null)
        {
            ChangeTurnNext();
            yield break;
        }

        yield return new WaitForSeconds(time);
        OnCommandBack(null);
        instance.whatCommand = WhatCommand.none;
        instance.subArrow.SetButtons(instance.princess.attackButtons);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = instance.princess.objectName;
        instance.princess.SetOnClickAttack();
        instance.princess.SetOnClickIdle();
        instance.mainArrow.StartSelect();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        Destroy(nowSelectedCharactor);
        nowSelectedCharactor = Instantiate(Resources.Load("SelectedCharactor"), instance.princess.gameObject.transform) as GameObject;
        nowSelectedCharactor.transform.parent = instance.princess.gameObject.transform;
    }

    //Playerのターンにする+初期化
    public IEnumerator ChangeTurnPlayer(float time = CHANGE_TURN_WAIT_TIME)
    {
        instance.whoseTurn = WhoseTurn.player;
        if (instance.player == null)
        {
            ChangeTurnNext();
            yield break;
        }

        yield return new WaitForSeconds(time);
        OnCommandBack(null);
        instance.whatCommand = WhatCommand.none;
        instance.subArrow.SetButtons(instance.player.attackButtons);
        instance.mainCommand.SetActive(true);
        instance.whoseNameText.text = instance.player.objectName;
        instance.player.SetOnClickAttack();
        instance.player.SetOnClickIdle();
        instance.mainArrow.StartSelect();
        
        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        Destroy(nowSelectedCharactor);
        nowSelectedCharactor = Instantiate(Resources.Load("SelectedCharactor"), instance.player.gameObject.transform) as GameObject;
        nowSelectedCharactor.transform.parent = instance.player.gameObject.transform;
    }

    //Enemyのターンにする+初期化
    public IEnumerator ChangeTurnEnemy(float time = CHANGE_TURN_WAIT_TIME)
    {
        instance.whoseTurn = WhoseTurn.enemy;
        OnReadyDetails();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        Destroy(nowSelectedCharactor);

        //デリゲートのスタックを吐き出す
        if (stackCommandBraver != null)
        {
            stackCommandBraver();
            stackCommandBraver = null;
        }
        yield return new WaitForSeconds(time);
        if (stackCommandPrincess != null)
        {
            stackCommandPrincess();
            stackCommandPrincess = null;
        }
        yield return new WaitForSeconds(time);
        if (stackCommandPlayer != null)
        {
            stackCommandPlayer();
            stackCommandPlayer = null;
        }
       
        //yield return new WaitForSeconds(time);
        instance.whatCommand = WhatCommand.none;
        instance.countEnemyTurn = 0;
        StartCoroutine(RotateEnemyTurn());
    }

    //前のターンに自動判断で切り替え
    public void ChangeTurnBack()
    {
        switch (GetWhoseTurn())
        {
            case WhoseTurn.braver:
                //AddMessage("これ以上戻れません");
                break;

            case WhoseTurn.princess:
                StartCoroutine(ChangeTurnBraver(0f));
                break;

            case WhoseTurn.player:
                StartCoroutine(ChangeTurnPrincess(0f));
                break;

            case WhoseTurn.enemy:
                break;
        }
    }

    //次のターンに自動判断で切り替え
    public void ChangeTurnNext()
    {
        switch (GetWhoseTurn())
        {
            case WhoseTurn.braver:
                StartCoroutine(ChangeTurnPrincess(0f));
                break;

            case WhoseTurn.princess:
                StartCoroutine(ChangeTurnPlayer(0f));
                break;

            case WhoseTurn.player:
                StartCoroutine(ChangeTurnEnemy());
                break;

            case WhoseTurn.enemy:
                StartCoroutine(ChangeTurnBraver());
                break;
        }
    }

    /********************
        * Enemy関連 *
     ********************/

    //全てのEnemyを攻撃させる
    public IEnumerator RotateEnemyTurn()
    {
        while (true)
        {
            yield return new WaitForSeconds(CHANGE_TURN_WAIT_TIME / 2f);
            
            //Enemyの数よりカウンターが回ったらプレイヤーのターンにする
            if (instance.countEnemyTurn >= enemies.Count)
            {
                ChangeTurnNext();
                yield break;
            }

            enemies[instance.countEnemyTurn].GetComponent<BattleEnemy>().EnemyTurn();
        }
    }

    //なにもできないとき
    public void NextEnemyTurn()
    {
        instance.countEnemyTurn++;
    }

    /********************
        * コマンド関連 *
     ********************/

    //今なんのコマンドを操作しているか
    public WhatCommand GetWhatCommand()
    {
        return instance.whatCommand;
    }

    //サブコマンドからメインコマンドに戻るとき
    public void OnCommandBack(AudioClip se)
    {
        if (instance.isPushed)
        {
            instance.whatCommand = WhatCommand.none;
            instance.isPushed = false;
            //iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x - instance.panelMoveValue, "time", instance.panelMoveTime));
        }
        else
        {
            //ChangeTurnから再帰でもう一度呼び出されてしまうため
            if (se != null)
                ChangeTurnBack();
        }
        instance.nowDetailCommand.SetActive(false);
        instance.mainArrow.StartSelect();
        instance.subArrow.StopSelect();
        soundBox.PlayOneShot(se, 1f);
    }

    //メインコマンドが押されたら
    public bool OnCommandPushed()
    {
        if (FadeSceneManager.IsFadeFinished() && !FindObjectOfType<iTween>())
        {
            instance.isPushed = true;
            //iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x + instance.panelMoveValue, "time", instance.panelMoveTime));
            //GlobalCoroutine.Go(WaitTime(), panelMoveTime);
            Invoke("WaitTime", panelMoveTime);
            soundBox.PlayOneShot(audioClass.decide, 1f);

            return true;
        }
        else
        {
            return false;
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

    public void Win()
    {
        print("You Win");
    }

    public void Lose()
    {
        print("You Lose");
    }

    /********************
        * ボタン関連*
     ********************/

    //メインコマンド決定
    private void OnReady()
    {
        if (instance.OnCommandPushed())
        {
            instance.mainArrow.StopSelect();
        }
    }

    //準備ができているならtrue, できていないならfalsaeを返す
    public void OnReadyDetails()
    {
        instance.mainCommand.SetActive(false);

        instance.whatCommand = WhatCommand.none;
        instance.isPushed = false;
        //iTween.MoveTo(instance.mainCommand, iTween.Hash("x", instance.mainCommand.transform.position.x - instance.panelMoveValue, "time", instance.panelMoveTime));
        instance.nowDetailCommand.SetActive(false);
        instance.mainArrow.StartSelect();
        instance.subArrow.StopSelect();
    }

    public void OnAttack()
    {
        instance.nowDetailCommand = instance.attackCommand;
        instance.whatCommand = WhatCommand.attack;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons);

        OnReady();
    }

    public void OnIdle()
    {
        instance.nowDetailCommand = instance.idleCommand;
        instance.whatCommand = WhatCommand.idle;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons, new Vector3(-5, 0, 0));

        OnReady();
    }

    public void OnMove()
    {
        instance.nowDetailCommand = instance.moveCommand;
        instance.whatCommand = WhatCommand.move;

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
        instance.whatCommand = WhatCommand.escape;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons, new Vector3(-5, 0, 0));

        OnReady();
    }
}
