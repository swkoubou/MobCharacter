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

        attackText = new string[3] { "獄炎", "氷花", "mogero" };
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
        UnityAction[] method = new UnityAction[] { OnSuperFlame, OnFreezeAce, OnAttackMoveSlash };
        SetMethod(method);
    }

    /*以下ボタン関数*/
    public void OnSuperFlame()
    {
        BattleManager.instance.stackCommandPrincess = new BattleManager.StackCommandPrincess(SuperFlame);
        BattleManager.instance.ChangeTurnNext();
        //Vector2 movedPos = ConvertObjectToVector(gameObject);
        //movedPos.y = 1;

        //if (ConvertVectorToObject(movedPos) == null)
        //{
        //    BattleManager.instance.AddMessage(messageList.nonTarget);
        //    soundBox.PlayOneShot(audioClass.notExecute, 1f);
        //    return;
        //}

        //effecter.transform.position = ConvertVectorToObject(movedPos).transform.position;
        //OnOnlyAnim(controller[0], audioClass.superFlame, "の"+attackText[0] +"!");
        //ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
    }

    public void SuperFlame()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        effecter.transform.position = ConvertVectorToObject(movedPos).transform.position;
        OnOnlyAnim(controller[0], audioClass.superFlame, "の" + attackText[0] + "!");
        ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
    }

    public void OnFreezeAce()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        effecter.transform.position = ConvertVectorToObject(movedPos).transform.position;
        OnOnlyAnim(controller[1], audioClass.fleezeAce, "の" + attackText[1] + "!");
        ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
    }

    public new void OnAttackMoveSlash()
    {
        base.OnAttackMoveSlash();
    }
}
