using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePrincess : CommonBattleChar
{
    private Vector2 defaultPrincessPos = new Vector2(2, 2);
    public Vector2 defaultOffset = new Vector2(0, 15);
    public Button[] commandButtons;


    new void Start()
    {
        HP = 30;
        attack = 2;
        //BattleManager.instance.AddGridPos(gameObject, defaultPrincessPos);
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
