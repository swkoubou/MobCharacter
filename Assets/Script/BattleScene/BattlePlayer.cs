using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : BattleAI
{
    public SelectArrow mainArrow;
    public SelectArrow subArrow;

    new void Start()
    {
        HP = 10;
        attack = 0;
        base.Start();
    }

    
    void Update()
    {
        
    }

    /*以下ボタン関数*/

    //攻撃コマンド
    public void OnAttack()
    {
        BattleManager.instance.OnCommandPushed();
        mainArrow.StopSelect();
        //BattleManager.instance.subCommand.gameObject.SetActive(true);
        subArrow.StartSelect();
        //BattleManager.instance.subCommand.gameObject.SetActive(false);

        ////実験用
        //Invoke("OnAttackDetails", 2f);
    }

    public void OnAttackDetails()
    {
        if (BattleManager.instance.isPushed)
        {
            StartCoroutine(BattleManager.instance.ChangeTurnBraver());

            BattleManager.instance.OnCommandBaack();
            BattleManager.instance.mainCommand.gameObject.SetActive(false);
        }
    }

    //道具コマンド
    public void OnTool()
    {
        BattleManager.instance.OnCommandPushed();
    }

    public void OnToolDetails()
    {

    }

    public void OnEscape()
    {
        FadeSceneManager.Execute(Loader.battleSceneName);
    }

    //このクラスではなにもしない
    protected override void SwitchCommand(int rand)
    {
        throw new NotImplementedException();
    }
}
