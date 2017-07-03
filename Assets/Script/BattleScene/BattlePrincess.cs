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
        defaultOffset = new Vector2(0, 1);
        hpberOffset = new Vector2(0, -1.32f);

        attackText = new string[3] { "獄炎", "氷花", "竜巻" };
        idleText = new string[1] { "お化粧を整える" };
        base.Start();
    }

    void Update()
    {
        //if(BattleManager.instance.GetWhoseTurn() == BattleManager.WhoseTurn.princess)
        //{
        //    StartCoroutine(BattleManager.instance.ChangeTurnPlayer());
        //}
    }

    public void SetOnClickAttack()
    {
        UnityAction[] method = new UnityAction[] { OnSuperFlame, OnFreezeAce, OnWindStorm };
        SetMethodAttack(method);
    }

    public void SetOnClickIdle()
    {
        UnityAction[] method = new UnityAction[] { OnIdle };
        SetMethodIdle(method);
    }

    /*以下ボタン関数*/
    public void OnSuperFlame()
    {
        if (isCommandPushed)
            return;

        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        isCommandPushed = true;
        BattleManager.instance.stackCommandPrincess = new BattleManager.StackCommandPrincess(SuperFlame);
        BattleManager.instance.ChangeTurnNext();
    }

    private void SuperFlame()
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
        OnlyAnim(controller[0], audioClass.superFlame, objectName + "の" + attackText[0] + "!");
        if (ConvertVectorToObject(movedPos) != null)
        {
            ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
        }
    }

    public void OnFreezeAce()
    {
        if (isCommandPushed)
            return;

        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += -1;
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null || movedPos.x < 0)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        isCommandPushed = true;
        BattleManager.instance.stackCommandPrincess = new BattleManager.StackCommandPrincess(FreezeAce);
        BattleManager.instance.soundBox.PlayOneShot(audioClass.decide, 1f);
        BattleManager.instance.ChangeTurnNext();
    }

    private void FreezeAce()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += -1;
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null || movedPos.x < 0)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        effecter.transform.position = ConvertVectorToObject(movedPos).transform.position;
        OnlyAnim(controller[1], audioClass.fleezeAce, objectName + "の" + attackText[1] + "!");
        if (ConvertVectorToObject(movedPos) != null)
        {
            ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
        }
    }

    public void OnWindStorm()
    {
        if (isCommandPushed)
            return;

        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += 1;
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null || movedPos.x > 2)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        isCommandPushed = true;
        BattleManager.instance.stackCommandPrincess = new BattleManager.StackCommandPrincess(WindStorm);
        BattleManager.instance.soundBox.PlayOneShot(audioClass.decide, 1f);
        BattleManager.instance.ChangeTurnNext();
    }

    private void WindStorm()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += 1;
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null || movedPos.x > 2)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        effecter.transform.position = ConvertVectorToObject(movedPos).transform.position;
        OnlyAnim(controller[2], audioClass.windStorm, objectName + "の" + attackText[2] + "!");
        if (ConvertVectorToObject(movedPos) != null)
        {
            ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
        }
    }

    public void OnIdle()
    {
        if (isCommandPushed)
            return;

        isCommandPushed = true;
        BattleManager.instance.stackCommandPrincess = new BattleManager.StackCommandPrincess(Idle);
        BattleManager.instance.soundBox.PlayOneShot(audioClass.decide, 1f);
        BattleManager.instance.ChangeTurnNext();
    }

    private void Idle()
    {
        //BattleManager.instance.OnReadyDetails();
        BattleManager.instance.AddMessage(objectName + "はお化粧を整えた");
    }
}
