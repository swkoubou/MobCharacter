using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BattleAI : MonoBehaviour
{
    protected BattlePlayer player;
    protected BattleBraver braver;
    protected GameObject[] enemies;
    protected int HP;
    protected int attack;

    //一度だけupdate関数内で使いたいので
    //protected bool isBraverOnce;
    //protected bool isEnemyOnce;
    protected bool isOnce;


    protected void Start()
    {
        player = FindObjectOfType<BattlePlayer>();
        braver = FindObjectOfType<BattleBraver>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = enemies.OrderBy(e => Vector2.Distance(e.transform.position, transform.position)).ToArray();
        //isBraverOnce = false;
        //isEnemyOnce = false;
        isOnce = false;
    }

    protected void BraverTurn()
    {
        //Braverターンじゃないなら
        if (BattleManager.instance.GetTurn() != BattleManager.WhoseTurn.braver)
        {
            isOnce = false;
            return;
        }

        int rand = Random.Range(0, 10);
        if (!isOnce)
        {
            isOnce = true;
            SwitchCommand(rand);
        }
    }

    protected void EnemyTurn()
    {
        //Enemyターンじゃないなら
        if (BattleManager.instance.GetTurn() != BattleManager.WhoseTurn.enemy)
        {
            isOnce = false;
            return;
        }

        int rand = Random.Range(0, 10);
        if (!isOnce)
        {
            isOnce = true;
            SwitchCommand(rand);
        }
    }

    public void LoseHP(int dmg)
    {
        HP -= dmg;
    }

    protected abstract void SwitchCommand(int rand);
}
