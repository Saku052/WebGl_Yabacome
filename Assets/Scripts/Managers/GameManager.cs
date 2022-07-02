using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public enum GameState{
	Playing,
	GameOver,
	WaitingForMoveToEnd
}

public class GameManager : MonoBehaviour {


	//コメントパネル上に表示されるパネル達
	[SerializeField] private GameObject _commentList;
	[SerializeField] private GameObject _AttempScore;
	[SerializeField] private GameObject _CommentBlock;
	[SerializeField] private Text _commentText;
	//アニメーターの取得
	[SerializeField] private Animator _GetGameAnim;
	//イベントシステムを取得
	[SerializeField] private EventSystem _eventSystem;

	//クリアに必要な数字
	[SerializeField] private int _clearNumber;
	//消去したいコメントをここで取得する
	public String _DeletingComment = "";
	private int _DiffNumber;	//難易度別に出現する数字をコントロールする
	[SerializeField] private GameObject Canvas;
	//色の取得
	[SerializeField] private Color32 _ColorClicked;
	[SerializeField] private Color32 _ColorDeleted;
	[SerializeField] private Color32 _ColorFaild;
	

	void Start () 
	{
		_CommentBlock.SetActive(false);
		state = GameState.GameOver;	//ゲームの初期状態をゲームオーバーに変更
		ControlDiffNum();	//難易度別に場面に出てくる数字を設定
	}

	//ゲームのイニシャライズボタン
	public void initGame()
	{
		//コメントを選択でいなくする
		_CommentBlock.SetActive(true);
		
		//盤面の初期化
		rows.Clear();
		columns.Clear();

		//イニシャライズ
		_initGame();

		//ゲームの盤面を表示する
		state = GameState.Playing;
		_GetGameAnim.SetBool("StartGame", true);
	}


	//ーーーーーーーーーーーーーーーーーーーー
	//数値が一定になったらコメントをクリックできなくする
	//ーーーーーーーーーーーーーーーーーーーー
	public void DisableComment(int nowNum)
	{
		if(nowNum >= 90){
			GameOver();
			this._CommentBlock.SetActive(true);
		} 
	}


	//目標値に到達した時
	private void YouWin()
	{
		state = GameState.GameOver;
		PlaySound(soundManager.gameWin, 1.0f);

		for(int i = 0; i < _commentList.transform.childCount; i++)	//コメントを入れ替える
		{
			if(this._DeletingComment ==
				_commentList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text){

				_commentList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = 
				"このコメントは排除されました";
				_commentList.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = this._ColorDeleted;
				_commentList.transform.GetChild(i).name = "2";

				//スコアを加算するスクリプト
				//iの値が小さければ小さい程スコアは少なくなる。
				float _adjustNumber = 2.0f*((float)i+1)/((float)i+2);
				ScoreTracker.instance.Score += (int)(_adjustNumber*(float)_clearNumber);
			}
		}
		
		CommentWhenTut();
		this._DeletingComment = "";
		_CommentBlock.SetActive(false);
		_GetGameAnim.SetBool("StartGame", false);
		ScoreController.instance.CheckExpression();					//表情を変えるスクリプト
	}


	//盤面がマックスになった時	
	public void GameOver()
	{
		state = GameState.GameOver;

		
		for(int i = 0; i < _commentList.transform.childCount; i++)	//コメントを入れ替える
		{
			if(this._DeletingComment ==
				_commentList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text){

				_commentList.transform.GetChild(i).GetChild(1).GetComponent<Image>().color = this._ColorFaild;
				_commentList.transform.GetChild(i).name = "1000";
			}
		}

		CommentWhenTutFail();
		this._DeletingComment = "";
		_CommentBlock.SetActive(false);
		_GetGameAnim.SetBool("StartGame", false);
		PlaySound(soundManager.gameOver, 1.0f);
	}



