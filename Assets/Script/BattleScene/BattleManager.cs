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
        tool,
        idle,
        move,
        escape
    };
    private WhatCommand whatCommand;

    //ターンを変更するとき何秒待つか
    public const float CHANGE_TURN_WAIT_TIME = 1f;
    public const float CHANGE_TURN_PLAYERS = 0.2f;

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

    //勇者と姫のメインとなるコマンドパネル
    private GameObject mainOtherCommand;

    //モブのメインとなるコマンドパネル
    private GameObject mainPlayerCommand;

    //今表示しているコマンドパネル
    private GameObject nowDetailCommand;
    private GameObject attackCommand;
    private GameObject toolCommand;
    private GameObject idleCommand;
    private GameObject moveCommand;
    private GameObject escapeCommand;

    //今誰のコマンドを入力しているのか分かる、名前部分のテキスト
    private Text whoseNameTextOther;
    private Text whoseNameTextPlayer;

    private Text[] logText;
    public string logTextPath;

    private SelectArrow mainOtherArrow;
    private SelectArrow mainPlayerArrow;
    private SelectArrow subArrow;

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
    public AudioSource soundBox;

    //画像系
    public Sprite winImage;
    public Sprite loseImage;

    private bool isGameEnd;

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
        //ゲームを動作させない
        if (instance.isGameEnd)
        {
            if (Input.anyKeyDown)
                FadeSceneManager.Execute(Loader.boardSceneName);

            return;
        }

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
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
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
    }

    //読み込まれる毎に初期化
    void Initialized()
    {
        instance.player = FindObjectOfType<BattlePlayer>();
        instance.braver = FindObjectOfType<BattleBraver>();
        instance.princess = FindObjectOfType<BattlePrincess>();

        instance.isPushed = false;
        instance.nowSelectedCharactor = null;
        instance.commandPanel = GameObject.Find(commandPanelPath);
        instance.mainOtherCommand = GameObject.Find(commandPanelPath + "MainOther");
        instance.mainPlayerCommand = GameObject.Find(commandPanelPath + "MainPlayer");
        instance.attackCommand = GameObject.Find(commandPanelPath + "Attack");
        instance.toolCommand = GameObject.Find(commandPanelPath + "Tool");
        instance.idleCommand = GameObject.Find(commandPanelPath + "Idle");
        instance.moveCommand = GameObject.Find(commandPanelPath + "Move");
        instance.escapeCommand = GameObject.Find(commandPanelPath + "Escape");
        instance.whoseNameTextOther = instance.mainOtherCommand.GetComponentInChildren<Text>();
        instance.whoseNameTextPlayer = instance.mainPlayerCommand.GetComponentInChildren<Text>();
        instance.logText = GameObject.Find(logTextPath).GetComponentsInChildren<Text>();
        instance.mainOtherArrow = instance.mainOtherCommand.GetComponentInChildren<SelectArrow>();
        instance.mainPlayerArrow = instance.mainPlayerCommand.GetComponentInChildren<SelectArrow>();
        //instance.subArrow = instance.commandPanel.transform.FindChild("SubArrow").GetComponent<TraceLine>();
        instance.subArrow = FindObjectOfType<TraceLine>();
        instance.eventSystem = FindObjectOfType<EventSystem>();
        instance.audioClass = FindObjectOfType<AudioClass>();
        instance.soundBox = FindObjectOfType<AudioClass>().gameObject.GetComponent<AudioSource>();
        instance.isGameEnd = false;

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
        if (GameObject.Find(enemiesPath).GetComponentsInChildren<BattleEnemy>().Length == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Loader.popMonster[i] != null)
                {
                    GameObject generate = Instantiate(Resources.Load(Loader.popMonster[i])) as GameObject;
                    generate.GetComponent<BattleEnemy>().defaultPos = new Vector2(i, 1);
                    generate.transform.SetParent(GameObject.Find(enemiesPath).transform);
                }
            }
        }
        var enemy = GameObject.Find(enemiesPath).GetComponentsInChildren<BattleEnemy>();
        foreach (var e in enemy)
            enemies.Add(e.gameObject);

        //最初は仮に入れておく
        instance.nowDetailCommand = instance.attackCommand;

        //ターンをBraverから始める
        StartCoroutine(ChangeTurnBraver());

        if(Loader.level == 4)
        {
            var bgm = GameObject.Find("BGM").GetComponent<AudioSource>();
            bgm.clip = audioClass.boss;
            bgm.Play();
        }
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
        if (log != null)
        {
            for (int i = 1; i < logText.Length; i++)
            {
                logText[i - 1].text = logText[i].text;
                logText[i].text = null;
            }
            logText[logText.Length - 1].text = log;
        }
    }

    /********************
        * ターン関連 *
     ********************/

    //今誰のターンなのか取得
    public WhoseTurn GetWhoseTurn()
    {
        return instance.whoseTurn;
    }

    private void GenerateCursor(CommonBattleChara target)
    {
        if (instance.nowSelectedCharactor != null)
            Destroy(instance.nowSelectedCharactor);

        foreach (Transform obj in GameObject.Find(basePositionsPath).GetComponent<Transform>())
        {
            int index = obj.transform.GetSiblingIndex();
            Vector2 pos = new Vector2(index / COUNT_BASE_POS % COUNT_BASE_POS, index % COUNT_BASE_POS);
            if (target.ConvertObjectToVector(target.gameObject) == pos)
            {
                instance.nowSelectedCharactor = Instantiate(Resources.Load("TargetCursolEffect"), new Vector3(-100, -100, 0), Quaternion.identity) as GameObject;
                instance.nowSelectedCharactor.transform.SetParent(basePositions[index / COUNT_BASE_POS % COUNT_BASE_POS, index % COUNT_BASE_POS].transform);
            }
        }
    }

    //Braverのターンにする+初期化
    public IEnumerator ChangeTurnBraver(float time = CHANGE_TURN_PLAYERS)
    {
        instance.whoseTurn = WhoseTurn.braver;
        if (instance.braver == null)
        {
            ChangeTurnNext();
            yield break;
        }
        OnReadyDetails();

        yield return new WaitForSeconds(time);
        OnCommandBack(null);
        instance.whatCommand = WhatCommand.none;
        instance.subArrow.SetButtons(instance.braver.attackButtons);
        instance.mainOtherCommand.SetActive(true);
        instance.mainPlayerCommand.SetActive(false);
        instance.whoseNameTextOther.text = instance.braver.objectName;
        instance.whoseNameTextPlayer.text = instance.braver.objectName;
        instance.braver.SetOnClickAttack();
        instance.braver.SetOnClickIdle();
        instance.braver.isCommandPushed = false;
        instance.mainOtherArrow.StartSelect();
        //instance.mainPlayerArrow.StopSelect();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        GenerateCursor(instance.braver);
        //Destroy(nowSelectedCharactor);
        //nowSelectedCharactor = Instantiate(Resources.Load("SelectedCharactor"), instance.braver.gameObject.transform) as GameObject;
        //nowSelectedCharactor.transform.parent = instance.braver.gameObject.transform;
    }

    //Princesのターンにする+初期化
    public IEnumerator ChangeTurnPrincess(float time = CHANGE_TURN_PLAYERS)
    {
        instance.whoseTurn = WhoseTurn.princess;
        if (instance.princess == null)
        {
            ChangeTurnNext();
            yield break;
        }
        OnReadyDetails();

        yield return new WaitForSeconds(time);
        OnCommandBack(null);
        instance.whatCommand = WhatCommand.none;
        instance.subArrow.SetButtons(instance.princess.attackButtons);
        instance.mainOtherCommand.SetActive(true);
        instance.mainPlayerCommand.SetActive(false);
        instance.whoseNameTextOther.text = instance.princess.objectName;
        instance.whoseNameTextPlayer.text = instance.braver.objectName;
        instance.princess.SetOnClickAttack();
        instance.princess.SetOnClickIdle();
        instance.princess.isCommandPushed = false;
        instance.mainOtherArrow.StartSelect();
        //instance.mainPlayerArrow.StopSelect();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        GenerateCursor(instance.princess);
        //Destroy(nowSelectedCharactor);
        //nowSelectedCharactor = Instantiate(Resources.Load("SelectedCharactor"), instance.princess.gameObject.transform) as GameObject;
        //nowSelectedCharactor.transform.parent = instance.princess.gameObject.transform;
    }

    //Playerのターンにする+初期化
    public IEnumerator ChangeTurnPlayer(float time = CHANGE_TURN_PLAYERS)
    {
        instance.whoseTurn = WhoseTurn.player;
        if (instance.player == null)
        {
            ChangeTurnNext();
            yield break;
        }
        OnReadyDetails();

        yield return new WaitForSeconds(time);
        OnCommandBack(null);
        instance.whatCommand = WhatCommand.none;
        instance.subArrow.SetButtons(instance.player.attackButtons);
        instance.mainOtherCommand.SetActive(false);
        instance.mainPlayerCommand.SetActive(true);
        instance.whoseNameTextOther.text = instance.player.objectName;
        instance.whoseNameTextPlayer.text = instance.player.objectName;
        instance.player.SetOnClickAttack();
        instance.player.SetOnClickIdle();
        instance.player.isCommandPushed = false;
        instance.mainOtherArrow.StopSelect();
        instance.mainPlayerArrow.StartSelect();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        GenerateCursor(instance.player);
        //Destroy(nowSelectedCharactor);
        //nowSelectedCharactor = Instantiate(Resources.Load("SelectedCharactor"), instance.player.gameObject.transform) as GameObject;
        //nowSelectedCharactor.transform.parent = instance.player.gameObject.transform;
    }

    //Enemyのターンにする+初期化
    public IEnumerator ChangeTurnEnemy(float time = CHANGE_TURN_WAIT_TIME)
    {
        instance.whoseTurn = WhoseTurn.enemy;
        OnReadyDetails();

        //今どのキャラを操作しているかを表示する矢印, 最初に初期化
        //Destroy(nowSelectedCharactor);

        //デリゲートのスタックを吐き出す
        yield return new WaitForSeconds(CHANGE_TURN_PLAYERS);
        OnReadyDetails();
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
                StartCoroutine(ChangeTurnBraver());
                break;

            case WhoseTurn.player:
                StartCoroutine(ChangeTurnPrincess());
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
                StartCoroutine(ChangeTurnPrincess());
                break;

            case WhoseTurn.princess:
                StartCoroutine(ChangeTurnPlayer());
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
            //GenerateCursor(enemies[instance.countEnemyTurn].GetComponent<BattleEnemy>());
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

        if(GetWhoseTurn() == WhoseTurn.player)
            instance.mainPlayerArrow.StartSelect();
        else
            instance.mainOtherArrow.StartSelect();

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
        Judge(winImage, "全ての敵を倒しました");
        if (Loader.level == 3)
            Loader.isUseEscape[Loader.level] = true;
        else if (Loader.level == 4)
            FadeSceneManager.Execute("Title", 3f, 0.1f);
        else
            Loader.level--;
    }

    public void Lose()
    {
        print("You Lose");
        Judge(loseImage, "誰か一人でも倒されると負け");
        if (Loader.level == 3 || Loader.level == 4)
            Loader.isUseEscape[Loader.level] = true;
        Loader.level--;
    }

    private void Judge(Sprite sprite, string message)
    {
        instance.isGameEnd = true;

        Image judge = GameObject.Find("Canvas/JudgePanel").GetComponent<Image>();
        judge.enabled = true;
        judge.sprite = sprite;
        AddMessage(message);
        FlashingManager.Execute(judge, FlashingManager.Hash("minAlpha", 0.3f, "infinite", true));

        instance.mainOtherArrow.StopSelect();
        instance.mainPlayerArrow.StopSelect();
        instance.subArrow.StopSelect();
        instance.commandPanel.SetActive(false);
    }

    /********************
        * ボタン関連*
     ********************/

    //メインコマンド決定
    private void OnReady()
    {
        if (instance.OnCommandPushed())
        {
            if (GetWhoseTurn() == WhoseTurn.player)
                instance.mainPlayerArrow.StopSelect();
            else
                instance.mainOtherArrow.StopSelect();
        }
    }

    //準備ができているなら
    public void OnReadyDetails()
    {
        instance.mainOtherCommand.SetActive(false);
        instance.mainPlayerCommand.SetActive(false);

        instance.whatCommand = WhatCommand.none;
        instance.isPushed = false;
        instance.nowDetailCommand.SetActive(false);
        //if (GetWhoseTurn() == WhoseTurn.player)
        //    instance.mainPlayerArrow.StartSelect();
        //else
        //    instance.mainOtherArrow.StartSelect();
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

    public void OnTool()
    {
        instance.AddMessage("なにも持っていません");
        instance.soundBox.PlayOneShot(instance.audioClass.notExecute, 1f);

        //instance.nowDetailCommand = instance.toolCommand;
        //instance.whatCommand = WhatCommand.tool;

        //Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        //instance.subArrow.SetButtons(buttons);

        //OnReady();
    }

    public void OnIdle()
    {
        instance.nowDetailCommand = instance.idleCommand;
        instance.whatCommand = WhatCommand.idle;

        Button[] buttons = instance.nowDetailCommand.GetComponentsInChildren<Button>();
        instance.subArrow.SetButtons(buttons, new Vector3(-140, 0, 0));

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
        instance.subArrow.SetButtons(buttons, new Vector3(-140, 0, 0));

        OnReady();
    }

    public void Escape()
    {
        if (Loader.level != 4)
        {
            FadeSceneManager.Execute(Loader.boardSceneName);
            instance.soundBox.PlayOneShot(instance.audioClass.escape, 1f);
            Loader.isForceEvent = false;
            Loader.isUseEscape[Loader.level] = true;
            Loader.level--;
        }
        else
        {
            instance.AddMessage("逃げたら即殺されそうだ");
            instance.soundBox.PlayOneShot(instance.audioClass.notExecute, 1f);
        }
    }
}
