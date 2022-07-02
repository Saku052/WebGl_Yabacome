using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public bool fxEnable = true;

	[Range(0, 1)]
	public float fxVolume = 1.0f;

	public AudioClip moveTile;
	public AudioClip mergeTile;
	public AudioClip clickButton;
	public AudioClip gameWin;
	public AudioClip gameOver;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleFx(){
		fxEnable = !fxEnable;
	}
}
