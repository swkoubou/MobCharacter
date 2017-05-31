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
        HP = 10;
        attack = 0;
        defaultPos = new Vector2(0, 2);
        defaultOffset = new Vector2(0, 1);
        //SetGrid(gameObject, defaultPos);
        base.Start();
    }
    
    void Update()
    {
        
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnNormalAttack, OnAttackMoveVertical, OnAttackMoveSlash};
        SetMethod(method);
    }

    /*以下ボタン関数*/

    public void OnNormalAttack()
    {
        base.OnNormalAttack(controller[0]);
    }

    public new void OnAttackMoveVertical()
    {
        base.OnAttackMoveVertical();
    }

    public new void OnAttackMoveSlash()
    {
        base.OnAttackMoveSlash();
    }

    public void OnEscape()
    {
        FadeSceneManager.Execute(Loader.battleSceneName);
        soundBox.PlayOneShot(audioClass.decide, 1f);
    }
}
