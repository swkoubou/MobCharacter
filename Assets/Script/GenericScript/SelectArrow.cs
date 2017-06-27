using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//動かしたい矢印キーのオブジェクトにアタッチする
//事前に矢印ImageとScriptはenabled = falseにしておく
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
    public Vector3 defaultOffset;

    //offsetを一時確保する空要素
    protected Vector3 offset = default(Vector3);

    //現在取得しているボタン
    protected GameObject currentSelected;

    //最後に正常に取得したボタン, バックアップ用
    protected GameObject lastSelected;

    //画像を表示し、選択を開始して良いか
    [HideInInspector]
    public bool isStartSelect;


    //初期化
    void Initialized()
    {
        isStartSelect = true;
        eventSystem = FindObjectOfType<EventSystem>();
        GetComponent<Image>().enabled = true;
        eventSystem.enabled = true;        

        selectButton[0].Select();
        currentSelected = selectButton[0].gameObject;
        lastSelected = selectButton[0].gameObject;
        AjustPosition(selectButton[0].gameObject);
    }


    protected void OnEnable()
    {
        Initialized();
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

        if(offset == default(Vector3))
            transform.position = new Vector3(pos.x + defaultOffset.x, pos.y + defaultOffset.y, pos.z + defaultOffset.z);
        else
            transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z + offset.z);

        if (currentSelected != lastSelected)
        {
            soundBox.PlayOneShot(selectSE, 1f);
        }

        //バックアップ
        lastSelected = currentSelected;
    }

    //ボタンのOnClikeを再設定し
    public void SetButtons(Button[] newButton, Vector3 newOffset = default(Vector3))
    {
        selectButton = new Button[newButton.Length];
        for (int i = 0; i < newButton.Length; i++)
        {
            selectButton[i] = newButton[i];
        }

        offset = newOffset;
    }


    //ここにアクセスすると実行される
    public void StartSelect()
    {
        GetComponent<SelectArrow>().enabled = true;
    }


    //ここにアクセスすると停止する
    public void StopSelect()
    {
        isStartSelect = false;
        GetComponent<Image>().enabled = false;
        GetComponent<SelectArrow>().enabled = false; 
    }
}
