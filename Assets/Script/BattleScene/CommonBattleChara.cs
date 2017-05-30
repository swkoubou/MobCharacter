using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommonBattleChara : MonoBehaviour
{
    protected int HP;
    protected int attack;
    protected float charMoveTime = 1f;

    public Vector2 defaultPos;
    public Vector2 defaultOffset;
    public Button[] buttonsObject;
    public string[] buttonsText;

    protected AudioClass audioClass;
    protected AudioSource soundBox;

    private string message;


    //protected void Start()
    //{
    //    enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    enemies = enemies.OrderBy(e => Vector2.Distance(e.transform.position, transform.position)).ToArray();
    //}   

    protected void Start()
    {
        SetGrid(gameObject, defaultPos);
        audioClass = FindObjectOfType<AudioClass>();
        soundBox = FindObjectOfType<AudioSource>();
    }

    public void LoseHP(int dmg)
    {
        HP -= dmg;
    }

    protected void PrintMessage()
    {

    }

    //グリッド配列にキャラクタを設置
    protected void SetGrid(GameObject obj, Vector2 pos)
    {
        if(obj == null)
        {
            BattleManager.instance.gridPositions[(int)pos.x, (int)pos.y] = null;
        }
        else
        {
            CommonBattleChara component = obj.GetComponent<CommonBattleChara>();
            Vector2 offset = component.defaultOffset;
            BattleManager.instance.gridPositions[(int)pos.x, (int)pos.y] = obj;
            MoveGrid(obj, pos);
        }
    }

    //入れ替え
    protected void ChangeGrid(GameObject obj, Vector2 target)
    {
        SetGrid(ConvertVectorToObject(target), ConvertObjectToVector(obj));
        SetGrid(obj, target);
    }

    //移動先がEnemyでなかったら移動可能
    protected bool CanChangeGrid(Vector2 target)
    {
        GameObject obj = ConvertVectorToObject(target);
        if(obj == null)
        {
            return true;
        }
        else if(obj.tag != "Enemy")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //protected bool CheckRegularPos(Vector2 pos)
    //{
    //    if (pos.x < 0 || 2 < pos.x || pos.y < 0 || 2 < pos.y)
    //        return false;
    //    else
    //        return true;
    //}

    //キャラクターを指定先のグリッドに移動する
    protected void MoveGrid(GameObject obj, Vector2 target)
    {
        Vector2 offset = obj.GetComponent<CommonBattleChara>().defaultOffset;
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
        //範囲外なら
        if (target.x < 0 || 2 < target.x || target.y < 0 || 2 < target.y)
        {
            var tmp = new GameObject();
            Destroy(tmp, 0.1f);
            return tmp;
        } 
        else
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

    protected void OnNormalAttack()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //攻撃し、その場に留まるので
        movedPos.y = 1;

        //線形に居ない場合は実行しない
        if (movedPos.x == -1 || movedPos.y == -1)
        {
            return;
        }

        if (BattleManager.instance.OnReadyDetails())
        {
            var moveHash = new Hashtable();
            //gridPOsiiotnsだとnullのときエラーが出るので
            moveHash.Add("x", BattleManager.instance.basePositions[(int)movedPos.x, (int)movedPos.y].transform.position.x);
            moveHash.Add("time", charMoveTime);
            iTween.MoveFrom(gameObject, moveHash);
            soundBox.PlayOneShot(audioClass.normalAttack, 1f);
        }
    }

    protected void OnAttackMoveVertical()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //向かい側に移動するのでyだけ動かす
        if (movedPos.y == 0)
            movedPos.y = 2;
        else if (movedPos.y == 2)
            movedPos.y = 0;
        else
            movedPos.y = -1;

        //線形に居ない場合は実行しない
        if (movedPos.x == -1 || movedPos.y == -1)
        {
            return;
        }

        if (CanChangeGrid(movedPos))
        {
            if (BattleManager.instance.OnReadyDetails())
            {
                ChangeGrid(gameObject, movedPos);
                MoveGrid(gameObject, movedPos);

                //向き反転
                var tmp = transform.localScale;
                tmp.x *= -1;
                transform.localScale = tmp;
            }
        }
        else
        {
            print("移動来ません");
        }
    }

    protected void OnAttackMoveSlash()
    {
        Vector2 nowPos = ConvertObjectToVector(gameObject);

        //対角に移動するので0と2を反転
        if (nowPos.x == 0)
            nowPos.x = 2;
        else if (nowPos.x == 2)
            nowPos.x = 0;
        else
            nowPos.x = -1;

        if (nowPos.y == 0)
            nowPos.y = 2;
        else if (nowPos.y == 2)
            nowPos.y = 0;
        else
            nowPos.y = -1;

        //対角に居ない場合は実行しない
        if (nowPos.x == -1 || nowPos.y == -1)
        {
            return;
        }

        if (CanChangeGrid(nowPos))
        {
            if (BattleManager.instance.OnReadyDetails())
            {
                ChangeGrid(gameObject, nowPos);
                MoveGrid(gameObject, nowPos);

                //向き反転
                var tmp = transform.localScale;
                tmp.x *= -1;
                transform.localScale = tmp;
            }
        }
        else
        {
            print("移動来ません");
        }
    }

    public void OnMoveUp()
    {
        Vector2 pos = ConvertObjectToVector(gameObject);

        //上段に居ないときのみ実行
        if(pos.x != 0)
            OnMoveLocation(-1);
    }

    public void OnMoveDown()
    {
        Vector2 pos = ConvertObjectToVector(gameObject);

        //下段に居ないときのみ実行
        if (pos.x != 2)
            OnMoveLocation(1);
    }

    //移動する
    protected void OnMoveLocation(int n)
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += n;

        if (ConvertVectorToObject(movedPos) != null)
        {
            print("移動来ません");
        }
        else
        {
            if (BattleManager.instance.OnReadyDetails())
            {
                ChangeGrid(gameObject, movedPos);
            }
        }
    }
}
