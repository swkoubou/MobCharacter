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
        attack = 2;
        defaultPos = new Vector2(1, 0);
        defaultOffset = new Vector2(0, 1);
        hpberOffset = new Vector2(0, -1.32f);

        attackText = new string[3] {"通常攻撃", "バーチカルアタック", "スラッシュスラント"};
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

    public new void OnNormalAttack()
    {
        //base.OnNormalAttack();
        BattleManager.instance.stackCommandPlayer = new BattleManager.StackCommandPlayer(base.OnNormalAttack);
        BattleManager.instance.ChangeTurnNext();
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
        FadeSceneManager.Execute(Loader.boardSceneName);
        soundBox.PlayOneShot(audioClass.escape, 1f);
    }
}