	//コメントをクリックした時の処理
	public void CommentClicked()
	{
		CommentWhenTut();	//チュートリアルだった場合

		//目標値
		string _ScoreNum = _eventSystem.currentSelectedGameObject.name;

		//ーーーーーーーーーーーーーーーーーーーーーーーーー
		//ゲーム開始のスクリプト
		//ーーーーーーーーーーーーーーーーーーーーーーーーー

		//終わっている数字でないなら
		if((_ScoreNum != "1000") && (_ScoreNum != "2")){
			//必要なスコア値を画面上に表示する
			this._AttempScore.GetComponent<Text>().text = 
			"目標値: " + _ScoreNum;

			//タップしたコメントを一時的に保管する
			this._DeletingComment = 
			_eventSystem.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;

			//タップしたコメントの色変更
			_eventSystem.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().color = this._ColorClicked;
			//タップしたコメントを表示する
			this._commentText.text = this._DeletingComment;

			//目標値を設定する
			_clearNumber = int.Parse(_ScoreNum); 

			//ゲームスタート
			initGame();
		}
	}

	//コメントをクリックした時
	//コメントをクリアした時
	//二回呼び出される
	private void CommentWhenTut()
	{
		if(GetLevel.Tutorial){
			//最初のコールでAudioNumを3にする
			//二回目で4にする
			Canvas.GetComponent<SenarioManager>()._AudioNum++;

			//二回目ならコメントを出し切ったか確認して、パネルを表示
			if(Canvas.GetComponent<SenarioManager>()._AudioNum == 4){
				Canvas.GetComponent<SenarioManager>().ifClickedEalier();
				Canvas.GetComponent<SenarioManager>()._tutPanel.SetActive(true);
			}

			//音声プレイ
			Canvas.GetComponent<SenarioManager>().AudioPlayList();
		}
		
	}
	private void CommentWhenTutFail()
	{
		if(GetLevel.Tutorial){
			Canvas.GetComponent<SenarioManager>()._AudioNum = 0;
			Canvas.GetComponent<SenarioManager>().AudioPlayList();
			Canvas.GetComponent<SenarioManager>().checkOver = true;
		}
	}
		
	

	private void Generate()
	{
		if(EmptyTiles.Count > 0){
			
			int indexForNewNumber = Random.Range(0, EmptyTiles.Count);
			int randomNum = Random.Range(0, 5);


			//ランダムでボックスに表示する数字とその確率の調整

			if(randomNum == 0){
				EmptyTiles[indexForNewNumber].Number = _DiffNumber;
			} else if(randomNum == 1 || randomNum == 2){
				EmptyTiles[indexForNewNumber].Number = 4;
			} else {
				EmptyTiles[indexForNewNumber].Number = 2;
			}

			EmptyTiles[indexForNewNumber].PlayAppearAnimation();
			EmptyTiles.RemoveAt(indexForNewNumber);

		}
	}

	private void ControlDiffNum()	//難易度別に出てくる数字をコントロールする
	{

		switch(GetLevel.Difficulty){
			//難易度イージーの場合
			case GameLevel.Easy:
				_DiffNumber = 4;
				break;
			//難易度ノーマルの場合
			case GameLevel.Normal:
				_DiffNumber = 4;
				break;
			//難易度ハードの場合
			case GameLevel.Hard:
				_DiffNumber = 8;
				break;
		}
	}


	private  bool MakeOneMoveDownIndex(Tile[] LineOfTiles){
		for(int i = 0; i < LineOfTiles.Length - 1; i++){
			// MOVE BLOCK
			if(LineOfTiles[i].Number == 0 && LineOfTiles[i +1 ].Number != 0){
				LineOfTiles[i].Number = LineOfTiles[i + 1].Number;
				LineOfTiles[i + 1].Number = 0;

				return true;
			}

			// MERGE BLOCK
			if(LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i + 1].Number &&
				LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false){

				LineOfTiles[i].Number *= 2;
				LineOfTiles[i + 1].Number = 0;
				LineOfTiles[i].mergedThisTurn = true;
				LineOfTiles[i].PlayMergeAnimation();

				ScoreTracker.instance.Score += (int)((float)LineOfTiles[i].Number*0.25f);

				if(LineOfTiles[i].Number == _clearNumber){	
					YouWin();
				}

				return true;
			}
		} 
		return false;
	}

	private  bool MakeOneMoveUpIndex(Tile[] LineOfTiles){
		for(int i = LineOfTiles.Length - 1; i > 0; i--){
			// MOVE BLOCK
			if(LineOfTiles[i].Number == 0 && LineOfTiles[i - 1 ].Number != 0){
				LineOfTiles[i].Number = LineOfTiles[i - 1].Number;
				LineOfTiles[i - 1].Number = 0;

				return true;
			}

			// MERGE BLOCK
			if(LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i - 1].Number &&
				LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false){

				LineOfTiles[i].Number *= 2;
				LineOfTiles[i - 1].Number = 0;
				LineOfTiles[i].mergedThisTurn = true;
				LineOfTiles[i].PlayMergeAnimation();
					
				ScoreTracker.instance.Score += (int)((float)LineOfTiles[i].Number*0.25f);

				if(LineOfTiles[i].Number == _clearNumber){
					YouWin();
				}

				return true;
			}
		}
		return false;
	}



