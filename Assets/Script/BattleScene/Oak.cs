using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oak : BattleAI
{


    new void Start()
    {
        HP = 10;
        attack = 1;
        base.Start();
    }


    void Update()
    {
        EnemyTurn();
    }


    protected override void SwitchCommand(int rand)
    {
        switch (rand)
        {
            case 0:
                break;

            default:
                if (!FindObjectOfType<iTween>())
                    iTween.MoveFrom(gameObject, iTween.Hash("y", transform.position.y - 100f, "time", 0.5f));
                break;
        }
        print("Oak");
        StartCoroutine(BattleManager.instance.ChangeTurnPlayer());
    }
}
