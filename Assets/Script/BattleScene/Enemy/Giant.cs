using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : BattleEnemy
{
    new void Start()
    {
        HP = 30;
        attack = 6;
        defaultOffset = new Vector2(0.2f, 2.3f);
        hpberOffset = new Vector2(-0.2f, -2.6f);
        attackText = new string[2] { "毒々しい緑", "大地の終わり" };
        base.Start();
    }

    void Update()
    {
        
    }

    protected override void SwitchCommand(int rand)
    {
        switch (rand)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                OnEnemyNormalAttack();
                break;

            case 5:
            case 6:
            case 7:
                Poison1();
                break;

            case 8:
            case 9:
                Poison2();
                break;
        }
    }

    private void Poison1()
    {
        for(int i=0; i < 3; i++)
        {
            GameObject target = ConvertVectorToObject(new Vector2(i, 2));
            if (target != null)
            {
                GameObject effect = Instantiate(Resources.Load("Effecter")) as GameObject;
                effect.transform.position = target.transform.position;
                Destroy(effect, 2f);
                OnlyAnim(effect, controller[0],null, null);
                target.GetComponent<CommonBattleChara>().DamagedAnim(attack);
            }
        }
        BattleManager.instance.AddMessage(objectName + "の" + attackText[0] + "!");
        BattleManager.instance.NextEnemyTurn();
    }

    private void Poison2()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject right = ConvertVectorToObject(new Vector2(i, 2));
            if (right != null)
            { 
                GameObject effect = Instantiate(Resources.Load("Effecter")) as GameObject;
                effect.transform.position = right.transform.position;
                Destroy(effect, 2f);
                OnlyAnim(effect, controller[1], null, null);
                right.GetComponent<CommonBattleChara>().DamagedAnim(attack);
            }

            GameObject left = ConvertVectorToObject(new Vector2(i, 0));
            if (left != null)
            {
                GameObject effect = Instantiate(Resources.Load("Effecter")) as GameObject;
                effect.transform.position = left.transform.position;
                Destroy(effect, 2f);
                OnlyAnim(effect, controller[1], null, null);
                left.GetComponent<CommonBattleChara>().DamagedAnim(attack);
            }
        }
        BattleManager.instance.AddMessage(objectName + "の" + attackText[1] + "!");
        BattleManager.instance.NextEnemyTurn();
    }
}
