using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraceLine : SelectArrow
{
    public GameObject traceLines;
    private SpriteRenderer[] moveLine;
    private Color lineColor;
    private readonly Color movedColor = new Color(255, 0, 255);
    private readonly Color notMovedColor = Color.blue;

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
        try
        {
            //まず白に初期化
            foreach (var e in moveLine)
            {
                e.color = new Color(255, 255, 255);
                e.enabled = false;
            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    new void Update()
    {
        base.Update();

        //override
        int[] selectedColor = null;
        switch (BattleManager.instance.GetWhoseTurn())
        {
            case BattleManager.WhoseTurn.player:
                switch (BattleManager.instance.GetWhatCommand())
                {
                    case BattleManager.WhatCommand.attack:
                        selectedColor = AttackCasePlayer();
                        break;

                    case BattleManager.WhatCommand.move:
                        selectedColor = MoveCaseALL(player.gameObject);
                        break;

                }
                break;

            case BattleManager.WhoseTurn.braver:
                switch (BattleManager.instance.GetWhatCommand())
                {
                    case BattleManager.WhatCommand.attack:
                        selectedColor = AttackCaseBraver();
                        break;

                    case BattleManager.WhatCommand.move:
                        selectedColor = MoveCaseALL(braver.gameObject);
                        break;
                }
                break;

            case BattleManager.WhoseTurn.princess:
                switch (BattleManager.instance.GetWhatCommand())
                {
                    case BattleManager.WhatCommand.attack:
                        selectedColor = AttackCasePrincess();
                        break;

                    case BattleManager.WhatCommand.move:
                        selectedColor = MoveCaseALL(princess.gameObject);
                        break;
                }
                break;
        }
        BrightLine(selectedColor);
    }

    int[] AttackCasePlayer()
    {
        int[] selectedColor = null;
        Vector2 pos = player.ConvertObjectToVector(player.gameObject);

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

            lineColor = notMovedColor;
        }
        else if (currentSelected == selectButton[1].gameObject)
        {
            if (pos == new Vector2(0, 0) || pos == new Vector2(0, 2))
                selectedColor = new int[] { 0, 1 };
            else if (pos == new Vector2(1, 0) || pos == new Vector2(1, 2))
                selectedColor = new int[] { 2, 3 };
            else if (pos == new Vector2(2, 0) || pos == new Vector2(2, 2))
                selectedColor = new int[] { 4, 5 };

            lineColor = movedColor;
        }
        else if (currentSelected == selectButton[2].gameObject)
        {
            if (pos == new Vector2(0, 0) || pos == new Vector2(2, 2))
                selectedColor = new int[] { 6, 7 };
            else if (pos == new Vector2(0, 2) || pos == new Vector2(2, 0))
                selectedColor = new int[] { 8, 9 };

            lineColor = movedColor;
        }

        return selectedColor;
    }

    int[] AttackCaseBraver()
    {
        int[] selectedColor = null;
        Vector2 pos = braver.ConvertObjectToVector(braver.gameObject);

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

            lineColor = notMovedColor;
        }
        else if (currentSelected == selectButton[1].gameObject)
        {
            if (pos == new Vector2(1, 0))
                selectedColor = new int[] { 2, 6 };
            else if (pos == new Vector2(2, 0))
                selectedColor = new int[] { 4, 16 };
            else if (pos == new Vector2(1, 2))
                selectedColor = new int[] { 3, 9 };
            else if (pos == new Vector2(2, 2))
                selectedColor = new int[] { 5, 15 };

            lineColor = movedColor;
        }
        else if (currentSelected == selectButton[2].gameObject)
        {
            if (pos == new Vector2(0, 0))
                selectedColor = new int[] { 0, 14 };
            else if (pos == new Vector2(1, 0))
                selectedColor = new int[] { 2, 8 };
            else if (pos == new Vector2(0, 2))
                selectedColor = new int[] { 1, 17 };
            else if (pos == new Vector2(1, 2))
                selectedColor = new int[] { 3, 7 };

            lineColor = movedColor;
        }

        return selectedColor;
    }

    int[] AttackCasePrincess()
    {
        int[] selectedColor = null;
        Vector2 pos = princess.ConvertObjectToVector(princess.gameObject);

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

            lineColor = notMovedColor;
        }
        else if (currentSelected == selectButton[1].gameObject)
        {
            if (pos == new Vector2(1, 0))
                selectedColor = new int[] { 14 };
            else if (pos == new Vector2(2, 0))
                selectedColor = new int[] { 8 };
            else if (pos == new Vector2(1, 2))
                selectedColor = new int[] { 17 };
            else if (pos == new Vector2(2, 2))
                selectedColor = new int[] { 7 };

            lineColor = notMovedColor;
        }
        else if (currentSelected == selectButton[2].gameObject)
        {
            if (pos == new Vector2(0, 0))
                selectedColor = new int[] { 6 };
            else if (pos == new Vector2(1, 0))
                selectedColor = new int[] { 16 };
            else if (pos == new Vector2(0, 2))
                selectedColor = new int[] { 9 };
            else if (pos == new Vector2(1, 2))
                selectedColor = new int[] { 15 };

            lineColor = notMovedColor;
        }

        return selectedColor;
    }

    int[] MoveCaseALL(GameObject obj)
    {
        int[] selectedColor = null;
        Vector2 pos = player.ConvertObjectToVector(obj);

        if (currentSelected == selectButton[0].gameObject)
        {
            if (pos == new Vector2(1, 0))
                selectedColor = new int[] { 10 };
            else if (pos == new Vector2(2, 0))
                selectedColor = new int[] { 11 };
            else if (pos == new Vector2(1, 2))
                selectedColor = new int[] { 12 };
            else if (pos == new Vector2(2, 2))
                selectedColor = new int[] { 13 };
        }
        else if (currentSelected == selectButton[1].gameObject)
        {
            if (pos == new Vector2(0, 0))
                selectedColor = new int[] { 10 };
            else if (pos == new Vector2(1, 0))
                selectedColor = new int[] { 11 };
            else if (pos == new Vector2(0, 2))
                selectedColor = new int[] { 12 };
            else if (pos == new Vector2(1, 2))
                selectedColor = new int[] { 13 };
        }

        lineColor = movedColor;
        return selectedColor;
    }

    void BrightLine(int[] array)
    {
        OnDisable();

        if (array == null)
            return;

        //指定したモノだけ色を変える
        for (int i = 0; i < array.Length; i++)
        {
            moveLine[array[i]].color = lineColor;
            moveLine[array[i]].enabled = true;
        }

        if (!FindObjectOfType<FlashingManager>())
        {
            for (int i = 0; i < array.Length; i++)
                FlashingManager.Execute(moveLine[array[i]], FlashingManager.Hash("minAlpha", 0.3f, "count", 2));
        }
    }

    //ここにアクセスすると停止する
    public new void StopSelect()
    {
        isStartSelect = false;
        GetComponent<Image>().enabled = false;
        GetComponent<TraceLine>().enabled = false;
    }
}
