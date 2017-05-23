using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SubArrow : SelectArrow
{


    void Start()
    {

    }


    //ボタンのOnClikeを再設定し
    public void RebootSelectButton(Button[] newButton)
    {
        selectButton = new Button[newButton.Length];
        for (int i = 0; i < newButton.Length; i++)
        {
            selectButton[i] = newButton[i];
        }
        //StartSelect();
    }
}
