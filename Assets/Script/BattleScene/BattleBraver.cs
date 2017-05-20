using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleBraver : CommonBattleChar
{
    private Vector2 defaultBraverPos = new Vector2(1, 2);
    public Vector2 defaultOffset = new Vector2(0, 15);
    public Button[] buttonsObject;
    public string[] buttonsText;


    new void Start()
    {
        HP = 30;
        attack = 5;
        //BattleManager.instance.AddGridPos(gameObject, defaultBraverPos);
        base.Start();
    }


    void Update()
    {
        BraverTurn();
    }

    public void SetOnClick()
    {
        UnityAction[] method = new UnityAction[] { OnMoveAttackVertical};
        SetMethod(buttonsObject, buttonsText, method);
        //for (int i = 0; i < buttonsObject.Length; i++)
        //{
        //    buttonsObject[i].onClick.RemoveAllListeners();
        //    buttonsObject[i].onClick.AddListener(method[i]);
        //    buttonsObject[i].transform.FindChild("Text").GetComponent<Text>().text = buttonsText[i];
        //}
    }

    /*以下ボタン関数*/
    void OnMoveAttackVertical()
    {

    }



    //このクラスではなにもしない
    protected override void SwitchCommand(int rand)
    {
        StartCoroutine(BattleManager.instance.ChangeTurnPlayer());
        //throw new NotImplementedException();
    }
}
