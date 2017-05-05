using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class FlashManager : MonoBehaviour
{
    //透過したいオブジェクト
    private GameObject targetObject;

    //透過したいオブジェクト
    private Image targetImage;

    //透過したいオブジェクト
    private Text targetText;

    //アルファ値
    private float alpha;

    //フェードする速さ
    private float flashSpeed;
    private const float FADE_SPEED = 0.5f;

    //GameObjectのiTween用
    [SerializeField, TooltipAttribute("ComponentTypeがGameObject型のときのみ使用する")]
    private float flashTime;
    private const float FLASH_TIME = 0.5f;

    //何回リピートするのか
    private int howRepeat;
    private const int HOW_REPEAT = 1;

    //リピートした回数
    private int repeatCount = 0;

    //無限ループするかしないか
    private bool infinityRepeat = false;

    //フェードを始めても良いか
    private bool isFlashStart = false;

    //フェードが終わったか
    private bool isFlashFinished = false;

    //フェードアウトが終わったか
    private bool isFlashOutFinished = false;

    //フェードインが終わったか
    private bool isFlashInFinished = false;

    //リピートも終わり、すべてのフェードが終わったか
    private static bool isAllFlashFinished = false;    

    //アルファ値がどこまでの間を通るか
    //最小値
    [SerializeField, Range(0f, 1.0f)]
    public float minAlpha = 0f;

    //最大値
    [SerializeField, Range(0f, 1.0f)]
    public float maxAlpha = 1f;

    //使用するのタイプを決める
    public enum ComponentType
    {
        GameObject,
        Image,
        Text
    }
    public ComponentType componentType;

    //フェードタイプ
    public enum FlashMode
    {
        Out,        //元から透明へ
        In          //透明から元へ
    }
    public FlashMode flashMode;


    void Start()
    {
        //使用するタイプを初期化
        switch (componentType)
        {
            case ComponentType.GameObject:
                InspectExist_GameObject();
                break;

            case ComponentType.Image:
                InspectExist_Image();
                break;

            case ComponentType.Text:
                InspectExist_Text();
                break;

            default:
                break;
        }
        InitAlpha();
    }

    //アルファ値を初期化
    void InitAlpha()
    {
        //フェードイン, フェードアウトでアルファ値の初期値を変える
        if (flashMode == FlashMode.In)
        {
            alpha = minAlpha; //元から始まる
        }
        else if (flashMode == FlashMode.Out)
        {
            alpha = maxAlpha; //透明から始まる
        }
        isFlashOutFinished = false;
        isFlashInFinished = false;
    }


    void Update()
    {
        if (isFlashStart)
        {
            //フェードが終わったかどうか
            IsFadeFinished(componentType);

            //フェードする
            switch (componentType)
            {
                case ComponentType.GameObject:
                    FadeObject();
                    break;

                case ComponentType.Image:
                    FadeImage();
                    break;
                case ComponentType.Text:
                    FadeText();
                    break;

                default:
                    break;
            }

            if (isFlashOutFinished || isFlashInFinished)
            {
                isFlashFinished = true;
            }
            else
            {
                isFlashFinished = false;
            }

            //フェードをリピートする
            if (isFlashFinished && (repeatCount < howRepeat || infinityRepeat))
            {
                isFlashFinished = false;
                repeatCount++;
                FadeRepeat();
            }
            else if (repeatCount >= howRepeat && !infinityRepeat)
            {
                if (flashMode ==  FlashMode.In)
                {
                    //見えなくする
                    switch (componentType)
                    {
                        case ComponentType.GameObject:
                            targetObject.SetActive(false);
                            break;

                        case ComponentType.Image:
                            targetImage.enabled = false;
                            break;
                        case ComponentType.Text:
                            targetText.enabled = false;
                            break;

                        default:
                            break;
                    }
                }

                isAllFlashFinished = true;
                Destroy(gameObject);
                Destroy(FindObjectOfType<FlashManager>());
            }
        }
    }

    //フェードが終わったかどうか
    void IsFadeFinished(ComponentType componentType)
    {
        if (flashMode == FlashMode.In)
        {
            if (alpha >= maxAlpha)
            {
                //これ以上上げない
                alpha = maxAlpha;
                isFlashInFinished = true;
            }
        }
        else if (flashMode == FlashMode.Out)
        {
            if (alpha <= minAlpha)
            {
                //これ以上下げない
                alpha = minAlpha;
                isFlashOutFinished = true;
            }
        }
    }

    //リピートする
    void FadeRepeat()
    {
        if (flashMode == FlashMode.In)
        {
            flashMode = FlashMode.Out;
        }
        else if (flashMode == FlashMode.Out)
        {
            flashMode = FlashMode.In;
        }
        InitAlpha();
    }

    void InspectExist_GameObject()
    {
        //存在するなら, nullじゃないなら
        if (!targetObject)
        {
            //Nullにする
            Assert.IsNull(targetObject);
        }

    }

    void InspectExist_Image()
    {
        //存在しないなら, nullじゃないなら
        if (!targetImage)
        {
            //Nullにする
            Assert.IsNull(targetImage);
        }
    }

    void InspectExist_Text()
    {
        //存在しないなら, nullじゃないなら
        if (!targetText)
        {
            //Nullにする
            Assert.IsNull(targetText);
        }
    }

    //GameObject型をフェードする
    void FadeObject()
    {
        if (flashMode == FlashMode.In)
        {
            iTween.FadeTo(targetObject, iTween.Hash("a", maxAlpha, "time", flashTime));
        }
        else if (flashMode == FlashMode.Out)
        {
            iTween.FadeTo(targetObject, iTween.Hash("a", minAlpha, "time", flashTime));
        }
    }

    //Image型をフェードする
    void FadeImage()
    {
        if (flashMode == FlashMode.In)
        {
            alpha += Time.deltaTime * flashSpeed;
        }
        else if (flashMode == FlashMode.Out)
        {
            alpha -= Time.deltaTime * flashSpeed;
        }
        Color tmp = targetImage.color;
        tmp.a = alpha;
        targetImage.color = tmp;
    }

    //Text型をフェードする
    void FadeText()
    {
        if (flashMode == FlashMode.In)
        {
            alpha += Time.deltaTime * flashSpeed;
        }
        else if (flashMode == FlashMode.Out)
        {
            alpha -= Time.deltaTime * flashSpeed;
        }
        Color tmp = targetText.color;
        tmp.a = alpha;
        targetText.color = tmp;
    }

    public static bool IsAllFlashFinished()
    {
        return isAllFlashFinished;
    }

    //フェードを始める
    public void FadeStart<T>(T obj, FlashMode flashMode, float flashSpeed, int howRepeat, bool infinityRepeat)
    {
        this.flashMode = flashMode;
        this.flashSpeed = FADE_SPEED;
        this.flashTime = FLASH_TIME;
        this.howRepeat = howRepeat;
        this.infinityRepeat = infinityRepeat;

        if (typeof(T) == typeof(GameObject))
        {
            targetObject = obj as GameObject;
            componentType = ComponentType.GameObject;
            this.flashTime = flashSpeed;
            targetObject.AddComponent<FlashManager>();
        }
        else if (typeof(T) == typeof(Image))
        {
            targetImage = obj as Image;
            componentType = ComponentType.Image;
            this.flashSpeed = flashSpeed;
            targetImage.gameObject.AddComponent<FlashManager>();
        }
        else if (typeof(T) == typeof(Text))
        {
            targetText = obj as Text;
            componentType = ComponentType.Text;
            this.flashSpeed = flashSpeed;
            targetText.gameObject.AddComponent<FlashManager>();
        }
        else
        {
            Destroy(GetComponent<FlashManager>());
        }

        isFlashStart = true;
    }

    //ここにアクセスすると開始
    public static void Excute<T>(T obj, FlashMode flashMode = FlashMode.Out, float flashSpeed = FADE_SPEED, int howRepeat = HOW_REPEAT, bool infinityRepeat = false)
    {
        GameObject flashManager = new GameObject();
        flashManager.name = "FlashManager";
        flashManager.gameObject.AddComponent<FlashManager>();

        flashManager.GetComponent<FlashManager>().FadeStart(obj, flashMode,flashSpeed, howRepeat, infinityRepeat);
    }
}
