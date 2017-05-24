using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEnemy : CommonBattleChar
{
    //一度だけupdate関数内で使いたいので
    protected bool isOnce;

    protected void Start()
    {
        isOnce = false;
    }

    void Update()
    {

    }

    protected void EnemyTurn()
    {
        //Enemyターンじゃないなら
        if (BattleManager.instance.GetWhoseTurn() != BattleManager.WhoseTurn.enemy)
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

    //EnemyのAI用
    protected abstract void SwitchCommand(int rand);
}
