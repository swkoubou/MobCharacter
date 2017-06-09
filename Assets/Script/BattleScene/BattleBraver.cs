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
        HP = 50;
        attack = 5;
        defaultPos = new Vector2(1, 2);
        defaultOffset = new Vector2(0, 1);
        hpberOffset = new Vector2(0, -1.32f);
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
    public void OnNormalAttack()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage("攻撃対象がいません");
            return;
        }

        OnOnlyAnim(controller[0], null, "の破滅光線!");
        ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().LoseHP(attack);
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
