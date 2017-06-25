using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class FlashingManager : MonoBehaviour
{
    //透過したいオブジェクト
    private object target;

    //フラッシュタイプ
    public enum FlashMode
    {
        Out,        //元から透明へ
        In          //透明から元へ
    }

    //オプションは引数でとらなくてもこのscriptは動作する
    public class FlashOptions
    {
        public FlashMode mode = FlashMode.Out;

        //アルファ値がどこまでの間を通るか
        //最小値
        [SerializeField, Range(0f, 1.0f)]
        public float minAlpha = 0f;

        //最大値
        [SerializeField, Range(0f, 1.0f)]
        public float maxAlpha = 1f;

        //透明以外に色をつけてフラッシュしたいとき
        public Color color = default(Color);

        //何回フラッシュするか
        public int count = 1;

        //すべてのフラッシュが終わる時間
        public float time = 1f;

        //無限ループするか
        public bool infinite = false;
    }
    public FlashOptions flashOptions = new FlashOptions();

    //1回毎の速さ
    private float flashSpeed;

    //現在のアルファ値
    private float alpha;

    //フェードを始めても良いか
    private bool isFlashStart = false;


    //アルファ値を初期化
    void Initialized()
    {
        //フェードイン, フェードアウトでアルファ値の初期値を変える
        if (flashOptions.mode == FlashMode.In)
        {
            alpha = flashOptions.minAlpha; //透明から始まる
        }
        else if (flashOptions.mode == FlashMode.Out)
        {
            alpha = flashOptions.maxAlpha; //透明から始まる
        }

        flashSpeed = flashOptions.time / flashOptions.count;

        if (flashOptions.color != default(Color))
        {
            if (target.GetType() == typeof(Image))
            {
                (target as Image).color = flashOptions.color;
            }
            else if (target.GetType() == typeof(Text))
            {
                (target as Text).color = flashOptions.color;
            }
            else if (target.GetType() == typeof(SpriteRenderer))
            {
                (target as SpriteRenderer).color = flashOptions.color;
            }
        }
    }

    void Start()
    {
        Initialized();
    }


    void Update()
    {
        try
        {
            if (flashOptions.count <= 0 && !flashOptions.infinite)
            {
                FlashingStop();
            }
            else
            {
                IsFadeFinished();
            }

            if (target.GetType() == typeof(Image))
            {
                Fading(target as Image);
            }
            else if (target.GetType() == typeof(Text))
            {
                Fading(target as Text);
            }
            else if (target.GetType() == typeof(SpriteRenderer))
            {
                Fading(target as SpriteRenderer);
            }
        }
        catch (Exception e)
        {
            print(e.Message);
            Destroy(gameObject);
        }
    }

    //フェードが終わったかどうか
    void IsFadeFinished()
    {
        if (flashOptions.mode == FlashMode.In)
        {
            if (alpha >= flashOptions.maxAlpha)
            {
                //これ以上上げない
                alpha = flashOptions.maxAlpha;
                flashOptions.count--;
                flashOptions.mode = FlashMode.Out;
            }
        }
        else if (flashOptions.mode == FlashMode.Out)
        {
            if (alpha <= flashOptions.minAlpha)
            {
                //これ以上下げない
                alpha = flashOptions.minAlpha;
                flashOptions.count--;
                flashOptions.mode = FlashMode.In;
            }
        }
    }

    //Image型をフェードする
    void Fading(Image target)
    {
        if (flashOptions.mode == FlashMode.In)
        {
            alpha += Time.deltaTime / flashSpeed;
        }
        else if (flashOptions.mode == FlashMode.Out)
        {
            alpha -= Time.deltaTime / flashSpeed;
        }
        Color tmp = target.color;
        tmp.a = alpha;
        target.color = tmp;
    }

    //Text型をフェードする
    void Fading(Text target)
    {
        if (flashOptions.mode == FlashMode.In)
        {
            alpha += Time.deltaTime / flashSpeed;
        }
        else if (flashOptions.mode == FlashMode.Out)
        {
            alpha -= Time.deltaTime / flashSpeed;
        }
        Color tmp = target.color;
        tmp.a = alpha;
        target.color = tmp;
    }

    //Sprite型をフェードする
    void Fading(SpriteRenderer target)
    {
        if (flashOptions.mode == FlashMode.In)
        {
            alpha += Time.deltaTime / flashSpeed;
        }
        else if (flashOptions.mode == FlashMode.Out)
        {
            alpha -= Time.deltaTime / flashSpeed;
        }
        Color tmp = target.color;
        tmp.a = alpha;
        target.color = tmp;
    }

    //引数のclassの中にあるフィールドと、optionsを比べ、同じ変数名があればフィールドに代入
    public void SetField(object childClass, Dictionary<string, object> options)
    {
        FieldInfo[] fields = childClass.GetType().GetFields();
        foreach (var item in options)
        {
            foreach (var field in fields)
            {
                if (field.Name.Equals(item.Key))
                {
                    field.SetValue(childClass, item.Value);
                }
            }
        }
    }

    //引数1と、それ以降がすべて一致するか
    public static bool IsTrueType(Type target, params Type[] types)
    {
        foreach (var type in types)
        {
            if (target == type)
            {
                return true;
            }
        }

        return false;
    }

    //1行で完結できるように
    public static Dictionary<string, object> Hash(params object[] args)
    {
        var dic = new Dictionary<string, object>(args.Length / 2);
        if (args.Length % 2 != 0)
        {
            Debug.LogError("引数が足りません");
            return null;
        }
        else
        {
            for (int i = 0; i < args.Length - 1; i += 2)
            {
                dic.Add((string)args[i], args[i + 1]);
            }
            return dic;
        }
    }

    //Flashingが終わったら破棄する
    private void FlashingStop()
    {
        if (flashOptions.color != default(Color))
        {
            if (target.GetType() == typeof(Image))
            {
                (target as Image).color = new Color(255, 255, 255);
            }
            else if (target.GetType() == typeof(Text))
            {
                (target as Text).color = new Color(255, 255, 255);
            }
            else if (target.GetType() == typeof(SpriteRenderer))
            {
                (target as SpriteRenderer).color = new Color(255, 255, 255);
            }
        }

        Destroy(gameObject);
    }

    //引数をグローバル変数に格納
    private void FlashingStart<T>(T target, Dictionary<string, object> options)
    {
        this.target = target;

        if (options != null)
        {
            //オプションを格納
            SetField(flashOptions, options);
        }
    }

    //ここにアクセスすると強制終了
    public void ForcedTerminate()
    {
        if (target.GetType() == typeof(Image))
        {
            (target as Image).color = new Color(255, 255, 255, 1);
        }
        else if (target.GetType() == typeof(Text))
        {
            (target as Text).color = new Color(255, 255, 255, 1);
        }
        else if (target.GetType() == typeof(SpriteRenderer))
        {
            (target as SpriteRenderer).color = new Color(255, 255, 255, 1);
        }

        Destroy(gameObject);
    }

    //ここにアクセスすると開始
    public static void Execute<T>(T target, Dictionary<string, object> options = null)
    {
        if (IsTrueType(target.GetType(), typeof(Image), typeof(Text), typeof(SpriteRenderer)))
        {
            var obj = new GameObject();
            obj.name = "FlashingManager";
            obj.AddComponent<FlashingManager>().FlashingStart(target, options);
        }
        else
        {
            Debug.LogError("引数targetの型が違います。");
            return;
        }
    }
}
