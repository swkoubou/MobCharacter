using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleSelect : SelectArrow {
    public Button[] childButton;
    public bool isOnChild = false;
    public Vector3 childOffset;
	
	void Start () {
        eventSystem = FindObjectOfType<EventSystem>();
        selectButton[0].Select();
        lastSelected = selectButton[0].gameObject;
    }

    void Update () {
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

            //子供要素に居るか
            if (!isOnChild)
            {
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
                //カーソルの位置を動かす
                for (int i = 0; i < childButton.Length; i++)
                {
                    if (currentSelected == childButton[i].gameObject)
                    {
                        ChildAjustPosition(childButton[i].gameObject);
                    }
                }
            }
        }
        else
        {
            eventSystem.enabled = false;
        }
    }

    public void ChildAjustPosition(GameObject newPos)
    {
        //カーソルの位置調整
        Vector3 pos = newPos.transform.position;
        transform.position = new Vector3(pos.x - childOffset.x, pos.y - childOffset.y, pos.z - childOffset.z);

        if (currentSelected != lastSelected)
        {
            soundBox.PlayOneShot(selectSE, 1f);
        }

        //バックアップ
        lastSelected = currentSelected;
    }
}
