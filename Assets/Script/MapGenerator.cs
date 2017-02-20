using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
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
    public string defaultPath = "MapText";

    private List<Vector3> gridPos = new List<Vector3>();
    private Transform mapHolder;
    private List<string> buffer = new List<string>();
    private int columns;
    private int rows;


    void Start()
    {

    }


    void Update()
    {

    }


    void InitializeList()
    {
        gridPos.Clear();

        columns = buffer[0].Length;
        rows = buffer.Count;
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPos.Add(new Vector3(x, y, 0f));
            }
        }
    }


    void MapSetup()
    {
        string foldaPath = Application.dataPath + "/" + defaultPath + "/";
        FileInfo fi = new FileInfo(@foldaPath + "test.txt");
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                //string buffer = sr.ReadToEnd();
                //string[] rows = buffer.Split('\n');
                //for(int i=0; i< rows.Length; i++)
                //{
                //    for(int j=0; j<rows[i].Length; j++)
                //    {
                //        print(rows[i][j]);
                //    }
                //}

                //一行ずつ取り出す
                while (sr.Peek() >= 0)
                {
                    buffer.Add(sr.ReadLine());
                }

                mapHolder = new GameObject("FieldMap").transform;
                //１文字ずつ判別し、オブジェクトを配置していく
                for (int y = 0; y < buffer.Count; y++)
                {
                    for (int x = 0; x < buffer[y].Length; x++)
                    {
                        GameObject toInstantiate = null;
                        char textMark = buffer[y][x];
                        //outerWallTiles
                        if (textMark == 'x')
                        {
                            toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                        }
                        //wallTiles
                        else if (textMark == 'o')
                        {
                            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                        }
                        //-のとき floorTiles
                        else
                        {
                            toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                        }
                        //左から右、上から下にテキストを読み込んでいくので、xは+軸、yは-軸に配置する
                        GameObject instance = Instantiate(toInstantiate, new Vector3(x, -y, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(mapHolder);
                    }
                }
            }
        }
        //エラーが出たら
        catch (Exception e)
        {
            print(e.Message);
        }
    }


    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPos.Count);
        Vector3 randomPos = gridPos[randomIndex];

        return randomPos;
    }


    void LayoutObjectRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++)
        {
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Vector3 randomPos = RandomPosition();
            Instantiate(tileChoice, randomPos, Quaternion.identity);
        }
    }


    //ここにアクセスするとマップ作成開始
    public void SetupScene(int level)
    {
        MapSetup();
        InitializeList();

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectRandom(enemyTiles, enemyCount, enemyCount);

        //Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
