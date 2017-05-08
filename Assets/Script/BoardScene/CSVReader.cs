using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public TextAsset[] csv;

    //読み込み用テキストのフォルダ名 *Build後にフォルダごと直接コピーする必要あり
    public string foldaName = "ExcelTalk";

    //喋る人物の名前を保存
    private List<string> talkingName = new List<string>();

    //喋る内容を保存
    private List<string> talkingContents = new List<string>();


    void Start()
    {
        Initialized();
    }


    void Update()
    {

    }


    void Initialized()
    {
        //ファイルの読み込み
        var excelInfo = Resources.Load(foldaName + "/" + csv[0].name) as TextAsset;

        //このままだと文字化けするのでメモ帳でUTF-8に文字コードを変えてから上書きする必要あり
        using (StringReader sr = new StringReader(excelInfo.text))
        {
            //最後尾まで一行ずつ取り出す
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');

                for (int i = 0; i < values.Length; i += 2)
                {
                    talkingName.Add(values[i]);
                    talkingContents.Add(values[i + 1]);
                }
            }

            for(int i=0; i < talkingName.Count; i++)
            {
                //print(talkingName[i] + ":" + talkingContents[i]);
            }
        }
    }
}
