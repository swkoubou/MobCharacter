using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
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

    public Count foodCount = new Count(1, 5);
    public GameObject player;
    public GameObject braver;
    public GameObject princess;
    public GameObject exit;
    public GameObject item;
    public GameObject door;
    public GameObject[] bosses;
    public GameObject[] bossFloorTiles;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject backgroundTile;
    public TextAsset[] mapText;

    private Text errorText;

    //exitの最終的な座標を内包したオブジェクト
    private GameObject lastExit;

    //ref修飾子で使う用の、一時変数
    private GameObject tmpTile;

    //読み込み用テキストのフォルダ名
    public string foldaName = "MapText";

    //マップ生成するときの全てのオブジェクト情報、位置情報をここに保存する
    private GameObject[,] allObjectData;

    //生成したマップの親になる
    private Transform mapHolder;

    //読み込んだテキストを保存
    private List<string> textBuffer = new List<string>();

    //列(縦orY)軸
    private int rows;

    //行(横orX)軸
    private int columns;



    void Start()
    {

    }


    void Update()
    {

    }


    //マス目とマップサイズを初期化・設定
    void Initialized()
    {
        //前回のものに追加読み込みしないように初期化
        textBuffer.Clear();

        //mapTextの配列からランダムに取得
        var mapInfo = Resources.Load(foldaName +"/フロア" + Loader.level) as TextAsset;

        //try catchは使わずにusingを使う
        using (StringReader sr = new StringReader(mapInfo.text))
        {
            //最後尾まで一行ずつ取り出す
            while (sr.Peek() >= 0)
            {
                textBuffer.Add(sr.ReadLine());
            }
        }

        //y軸なので縦
        rows = textBuffer.Count;

        //x軸なので横
        columns = textBuffer[0].Length;

        //配列の上限が分かったので初期化をする
        allObjectData = new GameObject[rows, columns];
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

                //wallTilesを生成+下には床を設置
                else if (textMark == 'o')
                {
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = wallTiles[Random.Range(0, wallTiles.Length)];
                }

                //-のとき floorTilesを生成
                else if (textMark == '-')
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];

                //_のとき bossFloorTilesを生成
                else if (textMark == '_')
                    allObjectData[y, x] = bossFloorTiles[Random.Range(0, bossFloorTiles.Length)];

                //Dのとき doorを生成
                else if (textMark == 'D')
                {
                    allObjectData[y, x] = bossFloorTiles[Random.Range(0, bossFloorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = door;
                }

                //Bのとき bossを生成
                else if (textMark == 'B')
                {
                    allObjectData[y, x] = bossFloorTiles[Random.Range(0, bossFloorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = bosses[Random.Range(0, bosses.Length)];
                }

                //Gのとき exitを生成
                else if (textMark == 'G')
                {
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = exit;
                }

                //Sのとき playerを生成
                else if (textMark == 'S')
                {
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    Vector2 pos = Loader.boardPlayerPos;
                    if (pos == new Vector2(-1, -1))
                        allObjectData[y, x] = player;
                }

                //bのときbraverを生成
                else if (textMark == 'b')
                {
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = braver;
                }

                //pのときprincessを生成
                else if (textMark == 'p')
                {
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = princess;
                }

                //Iのとき itemを生成
                else if (textMark == 'I')
                {
                    allObjectData[y, x] = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject tmp = Instantiate(allObjectData[y, x], new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(mapHolder);
                    allObjectData[y, x] = item;
                }

                //0のときbackgroundTilesを生成
                else
                    allObjectData[y, x] = backgroundTile;

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
        //配置する場所がないならやめる
        int spaceCount = 0;
        foreach (var e in allObjectData)
        {
            if (e.tag == "Floor")
                spaceCount++;
        }

        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++)
        {
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            int randomX;
            int randomY;
            GameObject randomObject;

            if (spaceCount <= 0)
                return;

            //tagがFloorが出るまで繰り返す
            do
            {
                randomY = Random.Range(0, rows);
                randomX = Random.Range(0, columns);
                randomObject = allObjectData[randomY, randomX];
            }
            while (randomObject.tag != "Floor");

            spaceCount--;

            //同じ場所に出現させないよう、存在を上書き
            allObjectData[randomY, randomX] = tileChoice;

            Instantiate(tileChoice, randomObject.transform.position, Quaternion.identity);
        }
    }


    //単体用ランダムに位置を決める　*ただし何もないところにしか出現させない
    void LayoutObjectRandom(ref GameObject tile)
    {
        int randomX;
        int randomY;
        GameObject randomObject;

        //配置する場所がないならやめる
        int spaceCount = 0;
        foreach(var e in allObjectData)
        {
            if (e.tag == "Floor")
                spaceCount++;
        }
        if (spaceCount <= 0)
            return;

        //tagがFloorが出るまで繰り返す
        do
        {
            randomY = Random.Range(0, rows);
            randomX = Random.Range(0, columns);
            randomObject = allObjectData[randomY, randomX];
        }
        while (randomObject.tag != "Floor");

        //同じ場所に出現させないよう、存在を上書き
        allObjectData[randomY, randomX] = tile;

        //位置情報を取得し、中身を更新
        GameObject instance = Instantiate(tile, randomObject.transform.position, Quaternion.identity);
        tile = instance;
    }

    //Exitを取得する
    public GameObject GetExit()
    {
        return lastExit;
    }


    //ここにアクセスするとマップ作成開始
    public void SetupScene(int level)
    {
        //マス目とマップサイズを初期化・設定
        Initialized();

        //マップを生成
        MapGenerate();

        //敵は対数的に増加
        //int enemyCount = (int)Mathf.Log(level, 2f);
        int enemyCount = level+10;

        //ref修飾子を使うのでtmpを使い初期化してから
        //tmpTile = player;
        //LayoutObjectRandom(ref tmpTile);
        //tmpTile = Braver;
        //LayoutObjectRandom(ref tmpTile);
        //tmpTile = exit;
        //LayoutObjectRandom(ref tmpTile);
        //lastExit = tmpTile;

        Vector2 pos = Loader.boardPlayerPos;
        if (pos != new Vector2(-1, -1))
        {
            if (allObjectData[(int)pos.x, -(int)pos.y] == null)
            {
                tmpTile = player;
                LayoutObjectRandom(ref tmpTile);
            }
            else
            {
                allObjectData[(int)pos.x, -(int)pos.y] = player;
                GameObject instance = Instantiate(allObjectData[(int)pos.x, -(int)pos.y], new Vector3((int)pos.x, (int)pos.y, 0f), Quaternion.identity) as GameObject;
                allObjectData[(int)pos.x, -(int)pos.y] = instance;
            }
        }

        //ランダムに位置を決める
        LayoutObjectRandom(enemyTiles, enemyCount, enemyCount);
        //LayoutObjectRandom(foodTiles, foodCount.min, foodCount.max);
    }
}
