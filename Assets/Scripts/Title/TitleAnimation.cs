using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    //アニメーターの取得
    [SerializeField] private Animator _TitleSettingsAnim;
    [SerializeField] private Animator _TitleGameAnim;


    private void Start()
    {
        OpenTitleGame();
    }

    //－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－
    //ホーム画面の一番下のボタン
    //－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－

    //設定ボタンを開く
    public void OpenTitleSettings()
    {
        //・・・・・・必要パネルを開く・・・・・・
        _TitleSettingsAnim.SetBool("OpenSetting", true);

        //・・・・開いていたパネルを閉じる・・・・
        _TitleGameAnim.SetBool("OpenGame", false);
    }

    //配信を開く
    public void OpenTitleGame()
    {
        //・・・・・・必要パネルを開く・・・・・・
        _TitleGameAnim.SetBool("OpenGame", true);

        //・・・・開いていたパネルを閉じる・・・・
        _TitleSettingsAnim.SetBool("OpenSetting", false);
    }

}
