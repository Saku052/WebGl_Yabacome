using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


//キャラの表情状態を管理する
public enum ScoreState
{
    Crying,
    Sad,
    Normal,
    Happy
}


public class ScoreController : MonoBehaviour
{

    //現在の表情を保存する
    public static ScoreState NowExpresion = ScoreState.Normal;
    //キャラを表示する
    [SerializeField] private Image _CharacterImage;
    [SerializeField] private Image _Character2Image;
    [SerializeField] private Image _FireImage;


    //スコア値レベルの変更
    [SerializeField] private int _level1;
    [SerializeField] private int _level2;
    [SerializeField] private int _level3;


    //画像パネルの取得
    [SerializeField] private GameObject Panel1;
    [SerializeField] private GameObject Panel2;


    //アニメーションの取得
    [SerializeField] private Animator Panel1Anim;
    [SerializeField] private Animator Panel2Anim;
    
    //スコアによって表情をチェックする
    public void CheckExpression()
    {

        //スプライトID
        string _spriteID = "CharacterMain3";
        string _sprite2ID = "MainIllust3";

        if(ScoreTracker.instance.Score <= _level1){
            NowExpresion = ScoreState.Crying;

            _spriteID = "CharacterMain1";
            _sprite2ID = "MainIllust1";
        }
        else if((ScoreTracker.instance.Score <= _level2) && (ScoreTracker.instance.Score > _level1)){
            NowExpresion = ScoreState.Sad;

            _spriteID = "CharacterMain2";
            _sprite2ID = "MainIllust2";
        }
        else if((ScoreTracker.instance.Score <= _level3) && (ScoreTracker.instance.Score > _level2)){
            NowExpresion = ScoreState.Normal;

            _spriteID = "CharacterMain3";
            _sprite2ID = "MainIllust3";
        }
        else if(ScoreTracker.instance.Score > _level3){
            NowExpresion = ScoreState.Happy;

            _spriteID = "CharacterMain4";
            _sprite2ID = "MainIllust4";
        }


        //キャラクターを表示する
        _CharacterImage.sprite = 
        Resources.Load("CharacterFace/" + _spriteID, typeof(Sprite)) as Sprite;
        _Character2Image.sprite = 
        Resources.Load("CharacterFace/" + _sprite2ID, typeof(Sprite)) as Sprite;
    }


    //ーーーーーーーーーーーーーーーーーーーーー
    //キャラの画像の位置を変える
    //ーーーーーーーーーーーーーーーーーーーーー
    public void ChangeImage(int num)
    {
        if(num == 34){
            _CharacterImage.SetOpacity(0.0f);
            _Character2Image.SetOpacity(1.0f);
        }else if(num == 54){
            _FireImage.SetOpacity(1.0f);
        }
    }


    //－－－－－－－－－－－－－－－－－－－－－－
    //パネルのアニメーション・画像を変更
    //－－－－－－－－－－－－－－－－－－－－－－
    public void ChangePanel(int num)
    {   
        if(num == 8){
            Panel1Anim.SetBool("OpenPan", true);
        }else if(num == 13){
            Panel1Anim.SetBool("OpenPan", false);
            updateNow = update.panel2;
        }else if(num == 25){
            Panel1Anim.SetBool("OpenPan", false);
            updateNow = update.panel3;
        }else if(num == 34){
            Panel1Anim.SetBool("OpenPan", false);
            updateNow = update.panel4;
        }else if(num == 59){
            Panel2Anim.SetBool("OpenPan", false);
            updateNow = update.panel5;
        }else if(num == 72){
            Panel1Anim.SetBool("OpenPan", false);
            updateNow = update.panel6;
        }else if(num == 81){
            Panel1Anim.SetBool("OpenPan", false);
        }
    }

    //パネルの状態の関数
    private update updateNow = update.none;
    enum update
    {
        none,
        panel2,
        panel3,
        panel4,
        panel5,
        panel6
    }

    //パネルのアニメーションを操作するアップデートメソッド
    private void Update()
    {
        switch(updateNow){

            case update.panel2:
                if(Panel1.transform.localScale == new Vector3(0.0f, 1.0f, 1.0f)){
                    Panel1.GetComponent<Image>().sprite = 
                    Resources.Load("Stuff/Panel2", typeof(Sprite)) as Sprite;
                    Panel1Anim.SetBool("OpenPan", true);
                }
                break;
            case update.panel3:
                if(Panel1.transform.localScale == new Vector3(0.0f, 1.0f, 1.0f)){
                    Panel1.GetComponent<Image>().sprite = 
                    Resources.Load("Stuff/Panel3", typeof(Sprite)) as Sprite;
                    Panel1Anim.SetBool("OpenPan", true);
                }
                break;
            case update.panel4:
                if(Panel1.transform.localScale == new Vector3(0.0f, 1.0f, 1.0f)){
                    Panel2Anim.SetBool("OpenPan", true);
                }
                break;
            case update.panel5:
                if(Panel2.transform.localScale == new Vector3(0.0f, 1.0f, 1.0f)){
                    Panel1.GetComponent<Image>().sprite = 
                    Resources.Load("Stuff/Wataame1", typeof(Sprite)) as Sprite;
                    Panel1Anim.SetBool("OpenPan", true);
                }
                break;
            case update.panel6:
                if(Panel2.transform.localScale == new Vector3(0.0f, 1.0f, 1.0f)){
                    Panel1.GetComponent<Image>().sprite = 
                    Resources.Load("Stuff/Wataame2", typeof(Sprite)) as Sprite;
                    Panel1Anim.SetBool("OpenPan", true);
                }
                break;
        }
    }
    

    //ーーーーーーーーーーーーーーーーーーーー
    //インスタンス化
    //ーーーーーーーーーーーーーーーーーーーー
    public static ScoreController instance;
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }

        ScoreController.NowExpresion = ScoreState.Normal;
    }
}



public static class ImageExt
{
    /// <summary>
    /// Imageの不透明度を設定する
    /// </summary>
    /// <param name="image">設定対象のImageコンポーネント(this)</param>
    /// <param name="alpha">不透明度。0=透明 1=不透明</param>
    public static void SetOpacity(this Image image, float alpha)
    {
        var c = image.color;
        image.color = new Color(c.r, c.g, c.b, alpha);
    }
}


public static class TextExt
{
    /// <summary>
    /// Imageの不透明度を設定する
    /// </summary>
    /// <param name="image">設定対象のImageコンポーネント(this)</param>
    /// <param name="alpha">不透明度。0=透明 1=不透明</param>
    public static void SetOpacityText(this Text image, float alpha)
    {
        var c = image.color;
        image.color = new Color(c.r, c.g, c.b, alpha);
    }
}