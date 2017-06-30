using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//MovingObjectクラスを継承する
public class BoardPlayer : MovingObject
{

    public int wallDamage = 1; //壁へのダメージ量
    public int pointsPerFood = 10; //フードの回復量
    public int pointsPerSoda = 20; //ソーダの回復量
    public float restartlevelDelay = 0.5f; //次レベルへ行く時の時間差

    private Animator animator; //PlayerChop, PlayerHit用

    //MovingObjectのStartメソッドを継承　baseで呼び出し
    protected override void Start()
    {
        HP = 10;
        attack = 5;

        //Animatorをキャッシュしておく
        animator = GetComponent<Animator>();

        //MovingObjectのStartメソッド呼び出し
        base.Start();
    }


    //Playerスクリプトが無効になる前に、体力をGameManagerへ保存
    //UnityのAPIメソッド(Unityに標準で用意された機能)
    private void OnDisable()
    {
        BoardManager.instance.playerHP = HP;
    }


    void Update()
    {
        bool isKeyUp = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow);

        if (isKeyUp)
            animator.SetTrigger("Wait");

        //Playerの順番かつPlayerが動き終わっているとき実行する
        if (BoardManager.instance.GetWhoseTurn() == BoardManager.WhoseTurn.player && !isMoving)
        {
            //Enterボタンを押すとPlayerはその場から動かず、ターンをスキップする
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //BoardManager.instance.ChangeTurnBraver();
                BoardManager.instance.ChangeTurnEnemy();
            }
            else
            {
                int horizontal = 0; //-1: 左移動, 1: 右移動
                int vertical = 0; //-1: 下移動, 1: 上移動

                horizontal = (int)Input.GetAxisRaw("Horizontal");
                vertical = (int)Input.GetAxisRaw("Vertical");

                //上下もしくは左右に移動を制限
                if (horizontal != 0)
                {
                    vertical = 0;
                }
                else if (vertical != 0)
                {
                    horizontal = 0;
                }

                //上下左右どれかに移動する時
                if (horizontal != 0 || vertical != 0)
                {
                    //Wall: ジェネリックパラメーター<T>に渡す型引数
                    //Playerの場合はWall以外判定する必要はない
                    AttemptMove<Wall>(horizontal, vertical);
                }
            }
        }
    }


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //MovingObjectのAttemptMove呼び出し
        base.AttemptMove<T>(xDir, yDir);

        string animStr = null;
        if (yDir == 1)
            animStr = "Up";
        else if (xDir == 1)
            animStr = "Right";
        else if (yDir == -1)
            animStr = "Down";
        else if (xDir == -1)
            animStr = "Left";

        animator.SetTrigger(animStr);

        //壁向かって移動できないときはターン遷移しない
        //if (isMoving)
        BoardManager.instance.ChangeTurnEnemy();
    }


    //MovingObjectの抽象メソッドのため必ず必要
    protected override void OnCantMove<T>(T component)
    {
        if (component.GetComponent<Wall>())
        {
            Wall other = component as Wall;

            //WallスクリプトのDamageWallメソッド呼び出し
            other.DamageWall(attack);
            //Wallに攻撃するアニメーションを実行
            //animator.SetTrigger("PlayerChop"); 
            BoardManager.instance.DamagedAnim(other.GetComponent<SpriteRenderer>());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Event")
        {
            FlashingManager.Execute(other.gameObject.GetComponent<SpriteRenderer>());
            BoardManager.instance.AddMessage("ドアを開けた");
            BoardManager.instance.soundBox.PlayOneShot(audioClass.openDoor, 1f);
            StartCoroutine(Delay(other.gameObject, 1f));
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            //Invoke: 引数分遅れてメソッドを実行する
            Invoke("Restart", restartlevelDelay);
            enabled = false; //Playerを無効にする
            Loader.boardPlayerPos = new Vector2(-1, -1);
        }
        else if (other.tag == "Item")
        {
            FlashingManager.Execute(other.gameObject.GetComponent<SpriteRenderer>());
            BoardManager.instance.AddMessage("宝箱を開けた");
            BoardManager.instance.soundBox.PlayOneShot(audioClass.getItem, 1f);
            StartCoroutine(Delay(other.gameObject, 1f));
        }
        else if (other.gameObject.tag == "Event")
        {
            FlashingManager.Execute(other.gameObject.GetComponent<SpriteRenderer>());
            BoardManager.instance.AddMessage("ドアを開けた");
            BoardManager.instance.soundBox.PlayOneShot(audioClass.openDoor, 1f);
            StartCoroutine(Delay(other.gameObject, 1f));
        }
    }

    IEnumerator Delay(GameObject other, float time)
    {
        yield return new WaitForSeconds(time);
        other.SetActive(false);
    }


    private void Restart()
    {
        FadeSceneManager.Execute(SceneManager.GetActiveScene().name);
    }


    //敵キャラがプレイヤーを攻撃した時のメソッド
    public override void LoseHP(int dmg)
    {
        BoardManager.instance.AddMessage("敵に遭遇した");
        Invoke("BattleScene", restartlevelDelay);
        BoardManager.instance.soundBox.PlayOneShot(audioClass.normalAttack, 1f);
        base.LoseHP(dmg);
        Loader.boardPlayerPos = transform.position;
        enabled = false;
    }

    private void BattleScene()
    {
        FadeSceneManager.Execute(Loader.battleSceneName);
    }
}
