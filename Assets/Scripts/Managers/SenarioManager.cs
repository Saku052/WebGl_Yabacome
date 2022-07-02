using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SenarioManager : MonoBehaviour
{
    //再生する為のオーディオソースを取得
    [SerializeField] private AudioSource audioSource;
    //字幕やコメントを表示するオブジェクト取得
    [SerializeField] private GameObject _CommentList;
    [SerializeField] public GameObject _tutPanel;
    [SerializeField] private GameObject _novelPanel;
    //最後のスコアを表示するパネル
    [SerializeField] private GameObject _FianalScorePanel;
    GameClear final;
    
    static public List<ScriptSinario> AudioVoice;   //オーディオリストのボイス
    public List<String> ListCommnet;    //他のクラスに送る用のリスト
    private List<Comments> _ListOfComment; //コメントのリストをここに保存する
    private Color _colorPallet;

    //クラスの情報を一旦全部取得
    private List<int> _commentCountList;
    private Comments _liveComment;
    //リストの数値
    public int _AudioNum = 0;
    private int _CommentNum = 0;


    //コメントをする時間
    private float _plzComment = 100.0f;

    private int _maxCommentCount;
    private int _CommentCount = 0;


    //一度に表示できるコメントの総数
    private int _TotalComment;

    //プレイヤーにダメージを与える
    private float _ScoreDamage = 0.0f;
    //ゲームオーバーチェック
    public bool checkOver = false;
    //テキスト表示の有無
    public bool CheckTextSkiped = false;
    [SerializeField] private GameObject Canvas;

    



    //順番にセリフを流していく
    public void AudioPlayList()
    {
        SkipSenario(_AudioNum);                             //イージーの場合・一部のシナリオスキップ

        audioSource.clip = 
        Resources.Load<AudioClip>("AudioFile/" + SenarioManager.AudioVoice[_AudioNum].ID);    //音声出力
        ScoreController.instance.ChangeImage(_AudioNum);    //画像の場所を変える
        ScoreController.instance.ChangePanel(_AudioNum);    //パネルを表示させる
        Canvas.GetComponent<GameManager>().DisableComment(_AudioNum);   //一定の数値になったらコメント不クリックにする

        
        _maxCommentCount = _commentCountList[_AudioNum];    //コメント数の編集（難易度別）

        _CommentCount = 0; //一度のオーディオでコメントした回数をリセット
        audioSource.Play(); //オーディオをスタートする

        gameObject.GetComponent<TextDisplay>().StartNovel();
    }


    private void GameOverVoice()    //ゲームオーバーの時のボイス
    {        
            audioSource.clip = 
            Resources.Load<AudioClip>("AudioFile/" + "VoiceA0");    //音声出力
            checkOver = true;   

            audioSource.Play(); //オーディオをスタートする

            gameObject.GetComponent<TextDisplay>().StartNovel();
    }


    private void CommentGone()  //コメントが流れて行った時
    {   
        //ノベルモードでないなら
        //コメントが流れていくたびに度にキャラに一定のダメージを与える
        if(GetLevel.Difficulty != GameLevel.Novel){
            _ScoreDamage = float.Parse(_CommentList.transform.GetChild(0).name);
            ScoreTracker.instance.Score -= (int)(_ScoreDamage/2.0f);
        }

        if(Canvas.GetComponent<GameManager>()._DeletingComment ==
        _CommentList.transform.GetChild(0).GetChild(0).GetComponent<Text>().text){
            Canvas.GetComponent<GameManager>().GameOver();
        }
    }


    public void ShowComments()
    {
        CommentGone();

        //コメントが表示される度に表情チェンジの確認をする
        ScoreController.instance.CheckExpression();
        
        //別クラス用のリストにここで追加する
        ListCommnet.Add(_CommentList.transform.GetChild(0).GetChild(0).GetComponent<Text>().text);

        //古くなったコメントを入れ替える
        for(int i = 0; i < _TotalComment - 1; i++){
            
            //コメントの内容を一つ一つ表示させる
            _CommentList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = 
            _CommentList.transform.GetChild(i + 1).GetChild(0).GetComponent<Text>().text;

            //DMGの内容を一つ一つ非表示にする
            _CommentList.transform.GetChild(i).name = 
            _CommentList.transform.GetChild(i + 1).name;

            //コメント色を変える
            _CommentList.transform.GetChild(i).GetChild(1).GetComponent<Image>().color =
            _CommentList.transform.GetChild(i + 1).GetChild(1).GetComponent<Image>().color;
        }
        
        //新しいコメントを表示する
        _CommentList.transform.GetChild(_TotalComment - 1).GetChild(0).GetComponent<Text>().text = 
        InsertCr(this._liveComment.Comment);
        

        //新しいコメントDMGを付ける
        _CommentList.transform.GetChild(_TotalComment - 1).name = this._liveComment.Damage.ToString();

        //コメント色を変える
        this.ApplyStyle(this._liveComment.Damage);
        _CommentList.transform.GetChild(_TotalComment - 1).GetChild(1).GetComponent<Image>().color =
        this._colorPallet;

        //最後に繰り上げてコメント情報をべっしん
        _CommentNum++;
        if(_CommentNum < this._ListOfComment.Count){
            this._liveComment = this._ListOfComment[_CommentNum];
        }

        //次にコメントする時間をここで取得する
        _plzComment = audioSource.clip.length*_liveComment.CommentAt;
    }

    private string InsertCr(string str) 
    {
        var sb = new System.Text.StringBuilder(str);

        var insertPos = 8;
        while (insertPos < sb.Length) {
            sb.Insert(insertPos, "\n");
            insertPos += "\n".Length + 8;
        }

        return sb.ToString();
    }

    private void GetTutorialLevel()
    {
        SenarioManager.AudioVoice = PlayFabLogin.TutorialSenario;
        this._ListOfComment = PlayFabLogin.TutorialComment;
        this._commentCountList = PlayFabLogin.TutComNum;
    }

    private void CheckGameLevel()   //ゲームの難易度の確認
    {
        switch(GetLevel.Difficulty){
            //ノベルモード
            case GameLevel.Novel:
                SenarioManager.AudioVoice = PlayFabLogin.VoiceSenario;
                this._ListOfComment = PlayFabLogin.EasyComment;
                this._commentCountList = PlayFabLogin.EasyComNum;

                //スコアの変更
                ScoreTracker.instance.Score = 9999;
                ScoreController.instance.CheckExpression();
                break;
            //イージー難易度
            case GameLevel.Easy:
                this._novelPanel.SetActive(false);
                SenarioManager.AudioVoice = PlayFabLogin.VoiceSenario;
                this._ListOfComment = PlayFabLogin.EasyComment;
                this._commentCountList = PlayFabLogin.EasyComNum;
                break;
            //ノーマル難易度
            case GameLevel.Normal:
                this._novelPanel.SetActive(false);
                SenarioManager.AudioVoice = PlayFabLogin.VoiceSenario;
                this._ListOfComment = PlayFabLogin.EasyComment;
                this._commentCountList = PlayFabLogin.NormalComNum;
                break;
            //ハード難易度
            case GameLevel.Hard:
                this._novelPanel.SetActive(false);
                SenarioManager.AudioVoice = PlayFabLogin.VoiceSenario;
                this._ListOfComment = PlayFabLogin.EasyComment;
                this._commentCountList = PlayFabLogin.HardComNum;
                break;
        }
    }

    //ーーーーーーーーーーーーーーーーーーーー
    //イージーの場合一部のシナリオスキップ
    //ーーーーーーーーーーーーーーーーーーーー
    private void SkipSenario(int num){
        if(GetLevel.Difficulty == GameLevel.Easy){
            if(num == 20){
                _AudioNum = 22;
            }else if(num == 52){
                _AudioNum = 56;
            }
        }
    }
    
   private void Start() 
   {
        //コメントの総数を取得
        _TotalComment = _CommentList.transform.childCount;
        this.final = new GameClear(_FianalScorePanel);
        this.final.DisableClear();

        if(!GetLevel.Tutorial){
            _tutPanel.SetActive(false);
            CheckGameLevel();

            StartGame();
        }else{

            _AudioNum = 1;
            this._novelPanel.SetActive(false);
            GetTutorialLevel();

            StartGame();
        }
   }

    private void StartGame()
    {
        //オーディオをプレイ
        AudioPlayList();
        //難易度に合わせたコメントをリストを取得して保存・使用
        this._liveComment = this._ListOfComment[_CommentNum];           
            
        //次にコメントする時間をここで取得する
        _plzComment = audioSource.clip.length*_liveComment.CommentAt;
    }

private bool _CheckFin = true;
    
    private void Update() 
    {
        
            //オーディオが止まる度に新しいオーディオにする
            if(audioSource.time == 0.0f && !audioSource.isPlaying){
                
                if(checkOver){
                    SceneManager.LoadScene("TitleScene");
                }
                
                //チュートリアル・ノベルモードなら何もしない
                //それ以外なら自動でシナリオが進む
                if(GetLevel.Tutorial || GetLevel.Difficulty == GameLevel.Novel){
                }else{
                    if(_AudioNum + 1 < SenarioManager.AudioVoice.Count){
                        //次のリストに移ってオーディオをプレイ
                        _AudioNum++;
                        if(ScoreController.NowExpresion == ScoreState.Crying){
                            GameOverVoice();
                        }else{
                            AudioPlayList();
                        }
                    }else{
                        if(_CheckFin){
                            _CheckFin = false;

                            gameObject.GetComponent<SendManager>().UpdateUserData();
                            StartCoroutine(final.ShowClear());
                            final.ShowGameClear();
                            StartCoroutine(final.GradScore());
                            StartCoroutine(final.GradRank());
                        }
                    }
                }
            }
            
        //コメントを追加する
        if(_CommentCount < _maxCommentCount)
        {
            //コメントを表示する
            if(audioSource.time > _plzComment){    
                ShowComments();

                _CommentCount++;
            }
        }
    }

    //クリア後クリックしたら次に行くかスコアを表示するか
    public void AfterGameClear()
    {
        final.GoToMainNext();
    }

    //ノベルモード時のクリックしたら次に進む処理
    public void NovelGoNext()
    {
        _AudioNum++;
        ifClickedEalier();
        AudioPlayList();
    }

    //チュートリアル時のクリックしたら次に進む処理
    public void TutorialGoNext()
    {
        switch(_AudioNum){
            case 1:
                _AudioNum++;
                AudioPlayList();
                break;
            case 2:
                ifClickedEalier();
                _tutPanel.SetActive(false);
                break;
            case 4:
                _AudioNum++;
                ifClickedEalier();
                AudioPlayList();
                break;
            case 5:
                _AudioNum++;
                ifClickedEalier();
                AudioPlayList();
                break;
            case 6:
                _AudioNum++;
                ifClickedEalier();
                AudioPlayList();
                break;
            case 7:
                _tutPanel.SetActive(false);
                _CommentNum = 0;
                _AudioNum = 0;
                GetLevel.Tutorial = false;
                this._novelPanel.SetActive(true);
                ifClickedEalier();
                CheckGameLevel();
                StartGame();
                break;
        }
    }

    public void ifClickedEalier()
    {
        //テキスト表示のスクリプトを中止
        CheckTextSkiped = true;
        //出されていないコメントを全て表示する
        while(_CommentCount < _maxCommentCount){
                    ShowComments();
                    _CommentCount++;
                }
    }


    private void ColorPallet(int index)
    {
        this._colorPallet = ColorManager.instance.Commentcolor[index].CommentCol;
    }


    private void ApplyStyle(int num)
    {
		switch(num){
		case 16:
			ColorPallet(0);
			break;
		case 32:
			ColorPallet(1);
			break;
		case 64:
			ColorPallet(2);
			break;
		case 128:
			ColorPallet(3);
			break;
		case 256:
			ColorPallet(4);
			break;
		case 512:
			ColorPallet(5);
			break;
		case 1024:
			ColorPallet(6);
			break;
		case 2048:
			ColorPallet(7);
			break;
        }
    }




    //シングルトン化
    public static SenarioManager instance;
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }
}
