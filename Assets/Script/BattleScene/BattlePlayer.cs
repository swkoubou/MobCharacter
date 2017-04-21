using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : MonoBehaviour
{

    
    void Start()
    {

    }

    
    void Update()
    {
        
    }


    bool CheckTurn()
    {
        if (BattleManager.instance.isTurn)
        {
            StartCoroutine(Wait(5f));
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        BattleManager.instance.isTurn = false;
    }


    public void OnAttackButton()
    {

    }
}
