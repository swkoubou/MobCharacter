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
        defaultPos = new Vector2(0, 2);
        defaultOffset = new Vector2(0, 1);
        hpberOffset = new Vector2(0, -1.32f);

        attackText = new string[3] { "破滅光線", "二重撃", "mogero" };
        idleText = new string[1] { "素振りをする" };
        base.Start();
    }


    void Update()
    {

    }

    public void SetOnClickAttack()
    {
        UnityAction[] method = new UnityAction[] { OnHyperRay, OnDoubleSlash, OnAttackMoveSlash };
        SetMethodAttack(method);
    }

    public void SetOnClickIdle()
    {
        UnityAction[] method = new UnityAction[] { OnIdle };
        SetMethodIdle(method);
    }

    /*以下ボタン関数*/
    public void OnHyperRay()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        BattleManager.instance.stackCommandBraver = new BattleManager.StackCommandBraver(HyperRay);
        BattleManager.instance.ChangeTurnNext();
    }

    public void HyperRay()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        OnOnlyAnim(controller[0], audioClass.hyperRay, "の" + attackText[0] + "!");
        ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
    }

    public void OnDoubleSlash()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        BattleManager.instance.stackCommandBraver = new BattleManager.StackCommandBraver(DoubleSlash);
        BattleManager.instance.ChangeTurnNext();
    }

    public void DoubleSlash()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        effecter.transform.position = ConvertVectorToObject(movedPos).transform.position;
        OnOnlyAnim(controller[1], audioClass.doubleSlash, "の" + attackText[1] + "!");
        ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(Mathf.FloorToInt(attack * 1.5f));
    }

    public void OnAttackMoveSlash()
    {
        
    }

    public void OnIdle()
    {
        BattleManager.instance.stackCommandBraver = new BattleManager.StackCommandBraver(Idle);
        BattleManager.instance.ChangeTurnNext();
    }

    private void Idle()
    {
        BattleManager.instance.OnReadyDetails();
        BattleManager.instance.AddMessage(objectName + "は素振りを始めた");
    }
}