//ここから複雑系統のコマンド、あまり触らない方がいいです。ただ細かい編集の為に使用することがあるかも。
//ここから複雑系統のコマンド、あまり触らない方がいいです。ただ細かい編集の為に使用することがあるかも。
//ここから複雑系統のコマンド、あまり触らない方がいいです。ただ細かい編集の為に使用することがあるかも。
//ここから複雑系統のコマンド、あまり触らない方がいいです。ただ細かい編集の為に使用することがあるかも。
//ここから複雑系統のコマンド、あまり触らない方がいいです。ただ細かい編集の為に使用することがあるかも。



	//ゲーム盤面のイニシャライズ
	private void _initGame()
	{
		Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile>();
		soundManager = GameObject.FindObjectOfType<SoundManager>();

		if(!soundManager){
			Debug.Log("ERROR: sound manager not found!");
		} 


		foreach(Tile tile in AllTilesOneDim){
			tile.Number = 0;
			AllTiles[tile.indRow, tile.indCol] = tile;
			EmptyTiles.Add(tile);
		}

		columns.Add(new Tile[]{AllTiles[0, 0], AllTiles[1, 0], AllTiles[2, 0], AllTiles[3, 0] });
		columns.Add(new Tile[]{AllTiles[0, 1], AllTiles[1, 1], AllTiles[2, 1], AllTiles[3, 1] });
		columns.Add(new Tile[]{AllTiles[0, 2], AllTiles[1, 2], AllTiles[2, 2], AllTiles[3, 2] });
		columns.Add(new Tile[]{AllTiles[0, 3], AllTiles[1, 3], AllTiles[2, 3], AllTiles[3, 3] });

		rows.Add(new Tile[]{AllTiles[0, 0], AllTiles[0, 1], AllTiles[0, 2], AllTiles[0, 3] });
		rows.Add(new Tile[]{AllTiles[1, 0], AllTiles[1, 1], AllTiles[1, 2], AllTiles[1, 3] });
		rows.Add(new Tile[]{AllTiles[2, 0], AllTiles[2, 1], AllTiles[2, 2], AllTiles[2, 3] });
		rows.Add(new Tile[]{AllTiles[3, 0], AllTiles[3, 1], AllTiles[3, 2], AllTiles[3, 3] });

		Generate();
		Generate();
	}



	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義

	public GameState state;

	[Range(0,2f)]
	public float delay;
	private bool moveMade;
	private bool[] lineMoveComplete = new bool[4]{true, true, true, true};
	

	// sound buttons:
	public GameObject btSoundOff;
	public GameObject btSoundOn;
	
	private Tile[,] AllTiles = new Tile[4,4];
	private List<Tile[]> columns = new List<Tile[]>();
	private List<Tile[]> rows = new List<Tile[]>();
	private List<Tile> EmptyTiles = new List<Tile>();

	[HideInInspector]
	public SoundManager soundManager;

	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義
	//ぶっちゃけ良く分からない使用変数の定義


