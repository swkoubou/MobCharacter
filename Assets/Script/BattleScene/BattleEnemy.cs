using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEnemy : MonoBehaviour
{


    void Start()
    {

    }


    void Update()
    {
        if (!BattleManager.instance.isTurn)
            return;

        int rand = Random.Range(0, 10);
        SwitchCommand(rand);
    }


    protected abstract void SwitchCommand(int rand);
}
