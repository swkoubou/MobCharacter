using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEnemy : MonoBehaviour
{
    protected BattlePlayer player;
    protected BattleBraver braver;
    private int HP;
    protected bool isOnce;



    void Start()
    {
        player = FindObjectOfType<BattlePlayer>();
        braver = FindObjectOfType<BattleBraver>();
        isOnce = false;
    }


    protected void EnemyTurn()
    {
        //Playerのターンなら
        if (BattleManager.instance.GetTurn())
        {
            isOnce = false;
            return;
        }

        int rand = Random.Range(0, 10);
        if (!isOnce)
        {
            isOnce = true;
            SwitchCommand(rand);
        }
    }


    public void LoseHP(int dmg)
    {
        HP -= dmg;
    }


    protected void ReverseOnce()
    {
        BattleEnemy self = GetComponent<BattleEnemy>();
        print(self);
        self.isOnce = !self.isOnce;
    }


    protected abstract void SwitchCommand(int rand);
}
