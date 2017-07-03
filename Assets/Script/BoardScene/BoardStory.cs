using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardStory : MonoBehaviour
{
    readonly public static string braver = "勇者";
    readonly public static string princess = "ヒロイン";
    readonly public static string player = "モブ";

    //public static string[] stage3Name = new string[] { braver, braver, princess, player, player, "", "", princess, braver, princess, braver, braver, player, braver, braver, princess };
    //public static string[] stage3Content = new string[] { "やっと森についたね！", "ね～皆。こっち行って少し休もうよ！" , "勇者様がいらっしゃるなら私はついていきますわ。" ,
    //    "（俺もいつか女の子とかが憧れるような存在になりたいな～）" ,"・・・・・！！！！！危ない！！！","{効果音}ドーン" ,"・・・・・・・・・・・・・・・・・・・・・・・・・・・",
    //"何！！！・・・っ勇者様！！！","イてててて・・・・みんな大丈夫かい？","大丈夫です！ですが、勇者様が・・・・","・・・・木で塞がってそっちにいけない。しょうがない。" ,
    //"モブ君悪いけどヒロインと一緒に次の村につながる洞窟の前に集合しよう！","うなずく(こく)","もし何かあっても決して叫ばない事だよ。もしかしたらさっきの木が倒れたの魔物かもしれないからね。" ,
    //"それじゃ責任もってうちのヒロイン様を頼んだよ。","私が勇者様を助けるわ！モブ君一緒にいくわよ！背中は頼んだわよ！" };

    //public static string[] stage4Name = new string[] { braver, "", princess, player, braver, braver, player, princess, player, braver };
    //public static string[] stage4Content = new string[] { "よし、やっと神殿についた。ここに最近泉水があるはず。・・・・！！" , "！！？", "ねーあれっても、もしかしてドラゴンだよね・・？" ,
    // "・・・・・・・・・・・・・・・・・・・・・・・・・・・・","そうみたいだね。ドラゴンと戦闘はさけて通れないっぽいね・・・・" ,"よし！みんな最後の戦いだ！！！！全ての力を絞り出してでもあのドラゴンを倒すぞ！！",
    //"あわわわわ・・・・・(腰が抜けそうだ)" ,"モブしっかりして！もうやるしかないのよ！！" ,"!!?ヒロインからお言葉を貰った・・・帰れないし俺、頑張る！！","よし！皆戦闘スタンバイ！行くぞぉ！！！" };


    //public static Dictionary<List<string>, string> hoge1 = new Dictionary<List<string>, string>()
    //{
    //    {new List<string>(){ {"やっと森についたね！" } }, braver },
    //    {new List<string>(){ { "ね～皆。こっち行って少し休もうよ！" } }, braver }
    //};

    public static Dictionary<int, string[]> stage3 = new Dictionary<int, string[]>()
    {
        { 0, new string[]{  braver, "やっと森についたね！" }  },
        { 1, new string[]{  braver, "ね～皆。こっち行って少し休もうよ！" }  },
        { 2, new string[]{  princess, "勇者様がいらっしゃるなら私はついていきますわ。" }  },
        { 3, new string[]{  player, "（俺もいつか女の子とかが憧れるような存在になりたいな～）" } },
        { 4, new string[]{ player,"・・・・・！！！！！危ない！！！" } },
        { 5, new string[]{ "","{効果音}ドーン" } },
        { 6, new string[]{ "","・・・・・・・・・・・・・・・・・・・・・・・・・・・"   } },
        { 7, new string[]{  princess,"何！！！・・・っ勇者様！！！" } },
        { 8, new string[]{  braver,"イてててて・・・・みんな大丈夫かい？" } },
        { 9, new string[]{ princess,"大丈夫です！ですが、勇者様が・・・・"  } },
        { 10, new string[]{braver,"・・・・木で塞がってそっちにいけない。しょうがない。"  } },
        { 11, new string[]{braver,"モブ君悪いけどヒロインと一緒に次の村につながる洞窟の前に集合しよう！" } },
        { 12, new string[]{player,"うなずく(こく)" } } ,
        { 13, new string[]{braver,"もし何かあっても決して叫ばない事だよ。もしかしたらさっきの木が倒れたの魔物かもしれないからね。"} } ,
        { 14, new string[]{braver,"それじゃ責任もってうちのヒロイン様を頼んだよ。" } },
        { 15, new string[]{princess,"私が勇者様を助けるわ！モブ君一緒にいくわよ！背中は頼んだわよ！"} } ,
    };

    public static Dictionary<int, string[]> stage4 = new Dictionary<int, string[]>()
    {
        { 0, new string[]{ braver, "よし、やっと神殿についた。ここに秘薬があるはず。・・・・！！" } },
        { 1, new string[]{ "","！！？" } },
        { 2, new string[]{ princess,"ねーあれっても、もしかしてドラゴンだよね・・？" } },
        { 3, new string[]{ player, "・・・・・・・・・・・・・・・・・・・・・・・・・・・・" } },
        { 4, new string[]{ braver,"そうみたいだね。ドラゴンと戦闘はさけて通れないっぽいね・・・・" } },
        { 5, new string[]{ braver,"よし！みんな最後の戦いだ！！！！全ての力を絞り出してでもあのドラゴンを倒すぞ！！" } },
        { 6, new string[]{ player,"あわわわわ・・・・・(腰が抜けそうだ)" } },
        { 7, new string[]{ princess,"モブしっかりして！もうやるしかないのよ！！" } },
        { 8, new string[]{ player,"!!?ヒロインからお言葉を貰った・・・帰れないし俺、頑張る！！" } },
        { 9, new string[]{ braver,"よし！皆戦闘スタンバイ！行くぞぉ！！！" } },
    };

    private int counter;

    void Start()
    {
        counter = 0;
    }

    void Update()
    {
        if (Loader.isForceEvent)
        {
            FindObjectOfType<BoardPlayer>().enabled = false;

            switch (Loader.level)
            {
                case 3:
                    Detail(stage3);
                    break;

                case 4:
                    Detail(stage4);
                    break;
            }
        }
    }

    void Detail(Dictionary<int, string[]> buffer)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            counter++;
            if (buffer.Count <= counter)
            {
                FadeSceneManager.Execute(Loader.battleSceneName);
                return;
            }
            Talk(buffer);
        }

        
    }

    void Talk(Dictionary<int, string[]> buffer)
    {
        BoardManager.instance.nameText.text = buffer[counter][0];
        BoardManager.instance.contentText.text = buffer[counter][1];
    }

    public void Execute()
    {
        if (Loader.isForceEvent)
        {
            switch (Loader.level)
            {
                case 3: 
                        Talk(stage3);
                    break;

                case 4:
                        Talk(stage4);
                    break;
            }
        }
    }
}