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
    private Text message;


    //MovingObjectのStartメソッドを継承　baseで呼び出し
    protected override void Start()
    {
        HP = 10;
        attack = 5;

        //Animatorをキャッシュしておく
        animator = GetComponent<Animator>();
        message = GameObject.Find("UI/Message").GetComponent<Text>();

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
        //Playerの順番かつPlayerが動き終わっているとき実行する
        if (BoardManager.instance.GetWhoseTurn() == BoardManager.WhoseTurn.player && !isMoving)
        {
            //Enterボタンを押すとPlayerはその場から動かず、ターンをスキップする
            if (Input.GetKeyDown(KeyCode.Return))
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            //Invoke: 引数分遅れてメソッドを実行する
            Invoke("Restart", restartlevelDelay);
            enabled = false; //Playerを無効にする
        }
        else if(other.tag == "Item")
        {
            other.gameObject.SetActive(false);
        }
        //else if (other.tag == "Food")
        //{
        //    //体力を回復しotherオブジェクトを削除
        //    other.gameObject.SetActive(false);
        //}
        //else if (other.tag == "Soda")
        //{
        //    //体力を回復しotherオブジェクトを削除
        //    other.gameObject.SetActive(false);
        //}
    }


    private void Restart()
    {
        FadeSceneManager.Execute(SceneManager.GetActiveScene().name);
    }


    //敵キャラがプレイヤーを攻撃した時のメソッド
    public override void LoseHP(int dmg)
    {
        enabled = false;
        message.enabled = true;
        Invoke("BattleScene", restartlevelDelay);
        base.LoseHP(dmg);
    }

    private void BattleScene()
    {
        FadeSceneManager.Execute(Loader.battleSceneName);
    }
}
