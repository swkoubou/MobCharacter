using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleBraver : CommonBattleChar
{

    new void Start()
    {
        HP = 30;
        attack = 5;
        defaultPos = new Vector2(1, 2);
        defaultOffset = new Vector2(0, 1);
        AddGrid(gameObject, defaultPos);
        MoveGrid(gameObject, defaultPos);
        base.Start();
    }


    void Update()
    {
        BraverTurn();
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnMoveAttackVertical};
        SetMethod(method);
    }

    /*以下ボタン関数*/
    void OnMoveAttackVertical()
    {

    }



    //このクラスではなにもしない
    protected override void SwitchCommand(int rand)
    {
        StartCoroutine(BattleManager.instance.ChangeTurnPlayer());
        //throw new NotImplementedException();
    }
}
