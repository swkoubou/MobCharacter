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

    public string objectName;
    public Vector2 defaultPos;
    protected Vector2 defaultOffset;

    protected Slider HPber;
    protected Vector2 hpberOffset;

    public Button[] attackButtons;
    public Button[] idleButtons;

    [HideInInspector]
    public string[] attackText;
    [HideInInspector]
    public string[] idleText;

    public RuntimeAnimatorController[] controller;
    protected Animator anim;
    protected GameObject effecter;

    protected AudioClass audioClass;
    protected AudioSource soundBox;

    //エラーメッセージリスト
    public class MessageList
    {
        //攻撃系
        public string nonTarget = "攻撃目標が居ません";

        //移動系
        public string nonMove = "移動できません";
    }
    public MessageList messageList = new MessageList();

    //protected void Start()
    //{
    //    enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    enemies = enemies.OrderBy(e => Vector2.Distance(e.transform.position, transform.position)).ToArray();
    //}   

    protected void Start()
    {
        //アニメーション用のボックスを子オブジェクトとして生成
        effecter = (Instantiate(Resources.Load("Effecter"), transform) as GameObject);
        effecter.transform.SetParent(gameObject.transform);

        //HPバーを子オブジェクトとして生成
        var canvas = (Instantiate(Resources.Load("HPber"), (Vector2)transform.position + hpberOffset, Quaternion.identity) as GameObject);
        canvas.transform.SetParent(gameObject.transform);
        HPber = canvas.GetComponentInChildren<Slider>();
        HPber.maxValue = HP;
        HPber.value = HPber.maxValue;

        SetGrid(gameObject, defaultPos);
        anim = GetComponentInChildren<Animator>();
        audioClass = FindObjectOfType<AudioClass>();
        soundBox = FindObjectOfType<AudioSource>();
    }

    //攻撃されたとき、攻撃側からここにアクセス
    public void DamagedAnim(int dmg)
    {
        HP -= dmg;
        HPber.value = HP;

        if (HP <= 0)
        {
            iTween.FadeTo(gameObject, iTween.Hash("alpha", 0f, "time", 2f));
            Destroy(gameObject, 2f);
        }
        else
        {
            iTween.ShakePosition(gameObject, iTween.Hash("x", 0.1f, "time", 1.5f));

            var options = FlashingManager.Hash("minAlpha", 0.3f, "color", Color.red, "count", 4, "time", 1.5f);
            FlashingManager.Execute(GetComponent<SpriteRenderer>(), options);
        }
    }

    //public void LoseHP(int dmg)
    //{
    //    HP -= dmg;
    //    HPber.value = HP;

    //    if (HP <= 0)
    //        Delete();
    //}

    //protected void Delete()
    //{
    //    var options = new Dictionary<string, object>();
    //    options.Add("time", 2f);
    //    FlashingManager.Execute(gameObject.GetComponent<SpriteRenderer>(), options);
    //    Destroy(gameObject, 1.5f);
    //}

    //グリッド配列にキャラクタを設置
    protected void SetGrid(GameObject obj, Vector2 pos)
    {
        if(obj == null)
        {
            BattleManager.instance.gridPositions[(int)pos.x, (int)pos.y] = null;
        }
        else
        {
            var component = obj.GetComponent<CommonBattleChara>();
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

    //キャラクターを指定先のグリッドに移動する
    protected void MoveGrid(GameObject obj, Vector2 target)
    {
        CheckDirection(obj, target);
        Vector2 offset = obj.GetComponent<CommonBattleChara>().defaultOffset;
        Vector2 end = BattleManager.instance.basePositions[(int)target.x, (int)target.y].transform.position;
        end += new Vector2(offset.x, offset.y);

        var moveHash = new Hashtable();
        moveHash.Add("position", new Vector3(end.x, end.y, 0));
        moveHash.Add("time", charMoveTime);
        iTween.MoveTo(obj, moveHash);
    }

    //向きをチェック
    protected void CheckDirection(GameObject obj, Vector2 target)
    {
        var tmp = obj.transform.localScale;
        if (target.y == 2)
        {
            if (tmp.x < 0)
                tmp.x *= -1;
        }
        else if (target.y == 0)
        {
            if (0 <= tmp.x)
                tmp.x *= -1;
        }
        obj.transform.localScale = tmp;
    }

    //指定したオブジェクトがどこに設置されているか調べて返す
    public Vector2 ConvertObjectToVector(GameObject obj)
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
    public GameObject ConvertVectorToObject(Vector2 target)
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
        for (int i = 0; i < attackButtons.Length; i++)
        {
            attackButtons[i].onClick.RemoveAllListeners();
            attackButtons[i].onClick.AddListener(method[i]);
            attackButtons[i].transform.GetComponentInChildren<Text>().text = attackText[i];
        }
    }

    protected void AnimStart(RuntimeAnimatorController effect, AudioClip se)
    {
        anim.runtimeAnimatorController = effect;
        anim.SetTrigger("Start");
        soundBox.PlayOneShot(se, 1f);
    }

    /*以下ボタン関数*/

    //その場から動かずアニメーションを使いたいとき
    protected void OnOnlyAnim(RuntimeAnimatorController effect, AudioClip se, string message)
    {
        if (BattleManager.instance.OnReadyDetails())
        {
            anim.runtimeAnimatorController = effect;
            anim.SetTrigger("Start");
            soundBox.PlayOneShot(se, 1f);
            BattleManager.instance.AddMessage(objectName + message);
        }
    }

    protected void DelayChange()
    {
        BattleManager.instance.ChangeTurnNext();
    }

    protected void OnNormalAttack()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //攻撃し、その場に留まるので
        movedPos.y = 1;

        //線形に居ない場合は実行しない
        if (movedPos.x == -1 || movedPos.y == -1 || ConvertVectorToObject(movedPos) == null)
        {
            BattleManager.instance.AddMessage(messageList.nonTarget);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        if (BattleManager.instance.OnReadyDetails())
        {
            var moveHash = new Hashtable();
            //gridPOsiiotnsだとnullのときエラーが出るので
            moveHash.Add("x", BattleManager.instance.basePositions[(int)movedPos.x, (int)movedPos.y].transform.position.x);
            moveHash.Add("time", charMoveTime);
            iTween.MoveFrom(gameObject, moveHash);

            AnimStart(null, audioClass.normalAttack);
            BattleManager.instance.AddMessage(objectName + "の通常攻撃!");
            //Invoke("DelayChange", BattleManager.instance.changeTurnWaitTime);
            ConvertVectorToObject(movedPos).GetComponent<CommonBattleChara>().DamagedAnim(attack);
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
                BattleManager.instance.AddMessage(objectName + "の移動攻撃!");
                BattleManager.instance.ChangeTurnNext();
            }
        }
        else
        {
            print(messageList.nonMove);
        }
    }

    protected void OnAttackMoveSlash()
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);

        //対角に移動するので0と2を反転
        if (movedPos.x == 0)
            movedPos.x = 2;
        else if (movedPos.x == 2)
            movedPos.x = 0;
        else
            movedPos.x = -1;

        if (movedPos.y == 0)
            movedPos.y = 2;
        else if (movedPos.y == 2)
            movedPos.y = 0;
        else
            movedPos.y = -1;

        //対角に居ない場合は実行しない
        if (movedPos.x == -1 || movedPos.y == -1)
        {
            BattleManager.instance.AddMessage(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
            return;
        }

        if (CanChangeGrid(movedPos))
        {
            if (BattleManager.instance.OnReadyDetails())
            {
                ChangeGrid(gameObject, movedPos);
                MoveGrid(gameObject, movedPos);
                BattleManager.instance.AddMessage(objectName + "のスラッシュ攻撃!");
                BattleManager.instance.ChangeTurnNext();
            }
        }
        else
        {
            print(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
        }
    }

    public void OnMoveUp()
    {
        Vector2 pos = ConvertObjectToVector(gameObject);

        //上段に居ないときのみ実行
        if (pos.x != 0)
            OnMoveLocation(-1);
        else
        {
            BattleManager.instance.AddMessage(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
        }
    }

    public void OnMoveDown()
    {
        Vector2 pos = ConvertObjectToVector(gameObject);

        //下段に居ないときのみ実行
        if (pos.x != 2)
            OnMoveLocation(1);
        else
        {
            BattleManager.instance.AddMessage(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
        }
    }

    //移動する
    protected void OnMoveLocation(int n)
    {
        Vector2 movedPos = ConvertObjectToVector(gameObject);
        movedPos.x += n;

        if (ConvertVectorToObject(movedPos) != null)
        {
            BattleManager.instance.AddMessage(messageList.nonMove);
            soundBox.PlayOneShot(audioClass.notExecute, 1f);
        }
        else
        {
            if (BattleManager.instance.OnReadyDetails())
            {
                ChangeGrid(gameObject, movedPos);
                BattleManager.instance.ChangeTurnNext();
            }
        }
    }
}
