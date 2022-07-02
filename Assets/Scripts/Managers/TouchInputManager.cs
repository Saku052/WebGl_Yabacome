using UnityEngine;
using System.Collections;

public class TouchInputManager : MonoBehaviour {

	private float fingerStartTime = 0.0f;
	private Vector2 fingerStartPos = Vector2.zero;

	private bool isSwipe = false;
	private float minSwipeDist = 50.0f;
	private float maxSwipeTime = 1.5f;

	private GameManager gameManager;

	void Awake(){
		gameManager = GameObject.FindObjectOfType<GameManager>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(gameManager.state == GameState.Playing && Input.touchCount > 0){
			foreach(Touch touch in Input.touches){
				switch(touch.phase){
				case TouchPhase.Began:
					isSwipe = true;
					fingerStartTime = Time.time;
					fingerStartPos = touch.position;
					break;

				case TouchPhase.Canceled:
					isSwipe = false;
					break;

				case TouchPhase.Ended:
					float gestureTime = Time.time - fingerStartTime;
					float gestureDist = (touch.position - fingerStartPos).magnitude;

					if(isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
						Vector2 direction = touch.position - fingerStartPos;
						Vector2 swipeType = Vector2.zero;

						if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
							swipeType = Vector2.right * Mathf.Sign(direction.x);
						} else {
							swipeType = Vector2.up * Mathf.Sign(direction.y);
						}

						if(swipeType.x != 0.0f){
							if(swipeType.x > 0.0f){
								// MOVE RIGHT
								gameManager.Move(MoveDirection.Right);
								gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
							} else {
								// MOVE LEFT
								gameManager.Move(MoveDirection.Left);
								gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
							}
						}

						if(swipeType.y != 0.0f){
							if(swipeType.y > 0.0f){
								// MOVE UP
								gameManager.Move(MoveDirection.Up);
								gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
							} else {
								// MOVE DOWN
								gameManager.Move(MoveDirection.Down);
								gameManager.PlaySound(gameManager.soundManager.mergeTile, 1.0f);
							}
						}
					}

					break;
				}
			}
		}
	}
}
