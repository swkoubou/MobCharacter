using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardBraver : MovingObject
{
    private Animator animator;
    private MapGenerator mapGenerator;
    private List<GameObject> targets = new List<GameObject>();
    public float searchRange = 10f;
    private bool skipMove;


    void Awake()
    {
        //始めに一度だけnullを入れておく
        targets.Add(null);
    }

    protected override void Start()
    {
        HP = 50;
        attack = 50;
        BoardManager.instance.AddBraver(this);
        animator = GetComponent<Animator>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        base.Start();
    }


    void Update()
    {
        
    }

    private void OnDisable()
    {
        BoardManager.instance.braverHP = HP;
    }


    void SearchEnemy()
    {
        targets.Clear();

        //指定半径の中に居るエネミーを見つけ出し、近い順に配列へ格納
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy")
            .Where(e => Vector2.Distance(e.transform.position, transform.position) < searchRange)
            .OrderBy(e => Vector2.Distance(e.transform.position, transform.position))
            .ToArray();
        
        //上位を確保しておく
        int serchCount = 2;
        if(enemies.Length < serchCount)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                //別のListに確保しておく
                targets.Add(enemies[i]);
            }

            //ターゲットが上位になかったら、空いているところにExitを入れる
            int j = enemies.Length;
            do
            {
                j++;
                targets.Add(mapGenerator.GetExit());
            } while (j <= serchCount);
        }
        else
        {
            for (int i = 0; i < serchCount; i++)
            {
                targets.Add(enemies[i]);
            }
        }

        //for(int i = 0; i < serchCount; i++)
        //{
        //   print(targets[i]);
        //}
    }


    protected override void AttemptMove<T, U>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T, U>(xDir, yDir);
        BoardManager.instance.ChangeTurnEnemy();
        skipMove = true;
    }


    //勇者移動用メソッド　GameManagerから呼ばれる
    public void MoveBraver()
    {
        //まずターゲットを検索
        if (targets[0] == null || targets[0] == mapGenerator.GetExit())
            SearchEnemy();

        int xDir = 0;
        int yDir = 0;

        //同じカラム(x軸)にいる時
        //Mathf.Abs: 絶対値をとる。-1なら1となる。
        if (Mathf.Abs(targets[0].transform.position.x - transform.position.x) < float.Epsilon)
        {
            //エネミーが上にいれば+1、下に入れば-1する
            yDir = targets[0].transform.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            //エネミーが右にいれば+1、左にいれば-1する
            xDir = targets[0].transform.position.x > transform.position.x ? 1 : -1;
        }
        //ジェネリック機能　攻撃対象はEnemyのみなので、型引数はEnemy
        AttemptMove<BoardEnemy, Wall>(xDir, yDir);
    }


    protected override void OnCantMove<T>(T component)
    {
        if(component.GetComponent<BoardEnemy>())
        {
            BoardEnemy other = component as BoardEnemy;
            other.LoseHP(attack);
            print("OnCantMove: Enemy");
        }
        else if (component.GetComponent<Wall>())
        {
            Wall other = component as Wall;

            //WallスクリプトのDamageWallメソッド呼び出し
            other.DamageWall(attack);
            print("OnCantMove: Wall");
        }
    }


    public void TargetDied()
    {
        targets[0] = null;
    }
}
