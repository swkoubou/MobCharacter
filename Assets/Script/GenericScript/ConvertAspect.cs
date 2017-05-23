using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//画面サイズを変更してもアスペクト比が崩れない
//CanvasのScreenSpace-Cameraの場合は逆効果
public class ConvertAspect : MonoBehaviour
{


    void Start()
    {

    }


    void Update()
    {

    }


    public static float GetWidth<T>(T x)
    {
        float width = System.Convert.ToSingle(x);
        float rate = Screen.width / width;

        //0除算でNaNエラーが出るので
        if (width == 0)
            rate = 0f;

        return rate;
    }

    public static float GetHeight<T>(T y)
    {
        float height = System.Convert.ToSingle(y);
        float rate = Screen.height / height;

        //0除算でNaNエラーが出るので
        if (height == 0)
            rate = 0f;
        

        return rate;
    }
}
