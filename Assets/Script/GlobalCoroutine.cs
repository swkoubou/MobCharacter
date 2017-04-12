using UnityEngine;
using System.Collections;

public class GlobalCoroutine : MonoBehaviour
{
    public static void Go(IEnumerator coroutine, float time)
    {
        GameObject obj = new GameObject();     // コルーチン実行用オブジェクト作成
        obj.name = "GlobalCoroutine";

        GlobalCoroutine component = obj.AddComponent<GlobalCoroutine>();
        if (component != null)
        {
            component.StartCoroutine(component.Do(coroutine, time));
        }
    }

    IEnumerator Do(IEnumerator src, float time)
    {
        yield return new WaitForSeconds(time);

        while (src.MoveNext())
        {               
            // コルーチンの終了を待つ
            yield return null;
        }

        // コルーチン実行用オブジェクトを破棄
        Destroy(this.gameObject);              
    }
}