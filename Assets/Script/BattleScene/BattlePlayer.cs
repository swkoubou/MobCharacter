using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattlePlayer : CommonBattleChar
{    

    new void Start()
    {
        HP = 10;
        attack = 0;
        defaultPos = new Vector2(0, 2);
        defaultOffset = new Vector2(0, 1);
        SetGrid(gameObject, defaultPos);
        base.Start();
    }
    
    void Update()
    {
        
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnMoveAttackVertical, OnMoveAttackSlash, OnEscape };
        SetMethod(method);
    }

    /*以下ボタン関数*/

    public void OnMoveAttackVertical()
    {
        base.OnMoveAttackVertical(gameObject);
    }

    public void OnMoveAttackSlash()
    {
        Vector2 nowPos = ConvertObjectToVector(gameObject);
        base.OnMoveAttackSlash(gameObject, nowPos);
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
