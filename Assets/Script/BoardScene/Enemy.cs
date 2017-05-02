using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int attackDamage;
    private Animator animator;
    private Transform target;
    private bool skipMove;


    protected override void Start()
    {
        BoardManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Braver").transform;
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
            Braver braver = BoardManager.instance.braver;
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
            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }

        //ジェネリック機能　攻撃対象はPlayerのみなので、型引数はPlayer
        AttemptMove<Player>(xDir, yDir);
    }


    //MovingObjectの抽象メソッドのため必ず必要
    protected override void OnCantMove<T>(T component)
    {
        if (component.GetComponent<Player>())
        {
            Player other = component as Player;
            other.LoseHP(attackDamage);
        }
        else if (component.GetComponent<Braver>())
        {
            Braver other = component as Braver;
            other.LoseHP(attackDamage);
        }
    }
}
