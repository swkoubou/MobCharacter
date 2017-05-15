using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBraver : BattleAI
{
    

    new void Start()
    {
        HP = 30;
        attack = 3;
        base.Start();
    }


    void Update()
    {
        BraverTurn();
    }

    protected override void SwitchCommand(int rand)
    {
        switch (rand)
        {
            case 0:
                break;

            default:
                if (!FindObjectOfType<iTween>())
                    iTween.ShakePosition(enemies[0].gameObject, iTween.Hash("x", 10f, "time", 0.5f));
                break;
        }
        print("Braver");
        StartCoroutine(BattleManager.instance.ChangeTurnEnemy());
    }
}
