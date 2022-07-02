using PlayFab;
using PlayFab.Json;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayFabLogin : MonoBehaviour
{
    //リストを初期化して追加
    public static List<ScriptSinario> VoiceSenario = new List<ScriptSinario>();
    public static List<ScriptSinario> TutorialSenario = new List<ScriptSinario>();
    public static List<Comments> TutorialComment = new List<Comments>();
    public static List<Comments> EasyComment = new List<Comments>();
    public static List<int> TutComNum = new List<int>();
    public static List<int> EasyComNum = new List<int>();
    public static List<int> NormalComNum = new List<int>();
    public static List<int> HardComNum = new List<int>();
    //特殊な変数
    private static ScriptSinario _voiSenario = null;
    private static ScriptSinario _tutSenario = null;
    private static Comments _EzComment = null;

    
    private bool _shouldCreateAccount;    //アカウントを作成するか
    private string _customID;             //ログイン時に使うID

    [SerializeField] private GameObject _FailLoging;
    [SerializeField] private GameObject _NowLoading;

    //インスタンス化
    public static PlayFabLogin instance;

    
    private void Awake()    //ゲーム開始時の関数
    {
        if(instance == null){
            instance = this;
         }
        _FailLoging.SetActive(false);

        if(VoiceSenario.Count == 0){
            Login();
        }else{
            TutorialSenario.Clear();
            VoiceSenario.Clear();
            TutComNum.Clear();
            EasyComment.Clear();
            EasyComNum.Clear();
            NormalComNum.Clear();
            HardComNum.Clear();
            _tutSenario = null;
            _voiSenario = null;
            _EzComment = null;
            GetLevel.Tutorial = false;

            StartCoroutine(NowLoading(LoadState.Loading));
        }
    }


    //=================================================================================
    //ログイン処理
    //=================================================================================

        private void Login() {  //ログイン処理
            _customID = LoadCustomID();
            var request = new LoginWithCustomIDRequest { CustomId = _customID,  CreateAccount = _shouldCreateAccount};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }

        //ログイン成功
        private void OnLoginSuccess(LoginResult result){
            //アカウントを作成しようとしたのに、IDが既に使われていて、出来なかった場合
            if (_shouldCreateAccount && !result.NewlyCreated) {
                Debug.LogWarning($"CustomId : {_customID} は既に使われています。");
                Login();//ログインしなおし
                return;
            }
            
            //アカウント作成時にIDを保存
            if (result.NewlyCreated) {
                GetLevel.Tutorial = true;
                SaveCustomID();
            }
            
            StartCoroutine(NowLoading(LoadState.Loading));
            Debug.Log($"PlayFabのログインに成功\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nアカウントを作成したか : {result.NewlyCreated}");
        }

          //ログイン失敗
        private void OnLoginFailure(PlayFabError error){
            _FailLoging.SetActive(true);
            Debug.LogError($"PlayFabのログインに失敗\n{error.GenerateErrorReport()}");
        }



    //=================================================================================
    //タイトルデータの取得
    //=================================================================================
    public void GetTitleData() //ボイスやコメント等のデータを取得してリストに追加
    {
        StartCoroutine(NowLoading(LoadState.StartGame));

        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),    //タイトルデータを呼び出す関数
        result =>
        {
            if(result.Data.ContainsKey("MainScript")){
                var _mainScript = 
                PlayFabSimpleJson.DeserializeObject<List<MainScript>>(result.Data["MainScript"]);

                foreach(var _script in _mainScript){    //ループでリストに音声関連データを追加していく
                    
                    _voiSenario = new ScriptSinario
                    {
                        ID = _script.ID,
                        Senario = _script.Script
                    };
                
                    VoiceSenario.Add(_voiSenario);
                    EasyComNum.Add(_script.Easy);
                    NormalComNum.Add(_script.Normal);
                    HardComNum.Add(_script.Hard);

                }

            }

            if(GetLevel.Tutorial)   //チュートリアル関連のデータを取得
            {
                if(result.Data.ContainsKey("TutorialScript")){
                    var _turorialScript = 
                    PlayFabSimpleJson.DeserializeObject<List<TutorialScript>>(result.Data["TutorialScript"]);
                    

                    foreach(var _script in _turorialScript){    //ループでリストに音声関連データを追加していく
                        
                        _tutSenario = new ScriptSinario
                        {
                            ID = _script.ID,
                            Senario = _script.Script
                        };
                    
                        TutorialSenario.Add(_tutSenario);
                        TutComNum.Add(_script.Comment);
                    }

                }

                if(result.Data.ContainsKey("TutorialComment")){
                        var _TutComment = 
                        PlayFabSimpleJson.DeserializeObject<List<Comments>>(result.Data["TutorialComment"]);

                        foreach(var _ecomment in _TutComment){     //ループでテキスト関連のデータを追加していく
                            
                            _EzComment = new Comments
                            {
                                Comment = _ecomment.Comment,
                                CommentAt = _ecomment.CommentAt,
                                Damage = _ecomment.Damage
                            };

                            TutorialComment.Add(_EzComment);
                        }
                    }
            }
            
            switch(GetLevel.Difficulty){
                case GameLevel.Novel:
                    if(result.Data.ContainsKey("EasyComment")){
                        var _easyComment = 
                        PlayFabSimpleJson.DeserializeObject<List<Comments>>(result.Data["EasyComment"]);

                        foreach(var _ecomment in _easyComment){     //ループでテキスト関連のデータを追加していく
                            
                            _EzComment = new Comments
                            {
                                Comment = _ecomment.Comment,
                                CommentAt = _ecomment.CommentAt,
                                Damage = _ecomment.Damage
                            };

                            EasyComment.Add(_EzComment);
                        }
                    }
                    SceneManager.LoadScene("mainScene");    //シーン移行
                    break;

                case GameLevel.Easy:  //難易度イージーだった場合。コメントリストにイージーを代入する
                    if(result.Data.ContainsKey("EasyComment")){
                        var _easyComment = 
                        PlayFabSimpleJson.DeserializeObject<List<Comments>>(result.Data["EasyComment"]);

                        foreach(var _ecomment in _easyComment){     //ループでテキスト関連のデータを追加していく
                            
                            _EzComment = new Comments
                            {
                                Comment = _ecomment.Comment,
                                CommentAt = _ecomment.CommentAt,
                                Damage = _ecomment.Damage
                            };

                            EasyComment.Add(_EzComment);
                        }
                    }
                    SceneManager.LoadScene("mainScene");    //シーン移行
                    break;
                case GameLevel.Normal:  //難易度ノーマルだった場合。コメントリストにノーマルを代入する
                    if(result.Data.ContainsKey("NormalComment")){
                        var _easyComment = 
                        PlayFabSimpleJson.DeserializeObject<List<Comments>>(result.Data["NormalComment"]);

                        foreach(var _ecomment in _easyComment){     //ループでテキスト関連のデータを追加していく
                            
                            _EzComment = new Comments
                            {
                                Comment = _ecomment.Comment,
                                CommentAt = _ecomment.CommentAt,
                                Damage = _ecomment.Damage
                            };

                            EasyComment.Add(_EzComment);
                        }
                    }
                    SceneManager.LoadScene("mainScene");    //シーン移行
                    break;
                case GameLevel.Hard:
                    if(result.Data.ContainsKey("HardComment")){
                        var _easyComment = 
                        PlayFabSimpleJson.DeserializeObject<List<Comments>>(result.Data["HardComment"]);

                        foreach(var _ecomment in _easyComment){     //ループでテキスト関連のデータを追加していく
                            
                            _EzComment = new Comments
                            {
                                Comment = _ecomment.Comment,
                                CommentAt = _ecomment.CommentAt,
                                Damage = _ecomment.Damage
                            };

                            EasyComment.Add(_EzComment);
                        }
                    }
                    SceneManager.LoadScene("mainScene");
                    break;
            }


        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }


    //=================================================================================
    //ログイン失敗画面時のリトライ画面
    //=================================================================================
    public void Reload()
    {
        SceneManager.LoadScene("TitleScene");
    }


    //=================================================================================
    //ローディング画面を少しづつ透明にする
    //=================================================================================
    enum LoadState{
        Loading,
        StartGame
    }
    private IEnumerator NowLoading(LoadState load)
    {   
        float i;
        Image loading =  _NowLoading.GetComponent<Image>();
        switch(load){
            case LoadState.Loading:
                i = 1.0f;
                while(i > 0.0f){
                    i = i - 0.02f;
                    loading.SetOpacity(i);
                    yield return new WaitForSeconds(0.001f);
                }
                break;
            case LoadState.StartGame:
                i = 0.0f;
                while(i < 1.0f){
                    i = i + 0.05f;
                    loading.SetOpacity(i);
                    yield return new WaitForSeconds(0.001f);
                }
                break;
        }
    }


    //=================================================================================
    //カスタムIDの取得
    //=================================================================================

    //IDを保存する時のKEY
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";
    
    //IDを取得
    private string LoadCustomID() {
        //IDを取得
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //保存されていなければ新規生成
        _shouldCreateAccount = string.IsNullOrEmpty(id);
        return _shouldCreateAccount ? GenerateCustomID() : id;
    }

    //IDの保存
    private void SaveCustomID() {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _customID);
    }
    
    //=================================================================================
    //カスタムIDの生成
    //=================================================================================
    
    //IDに使用する文字
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    //IDを生成する
    private string GenerateCustomID() {
        int idLength = 32;//IDの長さ
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //ランダムにIDを生成
        for (int i = 0; i < idLength; i++){
        stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }


}


public class MainScript //リストクラスを定義
{
    public string ID {get; set;}
    public string Script {get; set;}
    public int Easy {get; set;}
    public int Normal {get; set;}
    public int Hard {get; set;}
}

public class TutorialScript
{
    public string ID {get; set;}
    public string Script {get; set;}
    public int Comment {get; set;}
}

[Serializable]
public class ScriptSinario  //実際に使用するリストのクラスを定義する
{
    public string ID {get; set;}
    public string Senario {get; set;}
}

public class Comments
{
    public string Comment {get; set;}
    public float CommentAt {get; set;}
    public int Damage {get; set;}
}