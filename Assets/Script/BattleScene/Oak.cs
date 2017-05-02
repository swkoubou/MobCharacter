using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oak : BattleEnemy
{


    void Start()
    {

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
                    iTween.MoveFrom(gameObject, iTween.Hash("y", -2f, "time", 1f));
                break;
        }
        StartCoroutine(BattleManager.instance.ChangeTurnPlayer());
    }
}
