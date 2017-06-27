using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : BattleEnemy
{
    new void Start()
    {
        HP = 15;
        attack = 3;
        defaultOffset = new Vector2(0, 1.5f);
        hpberOffset = new Vector2(0, -1.8f);
        base.Start();
    }

    protected override void SwitchCommand(int rand)
    {
        switch (rand)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                OnEnemyNormalAttack();
                break;

            default:
                OnEnemyNormalAttack();
                break;
        }
    }
}
