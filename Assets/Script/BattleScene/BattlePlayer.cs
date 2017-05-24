using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattlePlayer : CommonBattleChar
{    

    void Start()
    {
        HP = 10;
        attack = 0;
        defaultPos = new Vector2(0, 2);
        defaultOffset = new Vector2(0, 1);
        SetGrid(gameObject, defaultPos);
    }
    
    void Update()
    {
        
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnAttackMoveVertical, OnAttackMoveSlash, OnEscape };
        SetMethod(method);
    }

    /*以下ボタン関数*/

    public void OnAttackMoveVertical()
    {
        base.OnAttackMoveVertical(gameObject);
    }

    public void OnAttackMoveSlash()
    {
        base.OnAttackMoveSlash(gameObject);
    }

    public void OnEscape()
    {
        FadeSceneManager.Execute(Loader.battleSceneName);
    }
}
