using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEnemy : MovingObject
{
    public int attackDamage;
    private Animator animator;
    private Transform target;
    private bool skipMove;


    protected override void Start()
    {
        HP = 20;
        attack = 5;
        BoardManager.instance.AddEnemy(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }


    void Update()
    {
        CheckHP();
    }

    void CheckHP()
    {
        if (HP <= 0)
        {
            HP = 0;
            BoardBraver braver = BoardManager.instance.braver;
            braver.TargetDied();

            Destroy(gameObject);
        }
    }


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    //敵キャラ移動用メソッド　GameManagerから呼ばれる
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        
        //死亡したときにエラーが出るので
        try
        {
            //同じカラム(x軸)にいる時
            //Mathf.Abs: 絶対値をとる。-1なら1となる。
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            {
                //プレイヤーが上にいれば+1、下に入れば-1する
                yDir = target.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                //プレイヤーが右にいれば+1、左にいれば-1する
                xDir = target.position.x > transform.position.x ? 1 : -1;

                //向きを変える
                Vector3 scale = transform.localScale;
                if (xDir > 0 && transform.localScale.x < 0)
                    scale.x = transform.localScale.x * -1;
                else if (xDir < 0 && transform.localScale.x > 0)
                    scale.x = transform.localScale.x * -1;
                transform.localScale = scale;
            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }

        //ジェネリック機能　攻撃対象はPlayerのみなので、型引数はPlayer
        AttemptMove<BoardPlayer>(xDir, yDir);
    }


    //MovingObjectの抽象メソッドのため必ず必要
    protected override void OnCantMove<T>(T component)
    {
        if (component.GetComponent<BoardPlayer>())
        {
            BoardPlayer other = component as BoardPlayer;
            other.LoseHP(attackDamage);
            iTween.MoveFrom(gameObject, iTween.Hash("position", other.transform.position, "time", 0.5f));
        }
        else if (component.GetComponent<BoardBraver>())
        {
            BoardBraver other = component as BoardBraver;
            other.LoseHP(attackDamage);
        }
    }
}
