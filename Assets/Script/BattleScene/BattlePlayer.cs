using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : BaseOperator
{
    

    
    void Start()
    {

    }

    
    void Update()
    {
        
    }


    /*以下ボタン関数*/

    //攻撃コマンド
    public void OnAttack()
    {
        BattleManager.instance.OnCommandPushed();

        //実験用
        Invoke("OnAttackDetails", 2f);
    }

    public void OnAttackDetails()
    {
        if (BattleManager.instance.isPushed)
        {
            StartCoroutine(BattleManager.instance.ChangeTurnEnemy());

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
        FadeSceneManager.Execute("BoardScene");
    }
}
