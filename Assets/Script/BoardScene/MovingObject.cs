using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public int HP;
    public int attack;
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    protected bool isMoving;
    protected bool isOnce = false;

    //moveTimeを計算するのを単純化するための変数
    private float inverseMoveTime;

    protected AudioClass audioClass;


    protected virtual void Start()
    {
        //BoxCollider2DとRigidbody2Dを何度もGetComponentしなくて済むようStartメソッドにてキャッシュしておく
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        isMoving = false;
        inverseMoveTime = 1f / moveTime;
        audioClass = FindObjectOfType<AudioClass>();
    }


    void Update()
    {
        
    }


    public virtual void LoseHP(int dmg)
    {
        HP -= dmg;
        BoardManager.instance.DamagedAnim(GetComponent<SpriteRenderer>());
    }


    //移動可能かを判断するメソッド　可能な場合はSmoothMovementへ
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        //現在地を取得
        Vector2 start = transform.position;

        //目的地を取得
        Vector2 end = start + new Vector2(xDir, yDir);

        //自身のColliderを無効にし、Linecastで自分自身を判定しないようにする
        boxCollider.enabled = false;

        //現在地と目的地との間にblockingLayerのついたオブジェクトが無いか判定
        hit = Physics2D.Linecast(start, end, blockingLayer);

        //Colliderを有効に戻す
        boxCollider.enabled = true;

        //何も無ければSmoothMovementへ遷移し移動処理
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }


    //現在地から目的地(引数end)へ移動するためのメソッド
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;

        //現在地から目的地を引き、2点間の距離を求める(Vector3型)
        //sqrMagnitudeはベクトルを2乗したあと2点間の距離に変換する(float型)
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //2点間の距離が0になった時、ループを抜ける
        //Epsilon : ほとんど0に近い数値を表す
        while (sqrRemainingDistance > float.Epsilon)
        {
            //現在地と移動先の間を1秒間にinverseMoveTime分だけ移動する場合の、
            //1フレーム分の移動距離を算出する
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //算出した移動距離分、移動する
            rb2D.MovePosition(newPosition);

            //現在地が目的地寄りになった結果、sqrRemainDistanceが小さくなる
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //1フレーム待ってから、while文の先頭へ戻る
            yield return null;
        }
        isMoving = false;
    }


    //移動を試みるメソッド
    //virtual : 継承されるメソッドに付ける修飾子
    //<T>：ジェネリック機能　型を決めておかず、後から指定する
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        //ジェネリック用の型引数をComponent型で限定
        where T : Component
    {
        //Enemyが死ぬときにエラーが発生するので、それを回避するため
        //デストロイされているならなにもしない
        if (this == null)
            return;

        RaycastHit2D hit;
        
        //Moveメソッド実行 戻り値がtrueなら移動成功、falseなら移動失敗
        bool canMove = Move(xDir, yDir, out hit);
        
        //移動できないところに行こうとしたら音を出す
        if (hit.transform != null)
        {
            if(hit.transform.gameObject.tag == "Inmortal")
                BoardManager.instance.soundBox.PlayOneShot(audioClass.notExecute, 1f);
            else if(hit.transform.gameObject.tag == "Wall")
                BoardManager.instance.soundBox.PlayOneShot(audioClass.notExecute, 1f);
        }

        //Moveメソッドで確認した障害物が何も無ければメソッド終了
        if (hit.transform == null)
        {
            return;
        }
        
        //障害物があった場合、障害物を型引数の型で取得
        //型が<T>で指定したものと違う場合、取得できない
        T hitComponent = hit.transform.GetComponent<T>();

        //障害物がある場合OnCantMoveを呼び出す
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    //攻撃対象が2つの場合
    protected virtual void AttemptMove<T, U>(int xDir, int yDir)
        where T : Component
        where U : Component
    {
        if (this == null)
            return;

        RaycastHit2D hit;

        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        T hitComponent1 = hit.transform.GetComponent<T>();
        U hitComponent2 = hit.transform.GetComponent<U>();

        if (!canMove && hitComponent1 != null)
        {
            OnCantMove(hitComponent1);
        }
        else if (!canMove && hitComponent2 != null)
        {
            OnCantMove(hitComponent2);
        }
    }

    //abstract: メソッドの中身はこちらでは書かず、サブクラスにて書く
    //<T>：AttemptMoveと同じくジェネリック機能
    //障害物があり移動ができなかった場合に呼び出される
    protected abstract void OnCantMove<T>(T component) where T : Component;
}
