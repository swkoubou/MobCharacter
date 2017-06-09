using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePrincess : CommonBattleChara
{

    new void Start()
    {
        HP = 30;
        attack = 4;
        defaultPos = new Vector2(2, 2);
        defaultOffset = new Vector2(0, 1);
        hpberOffset = new Vector2(0, -1.32f);
        //SetGrid(gameObject, defaultPos);
        base.Start();
    }

    void Update()
    {
        //if(BattleManager.instance.GetWhoseTurn() == BattleManager.WhoseTurn.princess)
        //{
        //    StartCoroutine(BattleManager.instance.ChangeTurnPlayer());
        //}
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
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        OnOnlyAnim(controller[0], audioClass.superFrame, "の獄炎!");
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
