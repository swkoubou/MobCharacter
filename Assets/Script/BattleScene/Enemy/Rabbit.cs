using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : BattleEnemy
{
    new void Start()
    {
        HP = 10;
        attack = 2;
        defaultOffset = new Vector2(0.2f, 1f);
        hpberOffset = new Vector2(-0.2f, -1.5f);
        base.Start();
    }

    void Update()
    {

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
