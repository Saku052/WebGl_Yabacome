using UnityEngine;
using System.Collections;

public enum MoveDirection{
	Right, Left, Up, Down
}

public class InputManager : MonoBehaviour {

	private GameManager gameManager;

	void Awake(){
		gameManager = GameObject.FindObjectOfType<GameManager>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(gameManager.state == GameState.Playing){
			if(Input.GetKeyDown(KeyCode.RightArrow)){
				gameManager.Move(MoveDirection.Right);
				gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
			}
			else if(Input.GetKeyDown(KeyCode.LeftArrow)){
				gameManager.Move(MoveDirection.Left);
				gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow)){
				gameManager.Move(MoveDirection.Up);
				gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
			}
			else if(Input.GetKeyDown(KeyCode.DownArrow)){
				gameManager.Move(MoveDirection.Down);
				gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
			}
		}
	}
}
