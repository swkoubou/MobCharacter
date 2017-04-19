using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//動かしたい矢印キーのオブジェクトにアタッチする
public class SelectArrow : MonoBehaviour
{
    //選択ボタン
    public Button[] selectButton;

    //Button.Select()を使うのに必要
    protected EventSystem eventSystem;

    //効果音ようのAudioSourceの空箱
    public AudioSource soundBox;

    //選択ボタンの効果音
    public AudioClip selectSE;

    //カーソルの位置調整
    public Vector3 offset;

    //現在取得しているボタン
    protected GameObject currentSelected;

    //最後に正常に取得したボタン, バックアップ用
    protected GameObject lastSelected;

    //画像を表示し、選択を開始して良いか
    public bool isStartSelect;   


    void Awake()
    {
        isStartSelect = false;
    }

    protected void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.enabled = false;

        //初期状態ではカーソルを見せない
        GetComponent<Image>().enabled = false;

        selectButton[0].Select();
        lastSelected = selectButton[0].gameObject;
    }


    protected void Update()
    {
        if (isStartSelect)
        {
            //クリックしてnullになってしまったら
            if (eventSystem.currentSelectedGameObject == null)
            {
                currentSelected = lastSelected;
            }
            //現在選択しているボタンを取得
            else
            {
                currentSelected = eventSystem.currentSelectedGameObject;
            }
            currentSelected.GetComponent<Button>().Select();

            //カーソルの位置を動かす
            for (int i = 0; i < selectButton.Length; i++)
            {
                if (currentSelected == selectButton[i].gameObject)
                {
                    AjustPosition(selectButton[i].gameObject);
                }
            }
        }
        else
        {
            eventSystem.enabled = false;
        }
    }


    //カーソルの位置を動かす + lastボタンにバックアップを取る
    public void AjustPosition(GameObject newPos)
    {
        //カーソルの位置調整
        Vector3 pos = newPos.transform.position;
        transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z + offset.z);

        if (currentSelected != lastSelected)
        {
            soundBox.PlayOneShot(selectSE, 1f);
        }

        //バックアップ
        lastSelected = currentSelected;
    }


    //ここにアクセスすると実行される
    public void StartSelect()
    {
        isStartSelect = true;
        GetComponent<Image>().enabled = true;
        eventSystem.enabled = true;
    }


    //ここにアクセスすると停止する
    public void StopSelect()
    {
        isStartSelect = false;
        GetComponent<Image>().enabled = false;
    }
}
