using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraceLine : SelectArrow
{
    public GameObject traceLines;
    private SpriteRenderer[] moveLine;

    private BattlePlayer player;
    private BattleBraver braver;
    private BattlePrincess princess;

    void Start()
    {
        moveLine = traceLines.GetComponentsInChildren<SpriteRenderer>();

        player = FindObjectOfType<BattlePlayer>();
        braver = FindObjectOfType<BattleBraver>();
        princess = FindObjectOfType<BattlePrincess>();
    }

    void OnDisable()
    {
        //まず白に初期化
        foreach (var e in moveLine)
            e.color = new Color(255, 255, 255);
    }

    new void Update()
    {
        base.Update();

        //override
        int[] selectedColor = null;
        switch (BattleManager.instance.GetWhoseTurn())
        {
            case BattleManager.WhoseTurn.player:
                Vector2 pos = player.ConvertObjectToVector(player.gameObject);

                if (BattleManager.instance.GetWhatCommand() == BattleManager.WhatCommand.attack)
                {
                    if (currentSelected == selectButton[0].gameObject)
                    {
                        
                        if (pos == new Vector2(0, 0))
                            selectedColor = new int[] { 0 };
                        else if (pos == new Vector2(0, 2))
                            selectedColor = new int[] { 1 };
                        else if (pos == new Vector2(1, 0))
                            selectedColor = new int[] { 2 };
                        else if (pos == new Vector2(1, 2))
                            selectedColor = new int[] { 3 };
                        else if (pos == new Vector2(2, 0))
                            selectedColor = new int[] { 4 };
                        else if (pos == new Vector2(2, 2))
                            selectedColor = new int[] { 5 };
                    }
                    else if (currentSelected == selectButton[1].gameObject)
                    {
                        if (pos == new Vector2(0, 0) || pos == new Vector2(0, 2))
                            selectedColor = new int[] { 0, 1 };
                        else if (pos == new Vector2(1, 0) || pos == new Vector2(1, 2))
                            selectedColor = new int[] { 2, 3 };
                        else if (pos == new Vector2(2, 0) || pos == new Vector2(2, 2))
                            selectedColor = new int[] { 4, 5 };
                    }
                    else if (currentSelected == selectButton[2].gameObject)
                    {
                        selectedColor = new int[] { 8, 9 };
                    }
                }
                break;

            case BattleManager.WhoseTurn.braver:

                break;

            case BattleManager.WhoseTurn.princess:

                break;
        }
        BrightLine(selectedColor);
    }

    void BrightLine(int[] array)
    {
        //まず白に初期化
        foreach (var e in moveLine)
            e.color = new Color(255, 255, 255);

        if (array == null)
            return;

        //指定したモノだけ色を変える
        for (int i = 0; i < array.Length; i++)
        {
            moveLine[array[i]].color = new Color(250, 0, 255);
            print(moveLine[array[i]].color);
        }
    }

}
