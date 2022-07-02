using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;



public class TextDisplay : MonoBehaviour
{
    public static List<int> _plScorelist  = new List<int>();

    [SerializeField] private Text _VoiceTut;
    private String _VoiceDialog;


    //セリフを一文字づつ表示していく
    private IEnumerator Novel()
    {
        int messageCount = 0; //現在表示中の文字数
        String nowText = "";
        _VoiceTut.text = "";
        while(_VoiceDialog.Length > messageCount)
        {
            nowText += _VoiceDialog[messageCount];
            _VoiceTut.text = InsertCr(nowText);
            messageCount++; 

            yield return new WaitForSeconds(0.05f);
        }
    }

    private string InsertCr(string str) 
    {
        var sb = new System.Text.StringBuilder(str);

        var insertPos = 22;
        while (insertPos < sb.Length) {
            sb.Insert(insertPos, "\n");
            insertPos += "\n".Length + 22;
        }

        return sb.ToString();
    }

    public void StartNovel()
    {

        //途中で止められたのなら、一旦ストップする
            if(SenarioManager.instance.CheckTextSkiped){
                StopCoroutine("Novel");
                SenarioManager.instance.CheckTextSkiped = false;
            }
        
        
        if(ScoreController.NowExpresion == ScoreState.Crying){
            //途中で止められたのなら、一旦ストップする
            if(SenarioManager.instance.CheckTextSkiped){
                StopCoroutine("Novel");
                SenarioManager.instance.CheckTextSkiped = false;
            }
            //クリアに失敗したと言うデータをサーバーに送信する
            _VoiceDialog = "えーと…まぁ配信はこれくらいにしておこうかな。ちょっと私具合悪いかも…あはは…それじゃ…";
            StartCoroutine("Novel");
        }
        else{
            //途中で止められたのなら、一旦ストップする
            if(SenarioManager.instance.CheckTextSkiped){
                StopCoroutine("Novel");
                SenarioManager.instance.CheckTextSkiped = false;
            }
            TextDisplay._plScorelist.Add(ScoreTracker.instance.Score);
            _VoiceDialog = SenarioManager.AudioVoice[SenarioManager.instance._AudioNum].Senario;
            StartCoroutine("Novel");
        }
    }


    
}
