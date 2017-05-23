using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePrincess : CommonBattleChar
{

    new void Start()
    {
        HP = 30;
        attack = 2;
        defaultPos = new Vector2(2, 0);
        defaultOffset = new Vector2(0, 1);
        SetGrid(gameObject, defaultPos);
        base.Start();
    }

    void Update()
    {

    }

    protected override void SwitchCommand(int rand)
    {
        throw new NotImplementedException();
    }
}
