using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class SendManager : MonoBehaviour
{

    

    //ーーーーーーーーーーーーーーーーーーーー
    //ユーザーデータの送信
    //ーーーーーーーーーーーーーーーーーーーー
    public void UpdateUserData()
    {

        if(TextDisplay._plScorelist != null)
        {
            //データを送った数を保存
            if(!PlayerPrefs.HasKey("JsonNum"))  PlayerPrefs.SetInt("JsonNum", 0);
            else                                PlayerPrefs.SetInt("JsonNum", PlayerPrefs.GetInt("JsonNum") + 1);
            
            //取得データのJson化
            string SendJson = "{";
            int i = 0;
            foreach (var item in TextDisplay._plScorelist)
            {
                SendJson += "\"" + i.ToString() + "\":" + "\"" + item.ToString() + "\"";
                if(i != TextDisplay._plScorelist.Count - 1) SendJson += ",";
                i++;
            }
            SendJson += "}";

            //送信するデータ
            var updateDataDict = new Dictionary<string, string>()
            {
                {GetLevel.Difficulty.ToString() + PlayerPrefs.GetInt("JsonNum").ToString(),
                SendJson}
            };

            //UpdateUserDataRequestのインスタンスを作成
            var request = new UpdateUserDataRequest
            {
                Data = updateDataDict,
                Permission = UserDataPermission.Private
            };

            PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnErrorUpdatingPlayerData);
        }
    }

     //ユーザー(プレイヤー)データの更新に成功
    private void OnSuccessUpdatingPlayerData(UpdateUserDataResult result) {
        Debug.Log($"ユーザー(プレイヤー)データの更新に成功しました");
        
    }

    //ユーザー(プレイヤー)データの更新に失敗
    private void OnErrorUpdatingPlayerData(PlayFabError error) {
        Debug.LogWarning($"ユーザー(プレイヤー)データの更新に失敗しました : {error.GenerateErrorReport()}");
    }
}
