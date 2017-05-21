using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class CommonBattleChar : MonoBehaviour
{
    protected BattlePlayer player;
    protected BattleBraver braver;
    protected BattlePrincess princess;
    protected GameObject[] enemies;
    protected int HP;
    protected int attack;
    private float charMoveTime = 1f;

    public Button[] buttonsObject;
    public string[] buttonsText;

    //一度だけupdate関数内で使いたいので
    protected bool isOnce;


    protected void Start()
    {
        player = FindObjectOfType<BattlePlayer>();
        braver = FindObjectOfType<BattleBraver>();
        princess = FindObjectOfType<BattlePrincess>();
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
    protected void AddGridPos(GameObject obj, Vector2 pos)
    {
        BattleManager.instance.gridPositions[(int)pos.x, (int)pos.y] = obj;
    }

    //指定したオブジェクトがどこに設置されているか調べて返す
    protected Vector2 GetGridPosVector2(GameObject obj)
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

    //キャラクターを指定先のグリッドに移動する
    protected void MoveGrid(GameObject obj, Vector2 target, Vector2 offset = default(Vector2))
    {
        Vector2 end = BattleManager.instance.basePositions[(int)target.x, (int)target.y].transform.position;
        end += new Vector2(ConvertAspect.GetWidth(offset.x), ConvertAspect.GetHeight(offset.y));

        var moveHash = new Hashtable();
        moveHash.Add("position", new Vector3(end.x, end.y, 0));
        moveHash.Add("time", charMoveTime);
        iTween.MoveTo(obj, moveHash);
    }

    ////今居る位置から移動先の位置までの距離を求める
    //public float GetGridDiffX(Vector2 start, Vector2 end)
    //{
    //    if (start.y != end.y)
    //        return -1;

    //    Vector2 diff = instance.basePositions[(int)end.x, (int)end.y].transform.position - instance.basePositions[(int)start.x, (int)start.y].transform.position;
    //    return diff.x;
    //}

    ////今居る位置から移動先の位置までの距離を求める
    //public float GetGridDiffY(Vector2 start, Vector2 end)
    //{
    //    if (start.x != end.x)
    //        return -1;

    //    Vector2 diff = instance.basePositions[(int)end.x, (int)end.y].transform.position - instance.basePositions[(int)start.x, (int)start.y].transform.position;
    //    return diff.y;
    //}

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

    //攻撃コマンド
    public void OnAttack()
    {
        BattleManager.instance.OnCommandPushed();
        BattleManager.instance.mainArrow.StopSelect();
        
        if(BattleManager.instance.GetWhoseTurn() == BattleManager.WhoseTurn.player)
            BattleManager.instance.subArrow.RebootSelectButton(player.buttonsObject);
        else if (BattleManager.instance.GetWhoseTurn() == BattleManager.WhoseTurn.braver)
            BattleManager.instance.subArrow.RebootSelectButton(braver.buttonsObject);
        //else if (BattleManager.instance.GetWhoseTurn() == BattleManager.WhoseTurn.princess)
        //    BattleManager.instance.subArrow.ResetSelectButton(princess.buttonsObject);
    }

    //準備ができているならtrue, できていないならfalsaeを返す
    public bool OnAttackDetails()
    {
        if (BattleManager.instance.isPushed && !FindObjectOfType<iTween>())
        {
            StartCoroutine(BattleManager.instance.ChangeTurnBraver());

            BattleManager.instance.OnCommandBaack();
            BattleManager.instance.mainCommand.SetActive(false);

            return true;
        }
        else
        {
            return false;
        }
    }

    protected void OnMoveAttackVertical(GameObject obj, Vector2 offset)
    {
        if (OnAttackDetails())
        {
            Vector2 nowPos = BattleManager.instance.GetGridPosVector2(obj);
            print(nowPos);
            for (int i = 0; i < BattleManager.COUNT_BASE_POS; i++)
            {
                if (nowPos == new Vector2(i, 2))
                {
                    Vector2 movedPos = new Vector2(i, 0);
                    BattleManager.instance.ChangeGridPos(obj, movedPos);
                    MoveGrid(obj, movedPos, offset);
                }
                else if (nowPos == new Vector2(i, 0))
                {
                    Vector2 movedPos = new Vector2(i, 2);
                    BattleManager.instance.ChangeGridPos(obj, movedPos);
                    MoveGrid(obj, movedPos, offset);
                }
            }
        }
    }

    //EnemyのAI用
    protected abstract void SwitchCommand(int rand);
}
