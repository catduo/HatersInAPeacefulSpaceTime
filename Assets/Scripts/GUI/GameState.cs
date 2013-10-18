using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {
	
	private GameObject menu;
	private GameObject mainCamera;
	private GameObject ftue;
	private TextMesh scoreText;
	private TextMesh highScore;
	private TextMesh thisScore;
	private AudioSource audioSource;
	static public int score;
	private int topScore;
	static public bool gameOver = false;
	private float touchSpeed;
	private float swipeThreshhold = 10;
	private Vector3 previousPosition = new Vector3 (1000,1000,1000);
	private LineRenderer drawSwipe;
	private int vertexCount = 0;
	private int vertexMax = 5;
	private GameObject capsule;
	private Vector3 screenToWorldCurrent;
	static public Vector2[] InputXYs = new Vector2[1];

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find("MainCamera");
		drawSwipe = transform.GetComponent<LineRenderer>();
		audioSource = transform.GetComponent<AudioSource>();
		drawSwipe.SetVertexCount(0);
		capsule = GameObject.Find ("SwipeCapsule");
		UpdateScore();
	}
	
	// Update is called once per frame
	void Update () {
		GetTouch();
		GetMouse();
		if(gameOver){
			GameOver();
		}
	}
	
	//find everything that has a touch initiated on it and let it know
	void GetTouch() {	
		InputXYs = new Vector2[Input.touchCount];
		for (int i = 0; i < Input.touchCount; i++){
			Touch touch = Input.GetTouch(i);
			InputXYs[i] = Camera.main.ScreenToWorldPoint(touch.position);
			if(touch.phase == TouchPhase.Began){
				previousPosition = touch.position;
	            Ray ray = Camera.main.ScreenPointToRay(touch.position);
	            RaycastHit objectTouched ;
	            if (Physics.Raycast (ray, out objectTouched)) {
	                 objectTouched.transform.SendMessage("Tap", SendMessageOptions.DontRequireReceiver);
	            }
			}
			if(touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved){
				previousPosition = touch.position;
	            Ray ray = Camera.main.ScreenPointToRay(touch.position);
	            RaycastHit objectTouched ;
	            if (Physics.Raycast (ray, out objectTouched)) {
	                 objectTouched.transform.SendMessage("Hold", SendMessageOptions.DontRequireReceiver);
	            }
			}
			if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
	            Ray ray = Camera.main.ScreenPointToRay(touch.position);
	            RaycastHit objectTouched ;
	            if (Physics.Raycast (ray, out objectTouched)) {
	                 objectTouched.transform.SendMessage("Release", SendMessageOptions.DontRequireReceiver);
	            }
				capsule.transform.position = new Vector3 (1000,1000,1000);
				previousPosition = new Vector3 (1000,1000,1000);
				drawSwipe.SetVertexCount(0);
				vertexCount = 0;
			}
			/*
			//create a collision with everything that is swiped by touch
			if(touch.phase == TouchPhase.Moved){
				if(previousPosition != new Vector3 (1000,1000,1000) && Vector3.Distance(previousPosition, touch.position) > swipeThreshhold && vertexCount < vertexMax){
					drawSwipe.SetVertexCount(vertexCount+1);
					previousPosition = mainCamera.camera.ScreenToWorldPoint(previousPosition);
					screenToWorldCurrent = mainCamera.camera.ScreenToWorldPoint(touch.position);
					drawSwipe.SetPosition(vertexCount, new Vector3(screenToWorldCurrent.x, screenToWorldCurrent.y, -10F));
					capsule.transform.position = new Vector3(previousPosition.x + (screenToWorldCurrent.x - previousPosition.x) / 2, previousPosition.y + (screenToWorldCurrent.y - previousPosition.y) / 2, -10F);
	       			capsule.transform.LookAt(new Vector3(previousPosition.x, previousPosition.y, -10F));
					capsule.transform.Rotate (new Vector3 (90,0,0));
	       			capsule.transform.localScale = new Vector3(1F, (screenToWorldCurrent - previousPosition).magnitude/2, 1F);
					vertexCount++;
					previousPosition = touch.position;
				}
				else{
					capsule.transform.position = new Vector3 (1000,1000,1000);
					previousPosition = touch.position;
					drawSwipe.SetVertexCount(0);
					vertexCount = 0;
				}
			}
			*/
		}
	}
	
	//find everything that has the mouse click on it and let it know
	void GetMouse() {
		if(Input.GetMouseButtonDown(0)){
			InputXYs = new Vector2[1];
			InputXYs[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 simTouch = Input.mousePosition;
	        Ray Ray = Camera.main.ScreenPointToRay(simTouch);
	        RaycastHit objectTouched;
	        if (Physics.Raycast (Ray, out objectTouched)) {
	             objectTouched.transform.SendMessage("Tap", SendMessageOptions.DontRequireReceiver);
	        }
		}
		//create a collision with everything that is swiped by mouse
		if(Input.GetMouseButton(0)){
			InputXYs = new Vector2[1];
			InputXYs[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(previousPosition != new Vector3 (1000,1000,1000) && Vector3.Distance(previousPosition, Input.mousePosition) > swipeThreshhold && vertexCount < vertexMax){
				drawSwipe.SetVertexCount(vertexCount+1);
				previousPosition = mainCamera.camera.ScreenToWorldPoint(previousPosition);
				screenToWorldCurrent = mainCamera.camera.ScreenToWorldPoint(Input.mousePosition);
				drawSwipe.SetPosition(vertexCount, new Vector3(screenToWorldCurrent.x, screenToWorldCurrent.y, -10F));
				capsule.transform.position = new Vector3(previousPosition.x + (screenToWorldCurrent.x - previousPosition.x) / 2, previousPosition.y + (screenToWorldCurrent.y - previousPosition.y) / 2, -10F);
       			capsule.transform.LookAt(new Vector3(previousPosition.x, previousPosition.y, -10F));
				capsule.transform.Rotate (new Vector3 (90,0,0));
       			capsule.transform.localScale = new Vector3(1F, (screenToWorldCurrent - previousPosition).magnitude/2, 1F);
				vertexCount++;
				previousPosition = Input.mousePosition;
			}
			else{
				capsule.transform.position = new Vector3 (1000,1000,1000);
				previousPosition = Input.mousePosition;
				drawSwipe.SetVertexCount(0);
				vertexCount = 0;
			}
			Vector3 simTouch = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(simTouch);
            RaycastHit objectTouched;
            if (Physics.Raycast (ray, out objectTouched)) {
                 objectTouched.transform.SendMessage("Hold", SendMessageOptions.DontRequireReceiver);
            }
		}
		if(Input.GetMouseButtonUp(0)){
			InputXYs = new Vector2[1];
			InputXYs[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			capsule.transform.position = new Vector3 (1000,1000,1000);
			previousPosition = new Vector3 (1000,1000,1000);
			drawSwipe.SetVertexCount(0);
			vertexCount = 0;
			Vector3 simTouch = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(simTouch);
            RaycastHit objectTouched;
            if (Physics.Raycast (ray, out objectTouched)) {
                 objectTouched.transform.SendMessage("Release", SendMessageOptions.DontRequireReceiver);
            }
		}
	}
	
	//update the score text
	void UpdateScore () {
	}
	
	//when the game ends put up a menu that lets you restart
	void GameOver(){
		if(topScore < 1){
			topScore = score;
		}
		else if(score > topScore){
			topScore = score;
		}
		PlayerPrefs.SetInt("TopScore", topScore);
		highScore.text = "High Score: " + topScore.ToString();
		thisScore.text = "This Round: " + score.ToString();
	}
	
	void Reset () {
		gameOver = false;
	}
}