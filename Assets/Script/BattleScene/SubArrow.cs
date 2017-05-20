using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SubArrow : SelectArrow
{


    void Start()
    {
        
    }


    public void RebootSelectButton(Button[] newButton)
    {
        selectButton = new Button[newButton.Length];
        for (int i = 0; i < newButton.Length; i++)
        {
            selectButton[i] = newButton[i];
        }
        StartSelect();
    }
}
