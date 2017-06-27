using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BattleEnemy : CommonBattleChara
{
    protected float changeTurnWaitTime = 1f;
    protected float enemyMoveTime;
    private bool canMove;

    protected new void Start()
    {
        //BattleManager.instance.enemies.Add(gameObject);
        enemyMoveTime = (charMoveTime + 1f) / 2;
        base.Start();
    }

    void Update()
    {
        
    }

    //DestroyされたらListからも消しておく
    //private void OnDestroy()
    //{
    //    BattleManager.instance.enemies.Remove(gameObject);
    //    print("COUNT= "+BattleManager.instance.enemies.Count);
    //}

    public void EnemyTurn()
    {
        SwitchCommand(Random.Range(0, 10));
    }

    protected void NonTarget()
    {
        canMove = true;

        List<GameObject> players = SerchChara();
        float distance = NearestColObject(players);
        if (distance <= -1)
        {
            print("2");
            OnEnemyMoveLocation(-1);
            BattleManager.instance.NextEnemyTurn();
        }
        else if (1 <= distance)
        {
            print("3");
            OnEnemyMoveLocation(1);
            BattleManager.instance.NextEnemyTurn();
        }

        //List<GameObject> players = SerchChara();
        //GameObject target = SameRowObject(players);
        //if (target != null)
        //{
        //    print("1");
        //    SwitchCommand(Random.Range(0, 10));
        //    BattleManager.instance.NextEnemyTurn();
        //}
        //else
        //{
        //    float distance = NearestColObject(players);
        //    if (distance <= -1)
        //    {
        //        print("2");
        //        OnEnemyMoveLocation(-1);
        //        BattleManager.instance.NextEnemyTurn();
        //    }
        //    else if (1 <= distance)
        //    {
        //        print("3");
        //        OnEnemyMoveLocation(1);
        //        BattleManager.instance.NextEnemyTurn();
        //    }
        //}
    }

    //プレイヤー達を検索し取得
    protected List<GameObject> SerchChara()
    {
        List<GameObject> players = new List<GameObject>();
        for (int i = 0; i < BattleManager.COUNT_BASE_POS; i++)
        {
            for (int j = 0; j < BattleManager.COUNT_BASE_POS; j++)
            {
                var obj = BattleManager.instance.gridPositions[i, j];

                //&&だと先の条件がfalseだと、それより後の条件は通らないので
                if (obj != null && obj.tag == "Player")
                {
                    players.Add(BattleManager.instance.gridPositions[i, j]);
                }
            }
        }

        return players;
    }

    protected GameObject SameRowObject(List<GameObject> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (ConvertObjectToVector(gameObject).x == ConvertObjectToVector(players[i]).x)
            {
                return players[i];
            }
        }

        //横軸が一致しなかったら
        return null;
    }

    protected float NearestColObject(List<GameObject> player)
    {
        float[] diff = new float[player.Count];
        for (int i = 0; i < player.Count; i++)
        {
            diff[i] = ConvertObjectToVector(player[i]).y - ConvertObjectToVector(gameObject).y;
        }

        float nearest = diff[0];
        for (int i = 0; i < player.Count - 1; i++)
        {
            if (Mathf.Abs(diff[i]) < Mathf.Abs(diff[i + 1]))
                nearest = diff[i + 1];
        }

        return nearest;
    }

    protected void OnEnemyNormalAttack()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        if(ConvertVectorToObject(new Vector2(movedPos.x, movedPos.y+1)) != null)
        {
            MoveAttack(new Vector2(movedPos.x, movedPos.y + 1));
        }
        else if(ConvertVectorToObject(new Vector2(movedPos.x, movedPos.y - 1)) != null)
        {
            MoveAttack(new Vector2(movedPos.x, movedPos.y - 1));
        }
        else
        {
            NonTarget();
        }
    }

    protected void MoveAttack(Vector2 target)
    {
        //線形に居ない場合は実行しない
        if (target.x == -1 || target.y == -1)
        {
            return;
        }

        var moveHash = new Hashtable();
        //gridPosiiotnsだとnullのときエラーが出るので
        moveHash.Add("x", BattleManager.instance.basePositions[(int)target.x, (int)target.y].transform.position.x);
        moveHash.Add("time", enemyMoveTime);
        iTween.MoveFrom(gameObject, moveHash);

        soundBox.PlayOneShot(audioClass.normalAttack, 1f);
        BattleManager.instance.AddMessage(objectName + "の攻撃");
        ConvertVectorToObject(target).GetComponent<CommonBattleChara>().DamagedAnim(attack);
        BattleManager.instance.NextEnemyTurn();
    }

    //移動する
    protected void OnEnemyMoveLocation(int n)
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += n;
        print(gameObject + ":" + n);
        //移動先がEnemyで動けないなら
        if (ConvertVectorToObject(movedPos) != null && ConvertVectorToObject(movedPos).tag == "Enemy")
        {
            print("4");
            if (!canMove)
                return;
            else
            {
                canMove = false;
                print("canMove: " + canMove);
                OnEnemyMoveLocation(-n);
            }
        }
        else if (ConvertVectorToObject(movedPos) != null)
        {
            print("5");
            if (!canMove)
                return;
            else
            {
                canMove = false;
                print("canMove: " + canMove);
                print("移動来ません" + n);
            }
        }
        else
        {
            ChangeGrid(gameObject, movedPos);
            BattleManager.instance.AddMessage(objectName + "は移動した");
        }
    }

    //EnemyのAI用
    protected abstract void SwitchCommand(int rand);
}
