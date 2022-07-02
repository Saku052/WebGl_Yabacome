using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tile : MonoBehaviour {

	public bool mergedThisTurn = false;

	private int number;
	private Text tileText;
	private Image tileImage;
	private Animator animationTile;


	public int indRow;
	public int indCol;

	public int Number{
		get{
			return number;
		}

		set{
			number = value;
			if(number == 0){
				SetEmpty();
			} else {
				ApplyStyle(number);
				SetVisible();
			}
		}
	}

	void Awake(){

		animationTile = GetComponent<Animator>();

		if((tileText = GetComponentInChildren<Text>()) == null){
			Debug.LogError("tileText not found");
		} else {
			tileText = GetComponentInChildren<Text>();
		}

		if((tileImage = transform.Find("NumberedCell").GetComponent<Image>()) == null){
			Debug.LogError("tileImage not found");
		} else {
			tileImage = transform.Find("NumberedCell").GetComponent<Image>();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void ApplyStyleFromHolder(int index){
		tileText.text = TileStyleHolder.Instance.TileStyles[index].Number.ToString();
		tileText.color = TileStyleHolder.Instance.TileStyles[index].TextColor;

		tileImage.color = TileStyleHolder.Instance.TileStyles[index].TileColor;
	}

	private void ApplyStyle(int num){
		switch(num){
		case 2:
			ApplyStyleFromHolder(0);
			break;
		case 4:
			ApplyStyleFromHolder(1);
			break;
		case 8:
			ApplyStyleFromHolder(2);
			break;
		case 16:
			ApplyStyleFromHolder(3);
			break;
		case 32:
			ApplyStyleFromHolder(4);
			break;
		case 64:
			ApplyStyleFromHolder(5);
			break;
		case 128:
			ApplyStyleFromHolder(6);
			break;
		case 256:
			ApplyStyleFromHolder(7);
			break;
		case 512:
			ApplyStyleFromHolder(8);
			break;
		case 1024:
			ApplyStyleFromHolder(9);
			break;
		case 2048:
			ApplyStyleFromHolder(10);
			break;
		case 4096:
			ApplyStyleFromHolder(11);
			break;
		case 8192:
			ApplyStyleFromHolder(12);
			break;
		default:
			Debug.LogError("Check the numbers that you pass to ApplyStyle!");
			break;
		}
	}

	private void SetVisible(){
		tileImage.enabled = true;
		tileText.enabled = true;
	}

	private void SetEmpty(){
		tileImage.enabled = false;
		tileText.enabled = false;
	}

	// Animation functions:
	public void PlayMergeAnimation(){
		animationTile.SetTrigger("Merge");
	}

	public void PlayAppearAnimation(){
		animationTile.SetTrigger("Appear");
	}
}
