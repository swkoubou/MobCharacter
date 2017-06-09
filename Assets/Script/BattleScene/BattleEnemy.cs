using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BattleEnemy : CommonBattleChara
{
    protected float changeTurnWaitTime = 1f;
    protected float enemyMoveTime;
    private bool canMove = true;

    protected new void Start()
    {
        //BattleManager.instance.enemies.Add(gameObject);
        enemyMoveTime = (charMoveTime + 1f) / 2;
        base.Start();
    }

    void Update()
    {

    }

    public void EnemyTurn()
    {
        BattleManager.instance.countEnemyTurn++;
        canMove = true;

        List<GameObject> players = SerchChara();
        GameObject target = SameRowObject(players);
        if (target != null)
        {
            print("1");
            SwitchCommand(target, Random.Range(0, 10));
        }
        else
        {
            float distance = NearestColObject(players);
            if (distance <= -1)
            {
                print("2");
                OnEnemyMoveLocation(-1);
            }
            else if (1 <= distance)
            {
                print("3");
                OnEnemyMoveLocation(1);
            }
        }
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

    protected GameObject SameRowObject(List<GameObject> player)
    {
        for (int i = 0; i < player.Count; i++)
        {
            if (ConvertObjectToVector(gameObject).x == ConvertObjectToVector(player[i]).x)
            {
                return player[i];
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

    protected void OnEnemyNormalAttack(GameObject target)
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //攻撃し、その場に留まるので
        movedPos.y = ConvertObjectToVector(target).y;

        //線形に居ない場合は実行しない
        if (movedPos.x == -1 || movedPos.y == -1)
        {
            return;
        }

        var moveHash = new Hashtable();
        //gridPOsiiotnsだとnullのときエラーが出るので
        moveHash.Add("x", BattleManager.instance.basePositions[(int)movedPos.x, (int)movedPos.y].transform.position.x);
        moveHash.Add("time", enemyMoveTime);
        iTween.MoveFrom(gameObject, moveHash);

        soundBox.PlayOneShot(audioClass.normalAttack, 1f);
        BattleManager.instance.AddMessage(objectName + "の攻撃");
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
            if (!canMove)
                BattleManager.instance.TurnSkip();
            else
                canMove = false;
            print(canMove);
            OnEnemyMoveLocation(-n);
        }
        else if (ConvertVectorToObject(movedPos) != null)
        {
            print("移動来ません" + n);
            BattleManager.instance.TurnSkip();
        }
        else
        {
            ChangeGrid(gameObject, movedPos);
            BattleManager.instance.AddMessage(objectName + "は移動した");
        }
    }

    //EnemyのAI用
    protected abstract void SwitchCommand(GameObject target, int rand);
}
