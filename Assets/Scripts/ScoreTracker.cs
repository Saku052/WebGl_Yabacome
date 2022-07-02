using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScoreTracker : MonoBehaviour {
	private int score;
	public static ScoreTracker instance;
	public Text scoreText;
	public Text highScoreText;

	public int Score{
		get{
			return score;
		}

		set{
			score = value;
			scoreText.text = score.ToString();

			if(PlayerPrefs.GetInt("HighScore") < score){
				PlayerPrefs.SetInt("HighScore", score);
				//highScoreText.text = score.ToString();
			}
		}
	}



	void Awake(){

		//PlayerPrefs.DeleteAll ();

		instance = this;

		if(!PlayerPrefs.HasKey("HighScore")){
			PlayerPrefs.SetInt("HighScore", 0);

			scoreText.text = "0";
			//highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			//highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
		}
	}
}


//ーーーーーーーーーーーーーーーーーーーー
//ゲームクリア時のスクリプト
//ーーーーーーーーーーーーーーーーーーーー
public class GameClear
{

	//パネル情報を保存する
	private static GameObject Panel;
	private static Image MainPanel;
	private static Text Score;
	private static Text Mode;

	//どのモードでクリアしたか
	private string Diff;

	//何スコアで何ランクか
	private int FRank = -1000;
	private int ERank = -500;
	private int DRank = 0;
	private int CRank = 500;
	private int BRank = 1000;
	private int ARank = 3000;
	private int SRank = 5000;
	private int VRank = 8000;


	//コンストラクタ
	public GameClear(GameObject panel)
	{
		GameClear.Panel = panel;
		GameClear.MainPanel = panel.GetComponent<Image>();
		GameClear.Mode = panel.transform.GetChild(0).gameObject.GetComponent<Text>();
		GameClear.Score = panel.transform.GetChild(1).gameObject.GetComponent<Text>();
	}

	
	//パネルを非表示にする
	public void DisableClear()
	{
		GameClear.Panel.SetActive(false);
	}


	//パネルを表示
	public void ShowGameClear()
	{
		GameClear.Panel.SetActive(true);

		//どのモードだったのかを文字列にする
		switch(GetLevel.Difficulty){
			case GameLevel.Easy:
				this.Diff = "イージーモード　";
				break;
			case GameLevel.Normal:
				this.Diff = "ノーマルモード　";
				break;
			case GameLevel.Hard:
				this.Diff = "ハードモード　";
				break;
		}

	}


	//パネルを徐々に表示する
	public IEnumerator ShowClear()
	{
		float i = 0.0f;
		while(i < 1.0f){
			i = i + 0.02f;
			GameClear.MainPanel.SetOpacity(i);
			GameClear.Score.SetOpacityText(i);
			GameClear.Mode.SetOpacityText(i);
			yield return new WaitForSeconds(0.001f);
		}
	}

	//スコアを徐々にあげる
	public IEnumerator GradScore()
	{
		int i = 0;

		if(ScoreTracker.instance.Score > 0){
			while(i < ScoreTracker.instance.Score){
				GameClear.Score.text = "スコア　: " + i.ToString();
				i = i + 50;
				yield return null;
			}
			GameClear.Score.text = "スコア　: " + ScoreTracker.instance.Score.ToString();
		}else{
			while(i > ScoreTracker.instance.Score){
				GameClear.Score.text = "スコア　: " + i.ToString();
				i = 1 - 50;
				yield return null;
			}
			GameClear.Score.text = "スコア　: " + ScoreTracker.instance.Score.ToString();
		}
	}
	
	public void GoToMainNext()
	{
		//スコアが全て表示されてなかった場合
		if("スコア　: " + ScoreTracker.instance.Score.ToString() != GameClear.Score.text){
			GameClear.Score.text = "スコア　: " + ScoreTracker.instance.Score.ToString();
		}
		//スコアが全て表示された場合
		else{
			SceneManager.LoadScene("TitleScene");
		}
	}


	//ランクを徐々に上げていく
	public IEnumerator GradRank()
	{
		if(ScoreTracker.instance.Score > this.FRank) GameClear.Mode.text = this.Diff + "Fランク";
		yield return new WaitForSeconds(0.5f);
		if(ScoreTracker.instance.Score > this.ERank) GameClear.Mode.text = this.Diff + "Eランク";
		yield return new WaitForSeconds(0.6f);
		if(ScoreTracker.instance.Score > this.DRank) GameClear.Mode.text = this.Diff + "Dランク";
		yield return new WaitForSeconds(0.7f);
		if(ScoreTracker.instance.Score > this.CRank) GameClear.Mode.text = this.Diff + "Cランク";
		yield return new WaitForSeconds(0.8f);
		if(ScoreTracker.instance.Score > this.BRank) GameClear.Mode.text = this.Diff + "Bランク";
		yield return new WaitForSeconds(0.9f);
		if(ScoreTracker.instance.Score > this.ARank) GameClear.Mode.text = this.Diff + "Aランク";
		yield return new WaitForSeconds(1.0f);
		if(ScoreTracker.instance.Score > this.SRank) GameClear.Mode.text = this.Diff + "Sランク";
		yield return new WaitForSeconds(1.0f);
		if(ScoreTracker.instance.Score > this.VRank) GameClear.Mode.text = this.Diff + "Vランク";
		yield return new WaitForSeconds(1.0f);
	}


}