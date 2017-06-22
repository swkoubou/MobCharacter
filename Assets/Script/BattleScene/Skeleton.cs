using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : BattleEnemy
{


    new void Start()
    {
        HP = 10;
        attack = 5;
        defaultOffset = new Vector2(0, 1.5f);
        hpberOffset = new Vector2(0, -1.8f);
        base.Start();
    }

    protected override void SwitchCommand(GameObject target, int rand)
    {
        switch (rand)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                OnEnemyNormalAttack(target);
                break;

            default:
                OnEnemyNormalAttack(target);
                break;
        }


    }
}
