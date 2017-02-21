using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    //下限と上限を決めるクラス
    [Serializable]
    public class Count
    {
        public int min;
        public int max;

        public Count(int minimm, int maximum)
        {
            min = minimm;
            max = maximum;
        }
    }

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public TextAsset[] mapText;

    //読み込み用テキストのフォルダパス
    public string defaultPath = "MapText";

    //マップ生成するときの全てのオブジェクト情報、位置情報をここに保存する
    private GameObject[,] allObjectData;

    //マス目
    private List<Vector3> gridPos = new List<Vector3>();

    //生成したマップの親になる
    private Transform mapHolder;

    //読み込んだテキストを保存
    private List<string> textBuffer = new List<string>();

    //行(横orX)軸
    private int columns;

    //列(縦orY)軸
    private int rows;


    void Start()
    {

    }


    void Update()
    {

    }


    //マス目とマップサイズを初期化・設定
    void Initialized()
    {
        //パスを獲得
        string foldaPath = Application.dataPath + "/" + defaultPath + "/";

        //mapTextの配列からランダムに取得
        FileInfo fi = new FileInfo(@foldaPath + mapText[Random.Range(0, mapText.Length)].name + ".txt");
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                //最後尾まで一行ずつ取り出す
                while (sr.Peek() >= 0)
                {
                    textBuffer.Add(sr.ReadLine());
                }
            }
        }
        //エラーが出たら
        catch (Exception e)
        {
            //エラーメッセージを表示する
            print(e.Message);
        }

        //x軸なので横
        columns = textBuffer[0].Length;

        //y軸なので縦
        rows = textBuffer.Count;

        //配列の上限が分かったので初期化をする
        allObjectData = new GameObject[rows, columns];

        gridPos.Clear();
        for (int y = 1; y < rows - 1; y++)
        {
            for (int x = 1; x < columns - 1; x++)
            {
                //左から右、上から下にテキストを読み込んでいくので、xは+軸、yは-軸に配置する
                gridPos.Add(new Vector3(x, -y, 0f));
            }
        }
    }


    //マップを生成
    void MapGenerate()
    {
        //名前をつけてオブジェクトをヒエラルキーに生成
        mapHolder = new GameObject("FieldMap").transform;

        //１文字ずつ判別し、オブジェクトを配置していく
        for (int y = 0; y < textBuffer.Count; y++)
        {
            for (int x = 0; x < textBuffer[y].Length; x++)
            {
                //１行ずつ取り出したものから、1文字ずつ取り出し比較
                char textMark = textBuffer[y][x];

                //outerWallTilesを生成
                if (textMark == 'x')
                    allObjectData[y, x] = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //wallTilesを生成
                else if (textMark == 'o')
                    allObjectData[y, x] = wallTiles[Random.Range(0, wallTiles.Length)];

                //-のとき floorTilesを生成
                else
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];

                //左から右、上から下にテキストを読み込んでいくので、xは+軸、yは-軸に配置する
                GameObject instance = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;

                //位置情報を取得し、中身を更新
                allObjectData[y, x] = instance;

                //多すぎて迷惑なので親子関連をつけて子オブジェクトにする(収納する)
                instance.transform.SetParent(mapHolder);
            }
        }
    }



    //配列用ランダムに位置を決める　*ただし何もないところにしか出現させない
    void LayoutObjectRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++)
        {
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            int randomX;
            int randomY;
            GameObject randomObject;

            //tagがFloorが出るまで繰り返す
            do
            {
                randomX = Random.Range(0, columns);
                randomY = Random.Range(0, rows);
                randomObject = allObjectData[randomY, randomX];
            }
            while (randomObject.tag != "Floor");

            //同じ場所に出現させないよう、存在を上書き
            allObjectData[randomY, randomX] = tileChoice;

            Instantiate(tileChoice, randomObject.transform.position, Quaternion.identity);
        }
    }


    //単体用ランダムに位置を決める　*ただし何もないところにしか出現させない
    void LayoutObjectRandom(GameObject tile)
    {
        int randomX;
        int randomY;
        GameObject randomObject;

        //tagがFloorが出るまで繰り返す
        do
        {
            randomX = Random.Range(0, columns);
            randomY = Random.Range(0, rows);
            randomObject = allObjectData[randomY, randomX];
        }
        while (randomObject.tag != "Floor");

        //同じ場所に出現させないよう、存在を上書き
        allObjectData[randomY, randomX] = tile;

        Instantiate(tile, randomObject.transform.position, Quaternion.identity);
    }


    //ここにアクセスするとマップ作成開始
    public void SetupScene(int level)
    {
        Initialized();
        MapGenerate();

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectRandom(enemyTiles, enemyCount, enemyCount);
        LayoutObjectRandom(exit);
    }
}
