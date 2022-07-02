using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{

    private GameObject commentWindowList;
    private GameObject senarioWindowList;

    private GameObject ChildObjPre;



    private void addOldComment()
    {

        //設定画面を表示させる度に通過したコメントを表示させる
        for(int i = 0; i < MainObj.GetComponent<SenarioManager>().ListCommnet.Count; i++){
        GameObject childObj = Instantiate(ChildObjPre) as GameObject;
        childObj.transform.GetChild(0).GetComponent<Text>().text =
        MainObj.GetComponent<SenarioManager>().ListCommnet[MainObj.GetComponent<SenarioManager>().ListCommnet.Count - i - 1];
        childObj.transform.SetParent(commentWindowList.transform, false);
        }
    }

    private void addOldSenario()
    {
        int nownum;
        //設定画面に表示し終わったセリフを表示していく。
        for(int i = 0; i < MainObj.GetComponent<SenarioManager>()._AudioNum; i++){
            
            nownum = MainObj.GetComponent<SenarioManager>()._AudioNum - i - 1;
            if(checkEasyMode(nownum)){
                GameObject childObj = Instantiate(ChildObjPre) as GameObject;
                childObj.transform.GetChild(0).GetComponent<Text>().text =
                SenarioManager.AudioVoice[nownum].Senario;
                childObj.transform.SetParent(senarioWindowList.transform, false);
            }
        }
    }

    private bool checkEasyMode(int num)
    {
        if(GetLevel.Difficulty == GameLevel.Easy){
            switch(num){
                case 20:
                    return false;
                    
                case 21:
                    return false;
                    
                case 52:
                    return false;
                    
                case 53:
                    return false;
                    
                case 54:
                    return false;
                    
                case 55:
                    return false;
                    
                default:
                    return true;
                    
            }
        }
        return true;
    }


    //音量設定用の物
    [SerializeField] private AudioSource Bgm;
    [SerializeField] private AudioSource Speach;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider Bgmslider;
    [SerializeField] private Slider Speachslider;

    //表示・非表示のボタンリスト
    [SerializeField] private GameObject _SettingWindow;
    [SerializeField] private GameObject MainObj;
    //再生する為のオーディオソースを取得
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource BGMaudioSource;
    private void Start()
    {
        //ゲーム画面用音量設定
        new Volume(Bgm, Bgmslider, Speach, Speachslider, MasterSlider).StartAudio();

        //リストの親要素を取得する
        this.commentWindowList = _SettingWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        this.senarioWindowList = _SettingWindow.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        
        ChildObjPre = Resources.Load<GameObject>("Stuff/Image");

        this._SettingWindow.SetActive(false);
    }
    public void ShowSettingWindow() //設定画面を表情する
    {
        //ノベルモードならいつでも出来る
        //オーディオが0秒地点でないなら一時停止できる
        if((audioSource.time != 0.0f) || (GetLevel.Difficulty == GameLevel.Novel)){
            audioSource.Pause();
            BGMaudioSource.Pause();
            this._SettingWindow.SetActive(true);
            addOldComment();
            addOldSenario();
        }

    }
    public void HideSettingWindow() //設定画面を非表示にする
    {
        foreach(Transform c in this.commentWindowList.transform){   //子要素の全消去
            GameObject.Destroy(c.gameObject);
        }
        foreach(Transform c in this.senarioWindowList.transform){   //子要素の全消去
            GameObject.Destroy(c.gameObject);
        }

        audioSource.Play();
        BGMaudioSource.Play();
        this._SettingWindow.SetActive(false);
    }

    public void BackToTitle()   //タイトル画面に戻る
    {
        SceneManager.LoadScene("TitleScene");
    }
    
}
