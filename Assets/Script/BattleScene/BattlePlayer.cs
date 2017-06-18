using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattlePlayer : CommonBattleChara
{    

    new void Start()
    {
        HP = 20;
        attack = 3;
        defaultPos = new Vector2(1, 0);
        defaultOffset = new Vector2(0, 1);
        hpberOffset = new Vector2(0, -1.32f);

        attackText = new string[3] {"通常攻撃", "バーチカルアタック", "スラッシュスラント"};
        idleText = new string[1] { "昼寝をする" };
        base.Start();
    }
    
    void Update()
    {
        
    }

    public void SetOnClickAttack()
    {
        UnityAction[] method = new UnityAction[] { OnNormalAttack, OnAttackMoveVertical, OnAttackMoveSlash};
        SetMethodAttack(method);
    }

    public void SetOnClickIdle()
    {
        UnityAction[] method = new UnityAction[] { OnIdle };
        SetMethodIdle(method);
    }

    /*以下ボタン関数*/

    public void OnNormalAttack()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //攻撃し、その場に留まるので
        movedPos.y = 1;

        if (ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        BattleManager.instance.stackCommandPlayer = new BattleManager.StackCommandPlayer(NormalAttack);
        BattleManager.instance.ChangeTurnNext();
    }

    private void NormalAttack()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.y = 1;

        BattleManager.instance.OnReadyDetails();
        var moveHash = new Hashtable();
        //gridPOsiiotnsだとnullのときエラーが出るので
        moveHash.Add("x", BattleManager.instance.basePositions[(int)movedPos.x, (int)movedPos.y].transform.position.x);
        moveHash.Add("time", charMoveTime);
        iTween.MoveFrom(gameObject, moveHash);

        AnimStart(null, audioClass.normalAttack);
        BattleManager.instance.AddMessage(objectName + "の通常攻撃!");
        //Invoke("DelayChange", BattleManager.instance.changeTurnWaitTime);
        ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);

    }

    public void OnAttackMoveVertical()
    {
        BattleManager.instance.stackCommandPlayer = new BattleManager.StackCommandPlayer(AttackMoveVertical);
        BattleManager.instance.ChangeTurnNext();
    }

    private void AttackMoveVertical()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //向かい側に移動するのでyだけ動かす
        if (movedPos.y == 0)
            movedPos.y = 2;
        else if (movedPos.y == 2)
            movedPos.y = 0;

        if (CanChangeGrid(movedPos))
        {
            BattleManager.instance.OnReadyDetails();

            Vector2 target = movedPos;
            target.y = 1;

            if(ConvertVectorToObject(movedPos) != null && ConvertVectorToObject(movedPos).tag == "Player")
                ConvertVectorToObject(target).GetComponent<CommonBattleChara>().DamagedAnim(attack*3);
            else
                ConvertVectorToObject(target).GetComponent<CommonBattleChara>().DamagedAnim(attack);

            ChangeGrid(gameObject, movedPos);
            MoveGrid(gameObject, movedPos);
            AnimStart(null, audioClass.normalAttack);
            BattleManager.instance.AddMessage(objectName + "の移動攻撃!");
        }
        else
        {
            print(messageList.nonMove);
        }
    }

    public void OnAttackMoveSlash()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //対角に移動するので0と2を反転
        if (movedPos.x == 0)
            movedPos.x = 2;
        else if (movedPos.x == 2)
            movedPos.x = 0;
        else
            movedPos.x = -1;

        if (movedPos.y == 0)
            movedPos.y = 2;
        else if (movedPos.y == 2)
            movedPos.y = 0;
        else
            movedPos.y = -1;

        //対角に居ない場合は実行しない
        if (movedPos.x == -1 || movedPos.y == -1)
        {
            BattleManager.instance.AddMessage(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        BattleManager.instance.stackCommandPlayer = new BattleManager.StackCommandPlayer(AttackMoveSlash);
        BattleManager.instance.ChangeTurnNext();
    }

    protected void AttackMoveSlash()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        
        //対角に移動するので0と2を反転
        if (movedPos.x == 0)
            movedPos.x = 2;
        else if (movedPos.x == 2)
            movedPos.x = 0;

        if (movedPos.y == 0)
            movedPos.y = 2;
        else if (movedPos.y == 2)
            movedPos.y = 0;

        if (CanChangeGrid(movedPos))
        {
            BattleManager.instance.OnReadyDetails();

            Vector2 target = new Vector2(1, 1);
            if (ConvertVectorToObject(movedPos) != null && ConvertVectorToObject(movedPos).tag == "Player")
                ConvertVectorToObject(target).GetComponent<CommonBattleChara>().DamagedAnim(attack * 4);
            else
                ConvertVectorToObject(target).GetComponent<CommonBattleChara>().DamagedAnim(attack);

            ChangeGrid(gameObject, movedPos);
            MoveGrid(gameObject, movedPos);
            AnimStart(null, audioClass.normalAttack);
            BattleManager.instance.AddMessage(objectName + "のスラッシュ攻撃!");
        }
        else
        {
            print(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
        }
    }

    public void OnIdle()
    {
        BattleManager.instance.stackCommandPlayer = new BattleManager.StackCommandPlayer(Idle);
        BattleManager.instance.ChangeTurnNext();
    }

    private void Idle()
    {
        BattleManager.instance.OnReadyDetails();
        BattleManager.instance.AddMessage(objectName + "は昼寝をした");
    }

    public void OnEscape()
    {
        FadeSceneManager.Execute(Loader.boardSceneName);
        soundBox.PlayOneShot(audioClass.escape, 1f);
    }
}
