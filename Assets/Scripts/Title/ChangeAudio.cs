using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChangeAudio : MonoBehaviour
{
    [SerializeField] private AudioSource Bgm;
    [SerializeField] private Slider Bgmslider;

    private void Start()
    {
        //タイトル画面用音量設定
        new Volume(Bgm, Bgmslider).StartAudio();
    }

}








//ーーーーーーーーーーーーーーーーーーーーー－－－－－－－－－－－－－－－－－－－－－
//音量設定用のクラス
//－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－
public class Volume
{
    //一時的にゲームオブジェクトを保存する
    private Slider _MasterSlider;
    private AudioSource _BgmVolume;
    private Slider _BgmSlider;
    private AudioSource _VoiceVolume;
    private Slider _VoiceSlider;

    //静的な数値を保存する
    private static float _MasterFloat = 0.5f;
    private static float _BgmFloat = 0.5f;
    private static float _voiceFloat = 0.5f;

    //どちらのシーンかを確認する
    private AudioScene _audioScene;
    enum AudioScene
    {
        Title,
        Game
    }
    

    //－－－－－－－－－－－－－－－－－－－－－－
    //コンストラクタ
    //－－－－－－－－－－－－－－－－－－－－－－

    //タイトル画面用のコンストラクタ
    public Volume(AudioSource BgmV, Slider BgmS)
    {   
        //フィールドにシーンのオブジェクトを保存する
        this._BgmSlider = BgmS;
        this._BgmVolume = BgmV;

        //前シーンのスライダー情報を引き継ぐ
        this._BgmSlider.value = _BgmFloat;
        this._BgmVolume.volume = _BgmFloat;

        //シーンタイプの変更
        this._audioScene = AudioScene.Title;
    } 

    //ゲーム画面用のコンストラクタ
    public Volume(AudioSource BgmV, Slider BgmS, AudioSource VoiceV, Slider VoiceS, Slider MasterS)
    {
        //フィールドにシーンのオブジェクトを保存する
        this._MasterSlider = MasterS;
        this._BgmVolume    = BgmV;
        this._BgmSlider    = BgmS;
        this._VoiceVolume  = VoiceV;
        this._VoiceSlider  = VoiceS;

        //前シーンのスライダー情報を引き継ぐ
        this._MasterSlider.value = _MasterFloat;
        this._BgmVolume.volume = _BgmFloat;
        this._BgmSlider.value = _BgmFloat;
        this._VoiceVolume.volume = _voiceFloat;
        this._VoiceSlider.value = _voiceFloat;

        //シーンタイプの変更
        this._audioScene = AudioScene.Game;
    }
    



    //－－－－－－－－－－－－－－－－－－－－－－
    //スライドが変動した時
    //－－－－－－－－－－－－－－－－－－－－－－
    public void ChangeMaster(float value)
    {
        this._BgmVolume.volume = this._MasterSlider.value;
        this._VoiceVolume.volume = this._MasterSlider.value;
        this._BgmSlider.value = this._MasterSlider.value;
        this._VoiceSlider.value = this._MasterSlider.value;
        _MasterFloat = this._MasterSlider.value;
    }
    public void ChangeBGM(float value)
    {
        this._BgmVolume.volume = this._BgmSlider.value;
        _BgmFloat = this._BgmSlider.value;
    }
    public void ChangeVoice(float value)
    {
        this._VoiceVolume.volume = this._VoiceSlider.value;
        _voiceFloat = this._VoiceSlider.value;
    }

    

    //－－－－－－－－－－－－－－－－－－－－－－
    //音量とスライダーを関連付ける
    //－－－－－－－－－－－－－－－－－－－－－－
    public void StartAudio()
    {
        this._BgmSlider.onValueChanged.AddListener(ChangeBGM);

        //ゲーム画面なら実行する
        if(this._audioScene == AudioScene.Game){
            this._MasterSlider.onValueChanged.AddListener(ChangeMaster);
            this._VoiceSlider.onValueChanged.AddListener(ChangeVoice);
        }
    }
}