public void NewGameButtonHandler(){
//		Application.LoadLevel(Application.loadLevel);

		PlaySound(soundManager.clickButton, 1.0f);

		StartCoroutine(WaitForLoadScene(0.5f, "Scenes/TitleScene"));

	}

	public void CheckSoundFxHandler(){

		soundManager.ToggleFx();

		if(!btSoundOff.activeSelf){
			btSoundOff.SetActive(true);
		} else {
			btSoundOff.SetActive(false);
		}

		if(!btSoundOn.activeSelf){
			btSoundOn.SetActive(true);
		} else {
			btSoundOn.SetActive(false);
		}
	}

	// BUTTON HANDLERS. END

	public void Move(MoveDirection moveDirection){

		bool moveMade = false;

		ResetMergedFlags();

		if(delay > 0){
			StartCoroutine(MoveCoroutine(moveDirection));
		} else {

			for(int i = 0; i < rows.Count; i++){
				switch(moveDirection){
				case MoveDirection.Down:
					while(MakeOneMoveUpIndex(columns[i]))
					{
						moveMade = true;
					}
					break;
				case MoveDirection.Left:
					while(MakeOneMoveDownIndex(rows[i]))
					{
						moveMade = true;
					}
					break;
				case MoveDirection.Right:
					while(MakeOneMoveUpIndex(rows[i]))
					{
						moveMade = true;
					}
					break;
				case MoveDirection.Up:
					while(MakeOneMoveDownIndex(columns[i]))
					{
						moveMade = true;
					}
					break;
				}
			}
		}
			

		if(moveMade){
			UpdateEmptyTiles();
			Generate();

			if(!CanMove()){
				GameOver();
			}
		}
	}

	private IEnumerator MoveCoroutine(MoveDirection moveDirection){
		state = GameState.WaitingForMoveToEnd;

		switch(moveDirection){
		case MoveDirection.Down:
			for(int i = 0; i < columns.Count; i++){
				StartCoroutine(MoveOneLineUpIndexCoroutine(columns[i], i));
			}
			break;
		case MoveDirection.Left:
			for(int i = 0; i < rows.Count; i++){
				StartCoroutine(MoveOneLineDownIndexCoroutine(rows[i], i));
			}
			break;
		case MoveDirection.Right:
			for(int i = 0; i < rows.Count; i++){
				StartCoroutine(MoveOneLineUpIndexCoroutine(rows[i], i));
			}
			break;
		case MoveDirection.Up:
			for(int i = 0; i < columns.Count; i++){
				StartCoroutine(MoveOneLineDownIndexCoroutine(columns[i], i));
			}
			break;
		}

		while(! (lineMoveComplete[0] && lineMoveComplete[1] && lineMoveComplete[2] && lineMoveComplete[3])){
			yield return null;
		}

		if(moveMade){
			UpdateEmptyTiles();
			Generate();

			if(!CanMove()){
				GameOver();
			} 
		}
		
		if(state == GameState.WaitingForMoveToEnd){
		state = GameState.Playing;
		}
		StopAllCoroutines();
	}

private IEnumerator MoveOneLineUpIndexCoroutine(Tile[] line, int index){
		lineMoveComplete[index] = false;

		while(MakeOneMoveUpIndex(line)){
			moveMade = true;

			yield return new WaitForSeconds(delay);
		}
		lineMoveComplete[index] = true;
	}

	private IEnumerator MoveOneLineDownIndexCoroutine(Tile[] line, int index){
		lineMoveComplete[index] = false;
		while(MakeOneMoveDownIndex(line)){
			moveMade = true;
			yield return new WaitForSeconds(delay);
		}
		lineMoveComplete[index] = true;
	}

	private void ResetMergedFlags(){
		foreach(Tile tile in AllTiles){
			tile.mergedThisTurn = false;
		}
	}

	private void UpdateEmptyTiles(){
		EmptyTiles.Clear();

		foreach(Tile tile in AllTiles){
			if(tile.Number == 0){
				EmptyTiles.Add(tile);
			}
		}
	}

	private bool CanMove(){
		if(EmptyTiles.Count > 0){
			return true;
		} else {
			// check columns:
			for(int i = 0; i < columns.Count; i++){
				for(int j = 0; j < rows.Count - 1; j++){
					if(AllTiles[j, i].Number == AllTiles[j + 1, i].Number){
						return true;
					}
				}
			}

			// check rows:
			for(int i = 0; i < rows.Count; i++){
				for(int j = 0; j < columns.Count - 1; j++){
					if(AllTiles[i, j].Number == AllTiles[i, j + 1].Number){
						return true;
					}
				}
			}
		}

		return false;
	}

	// play FxSound function:
	public void PlaySound(AudioClip clip, float volMultiplier = 1.0f){
		if(clip && soundManager.fxEnable){
			AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
		}
	}
	private IEnumerator WaitForLoadScene(float waitTime, string sceneName){
		yield return new WaitForSeconds(waitTime);

		SceneManager.LoadScene(sceneName);
	}
}

