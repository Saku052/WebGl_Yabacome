using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameLevel   //ゲームレベルの設定
{
    Novel,
    Easy,
    Normal,
    Hard
}


public class GetLevel : MonoBehaviour
{
    static public GameLevel Difficulty{
        get;
        private set;
    }   //ゲームの難易度をここに保存する

    static public bool Tutorial = false;

    public void ClickedTutorial()
    {
        GetLevel.Difficulty = GameLevel.Easy;
        GetLevel.Tutorial = true;
        gameObject.GetComponent<PlayFabLogin>().GetTitleData();
    }
    public void ClickedNovel()
    {
        GetLevel.Difficulty = GameLevel.Novel;
        gameObject.GetComponent<PlayFabLogin>().GetTitleData();
    }
    public void ClickedEasy()
    {
        GetLevel.Difficulty = GameLevel.Easy;
        gameObject.GetComponent<PlayFabLogin>().GetTitleData();
    }
    public void ClickedNormal()
    {
        GetLevel.Difficulty = GameLevel.Normal;
        gameObject.GetComponent<PlayFabLogin>().GetTitleData();
    }

    public void ClickedHard()
    {
        GetLevel.Difficulty = GameLevel.Hard;
        gameObject.GetComponent<PlayFabLogin>().GetTitleData();
    }

}
