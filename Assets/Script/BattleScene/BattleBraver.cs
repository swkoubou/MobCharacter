using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleBraver : CommonBattleChara
{

    new void Start()
    {
        HP = 30;
        attack = 5;
        defaultPos = new Vector2(1, 2);
        defaultOffset = new Vector2(0, 1);
        //SetGrid(gameObject, defaultPos);
        base.Start();
    }


    void Update()
    {
        
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnNormalAttack, OnAttackMoveVertical, OnAttackMoveSlash };
        SetMethod(method);
    }

    /*以下ボタン関数*/
    public new void OnNormalAttack()
    {
        base.OnNormalAttack();
    }

    public new void OnAttackMoveVertical()
    {
        base.OnAttackMoveVertical();
    }

    public new void OnAttackMoveSlash()
    {
        base.OnAttackMoveSlash();
    }
}
