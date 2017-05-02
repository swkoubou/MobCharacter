using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeSceneManager : MonoBehaviour
{
    //どの色でフェードするか
    public Color fadeColor = Color.black;

    //アルファ値
    private float alpha;

    //フェードしているか否か
    [HideInInspector]
    private static bool isFading = false;

    //フェードが終わったか否か
    [HideInInspector]
    private static bool isFadeFinished = true;

    //遷移するシーンの番号
    private int sceneIndex = -1;

    //遷移するシーンの名前
    private string sceneName = null;

    //コルーチンの待ち時間
    private float waitForSeconds;
    private const float WAIT_FOR_SECONDS = 0f;

    //値が大きいほど早くフェードする
    private float fadeSpeed;
    private const float FADE_SPEED = 0.5f;


    private enum FadeMode
    {
        none = -1,  //すぐに遷移する
        open,       //黒から透明へ+遷移しない
        close       //透明から黒へ+遷移する
    }
    private FadeMode fadeMode = FadeMode.open;


    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (isFading)
        {
            //フェードインして、現在のシーンが始まる
            if (fadeMode == FadeMode.open)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                if (alpha <= 0)
                {
                    FadeFinished(alpha);
                }
            }
            //フェードアウトして、次のシーンへ遷移する
            else if (fadeMode == FadeMode.close)
            {
                alpha += Time.deltaTime * fadeSpeed;
                if (alpha >= 1)
                {
                    FadeFinished(alpha);
                }
            }
            //noneなら
            else
            {
                TransitionScene();
            }
        }
    }

    //シーンが読み込まれる度に呼ばれる
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeMode = FadeMode.open;
        sceneIndex = -1;
        sceneName = null;
        isFading = false;
        isFadeFinished = false;

        if (fadeMode == FadeMode.open)
        {
            alpha = 1f; //黒から始まる
            isFading = true;
        }
        else if (fadeMode == FadeMode.close)
        {
            alpha = 0f; //透明から始まる
        }
    }


    IEnumerator FadeStop()
    {
        //指定秒数待つ
        yield return new WaitForSeconds(waitForSeconds);

        //Update関数の処理を開始する
        isFading = true;
    }

    void FadeFinished(float alpha)
    {
        isFading = false;

        //画面が黒ならシーン遷移する
        if (alpha >= 1)
        {
            //シーン遷移
            TransitionScene();
        }

        isFadeFinished = true;
    }

    void TransitionScene()
    {
        if (sceneName == "Quit()")
        {
            Application.Quit();
        }
        else if (sceneIndex != -1)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else if (sceneName != null)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    void OnGUI()
    {
        fadeColor.a = alpha;
        GUI.color = fadeColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
    }

    public static bool IsFading()
    {
        return isFading;
    }

    public static bool IsFadeFinished()
    {
        return isFadeFinished;
    }

    //必要な変数に値を格納しフェードを始める
    void FadeStart<T>(T scene, float waitForSeconds, float fadeSpeed)
    {
        if (isFadeFinished)
        {
            if (typeof(T) == typeof(int))
                sceneIndex = System.Convert.ToInt32(scene);
            else if (typeof(T) == typeof(string))
                sceneName = scene as string;

            this.waitForSeconds = waitForSeconds;   //シーン遷移までの時間
            this.fadeSpeed = fadeSpeed;             //フェードする速さ
            fadeMode = FadeMode.close;
            StartCoroutine(FadeStop());
        }
    }

    //ここにアクセスすると実行
    public static void Execute<T>( T scene, float waitForSeconds = WAIT_FOR_SECONDS, float fadeSpeed = FADE_SPEED)
    {
        if (!FindObjectOfType<FadeSceneManager>())
        {
            GameObject fadeManager = new GameObject();
            fadeManager.name = "FadeSceneManager";
            DontDestroyOnLoad(fadeManager);
            fadeManager.AddComponent<FadeSceneManager>();
        }

        FindObjectOfType<FadeSceneManager>().FadeStart(scene, waitForSeconds, fadeSpeed);
    }

    //不要になったら削除する
    public static void Destroy()
    {
        if(FindObjectOfType<FadeSceneManager>())
            Destroy(FindObjectOfType<FadeSceneManager>().gameObject);
    }
}
