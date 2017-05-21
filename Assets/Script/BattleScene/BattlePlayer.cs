using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattlePlayer : CommonBattleChar
{
    private Vector2 defaultPlayerPos = new Vector2(0, 2);
    public Vector2 defaultOffset = new Vector2(0, 15);
    //public Button[] buttonsObject;
    //public string[] buttonsText;
    

    new void Start()
    {
        HP = 10;
        attack = 0;
        BattleManager.instance.AddGridPos(gameObject, defaultPlayerPos);
        base.Start();
    }
    
    void Update()
    {
        
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnMoveAttackVertical, OnTool, OnEscape };
        SetMethod(method);
    }

    /*以下ボタン関数*/

    public void OnMoveAttackVertical()
    {
        base.OnMoveAttackVertical(gameObject, defaultOffset);
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
