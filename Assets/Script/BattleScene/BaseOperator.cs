using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class BaseOperator : MonoBehaviour
{
    protected GameObject[] enemies;
    private int HP;

    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = enemies.OrderBy(e => Vector2.Distance(e.transform.position, transform.position)).ToArray();

        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    print(enemies[i]);
        //}
    }


    void Update()
    {

    }

    public void LoseHP(int dmg)
    {
        HP -= dmg;
    }
}