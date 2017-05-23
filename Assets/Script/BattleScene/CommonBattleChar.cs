using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class CommonBattleChar : MonoBehaviour
{
    protected GameObject[] enemies;
    protected int HP;
    protected int attack;
    private float charMoveTime = 1f;

    protected Vector2 defaultPos;
    public Vector2 defaultOffset;
    public Button[] buttonsObject;
    public string[] buttonsText;

    //一度だけupdate関数内で使いたいので
    protected bool isOnce;


    protected void Start()
    {
        //enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //enemies = enemies.OrderBy(e => Vector2.Distance(e.transform.position, transform.position)).ToArray();
        isOnce = false;
    }

    protected void BraverTurn()
    {
        //Braverターンじゃないなら
        if (BattleManager.instance.GetWhoseTurn() != BattleManager.WhoseTurn.braver)
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

    protected void EnemyTurn()
    {
        //Enemyターンじゃないなら
        if (BattleManager.instance.GetWhoseTurn() != BattleManager.WhoseTurn.enemy)
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

    //グリッド配列にキャラクタを設置
    protected void SetGrid(GameObject obj, Vector2 pos)
    {
        if(obj == null)
        {
            BattleManager.instance.gridPositions[(int)pos.x, (int)pos.y] = null;
            //var moveHash = new Hashtable();
            //moveHash.Add("position", new Vector3(pos.x, pos.y, 0));
            //moveHash.Add("time", charMoveTime);
            //iTween.MoveTo(obj, moveHash);
        }
        else
        {
            CommonBattleChar component = obj.GetComponent<CommonBattleChar>();
            Vector2 offset = component.defaultOffset;
            BattleManager.instance.gridPositions[(int)pos.x, (int)pos.y] = obj;
            MoveGrid(obj, pos);
        }
    }

    //入れ替え
    public void ChangeGridPos(GameObject obj, Vector2 end)
    {
        SetGrid(ConvertVectorToObject(end), ConvertObjectToVector(obj));
        SetGrid(obj, end);
    }

    //キャラクターを指定先のグリッドに移動する
    protected void MoveGrid(GameObject obj, Vector2 target)
    {
        Vector2 offset = obj.GetComponent<CommonBattleChar>().defaultOffset;
        Vector2 end = BattleManager.instance.basePositions[(int)target.x, (int)target.y].transform.position;
        end += new Vector2(offset.x, offset.y);

        var moveHash = new Hashtable();
        moveHash.Add("position", new Vector3(end.x, end.y, 0));
        moveHash.Add("time", charMoveTime);
        iTween.MoveTo(obj, moveHash);
    }

    //指定したオブジェクトがどこに設置されているか調べて返す
    protected Vector2 ConvertObjectToVector(GameObject obj)
    {
        for (int i = 0; i < BattleManager.COUNT_BASE_POS; i++)
        {
            for (int j = 0; j < BattleManager.COUNT_BASE_POS; j++)
            {
                if (BattleManager.instance.gridPositions[i, j] == obj)
                {
                    return new Vector2(i, j);
                }
            }
        }

        //一致しなかったら-1, -1を返す
        return new Vector2(-1, -1);
    }

    //指定した位置情報に何が設置されているか調べて返す
    protected GameObject ConvertVectorToObject(Vector2 target)
    {
        return BattleManager.instance.gridPositions[(int)target.x, (int)target.y];
    }

    protected void SetMethod(UnityAction[] method)
    {
        for (int i = 0; i < buttonsObject.Length; i++)
        {
            buttonsObject[i].onClick.RemoveAllListeners();
            buttonsObject[i].onClick.AddListener(method[i]);
            buttonsObject[i].transform.FindChild("Text").GetComponent<Text>().text = buttonsText[i];
        }
    }


    /*以下ボタン関数*/    

    protected void OnMoveAttackVertical(GameObject obj)
    {
        if (BattleManager.instance.OnReadyDetails())
        {
            Vector2 nowPos = ConvertObjectToVector(obj);
            for (int i = 0; i < BattleManager.COUNT_BASE_POS; i++)
            {
                if (nowPos == new Vector2(i, 2))
                {
                    Vector2 movedPos = new Vector2(i, 0);
                    ChangeGridPos(obj, movedPos);
                    MoveGrid(obj, movedPos);

                    //向き反転
                    var tmp = obj.transform.localScale;
                    tmp.x *= -1;
                    obj.transform.localScale = tmp;
                }
                else if (nowPos == new Vector2(i, 0))
                {
                    Vector2 movedPos = new Vector2(i, 2);
                    ChangeGridPos(obj, movedPos);
                    MoveGrid(obj, movedPos);

                    //向き反転
                    var tmp = obj.transform.localScale;
                    tmp.x *= -1;
                    obj.transform.localScale = tmp;
                }
            }
        }
    }

    protected void OnMoveAttackSlash(GameObject obj, Vector2 newPos)
    {
        //対角に移動するので0と2を反転
        if (newPos.x == 0)
            newPos.x = 2;
        else if (newPos.x == 2)
            newPos.x = 0;
        else
            newPos.x = -1;

        if (newPos.y == 0)
            newPos.y = 2;
        else if (newPos.y == 2)
            newPos.y = 0;
        else
            newPos.y = -1;

        //対角に居ない場合は実行しない
        if (newPos.x == -1 || newPos.y == -1)
        {
            return;
        }

        if (BattleManager.instance.OnReadyDetails())
        {
            Vector2 movedPos = newPos;
            ChangeGridPos(obj, movedPos);
            MoveGrid(obj, movedPos);

            //向き反転
            var tmp = obj.transform.localScale;
            tmp.x *= -1;
            obj.transform.localScale = tmp;
        }
    }

    public void OnLocateMove()
    {

    }

    //EnemyのAI用
    protected abstract void SwitchCommand(int rand);
}
