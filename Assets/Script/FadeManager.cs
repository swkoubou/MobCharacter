using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    //どの色でフェードするか
    public Color fadeColor = Color.black;

    //アルファ値
    private float alpha;

    //フェードしているか否か
    public bool isFading = false;

    //フェードが終わったか否か
    public bool isFadeFinished = false;

    //遷移するシーンの番号
    private int sceneIndex = -1;

    //遷移するシーンの名前
    private string sceneName = null;

    //値が大きいほど早くフェードする
    private float fadeSpeed = 0.5f;    

    //コルーチンの待ち時間
    private float waitForSeconds = 0f;


    public enum FadeMode
    {
        none = -1,  //すぐに遷移する
        open,       //黒から白へ+遷移しない
        close       //白から黒へ+遷移する
    }
    public FadeMode fadeMode = FadeMode.open;


    void Start()
    {
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

    //ここにシーン番号を引数にしてアクセスするとフェードが始まる
    public void FadeStart(int sceneIndex, float fadeSpeed = 0.5f, float waitForSeconds = 0f)
    {
        //フェード中に干渉しないように
        if (this.sceneIndex == -1 && this.sceneName == null)
        {
            this.sceneIndex = sceneIndex;           //シーン遷移の番号
            this.fadeSpeed = fadeSpeed;             //フェードする速さ
            this.waitForSeconds = waitForSeconds;   //シーン遷移までの時間
            StartCoroutine(FadeStop());
        }
    }

    //ここにシーン名前を引数にしてアクセスするとフェードが始まる
    public void FadeStart(string sceneName, float fadeSpeed = 0.5f, float waitForSeconds = 0f)
    {
        //フェード中に干渉しないように
        if (this.sceneIndex == -1 && this.sceneName == null)
        {
            this.sceneName = sceneName;             //シーン遷移の名前
            this.fadeSpeed = fadeSpeed;             //フェードする速さ
            this.waitForSeconds = waitForSeconds;   //シーン遷移までの時間
            StartCoroutine(FadeStop());
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
}
