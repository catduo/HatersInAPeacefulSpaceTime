/*using UnityEngine;
using System.Collections;

public class PlanetaryControls: MonoBehaviour {
	
	public string action;
	public string upgradeConstruct;
	public string upgradeConstruct2;
	public string scrollLeft;
	public string scrollRight;
	public string switchClock;
	public string switchCounterClock;
	
	private Transform up;
	private Transform down;
	private Transform left;
	private Transform right;
	public Transform selected;
	
	public Transform statusArea;
	
	public float orbitSpeed;
	public float revolutionSpeed;
	
	public float planetaryHealth = 100;
	private Transform healthBar;
	public int playerMoney = 0;
	public TextMesh moneyText;
	private float moneyRate = 1;
	private float lastMoneyTime;
	public int player;
	public int otherPlayer;
	private Vector3 startPosition;
	public bool is_remote = false;
	private bool is_client = false;
	private bool is_gameStarted = false;
	
	// Use this for initialization
	void Start () {
		healthBar = statusArea.FindChild("HealthBar");
		moneyText = statusArea.FindChild("Money").GetComponent<TextMesh>();
		healthBar.GetComponent<ProgressBar>().measureCap = planetaryHealth;
		healthBar.GetComponent<ProgressBar>().measure = planetaryHealth;
		startPosition = transform.position;
		lastMoneyTime = 0;
		if(transform.position.x < 0){
			player = 1;
			otherPlayer = 2;
		}
		else{
			player = 2;
			otherPlayer = 1;
		}
		up = transform.FindChild("Up");
		down = transform.FindChild("Down");
		left = transform.FindChild("Left");
		right = transform.FindChild("Right");
		selected = up;
	}
	
	// Update is called once per frame
	void Update () {
		Income();
		if(!is_remote){
			Action();
			ChangeSelected();
		}
	}
	
	public void Remote(string remoteValue){
		is_remote = true;
		if(remoteValue == "client"){
			is_client = true;
		}
	}	
	public void NotRemote(string remoteValue){
		is_remote = false;
		if(remoteValue == "client"){
			is_client = true;
		}
	}
	
	void Income(){
		if(lastMoneyTime + moneyRate < Time.time){
			playerMoney++;
			moneyText.text = "&" + playerMoney.ToString();
			lastMoneyTime = Time.time;
		}
	}
	
	void FixedUpdate(){
		transform.Rotate(new Vector3(0, 0, revolutionSpeed));
		transform.RotateAround(transform.parent.position, new Vector3(0,0,1), orbitSpeed);
	}
	
	void Action() { 
		if(Input.GetKeyDown(action)){
			GetAction ();
		}
		if(Input.GetKeyDown(upgradeConstruct)){
			GetUpgradeConstruct ();
		}
		if(Input.GetKeyDown(upgradeConstruct2)){
			GetUpgradeConstruct ();
		}
		if(Input.GetKeyDown(scrollLeft)){
			GetScroll (-1);
		}
		if(Input.GetKeyDown(scrollRight)){
			GetScroll (1);
		}
	}
	
	[RPC] void GetAction () {
		selected.GetComponent<Building>().Action();
		if(!is_remote){
			networkView.RPC("GetAction", RPCMode.Others);
		}
	}
	
	[RPC] void GetScroll (int direction) {
		selected.GetComponent<Building>().Scroll(direction);
		if(!is_remote){
			networkView.RPC("GetScroll", RPCMode.Others, direction);
		}
	}
	
	[RPC] void GetUpgradeConstruct () {
		selected.GetComponent<Building>().Construct();
		if(!is_remote){
			networkView.RPC("GetUpgradeConstruct", RPCMode.Others);
		}
	}
	
	[RPC] void GetConstruct () {
		selected.GetComponent<Building>().Construct();
		if(!is_remote){
			networkView.RPC("GetConstruct", RPCMode.Others);
		}
	}
	
	void ChangeSelected(){
		if(Input.GetKeyDown(switchClock)){
			GetChangeClock () ;
		}
		if(Input.GetKeyDown(switchCounterClock)){
			GetChangeCounterClock ();
		}
	}
	
	[RPC] void GetChangeClock () {
		selected.GetComponent<Building>().UnSelected();
			if(selected == up){
				selected = right;
			}
			else if(selected == right){
				selected = down;
			}
			else if(selected == down){
				selected = left;
			}
			else if(selected == left){
				selected = up;
			}
		selected.GetComponent<Building>().Selected();
		if(!is_remote){
			networkView.RPC("GetChangeClock", RPCMode.Others);
		}
	}
	
	[RPC] void GetChangeCounterClock () {
		selected.GetComponent<Building>().UnSelected();
			if(selected == up){
				selected = left;
			}
			else if(selected == left){
				selected = down;
			}
			else if(selected == down){
				selected = right;
			}
			else if(selected == right){
				selected = up;
			}
		selected.GetComponent<Building>().Selected();
		if(!is_remote){
			networkView.RPC("GetChangeCounterClock", RPCMode.Others);
		}
	}
	
	void OnCollisionEnter(Collision collision){
		if((player == 1 && !is_remote) ||(player == 2 && is_remote)){
			BulletCollision (collision);
		}
	}
	
	[RPC] void BulletCollision (float damage){
		if(NetworkManager.is_local || NetworkManager.is_online || AI.is_ai){
			planetaryHealth -= damage;
			healthBar.GetComponent<ProgressBar>().measure = planetaryHealth;
			if(planetaryHealth < 1 && !NetworkManager.is_gameOver){
				GetComponent<AudioSource>().Play();
				particleSystem.Play();
				transform.FindChild("PlanetArt").renderer.enabled = false;				
				up.GetComponent<Building>().Reset();
				down.GetComponent<Building>().Reset();
				left.GetComponent<Building>().Reset();
				right.GetComponent<Building>().Reset();
				selected.GetComponent<Building>().UnSelected();
				GameState.gameOver = true;
				NetworkManager.is_gameOver = true;
				AI.is_ai = false;
				if(is_remote){
					NetworkManager.winningPlayerText = "You Won!";
				}
				else{
					NetworkManager.winningPlayerText = "You Lost!";
				}
			}
		}
	}
	
	[RPC] void BulletCollision (Collision collision) {
		if(NetworkManager.is_local || NetworkManager.is_online || AI.is_ai){
			planetaryHealth -= collision.transform.GetComponent<Projectile>().damage;
			healthBar.GetComponent<ProgressBar>().measure = planetaryHealth;
			if((player == 1 && !is_remote) || (player == 2 && is_remote)){
				networkView.RPC("BulletCollision", RPCMode.Others, collision.transform.GetComponent<Projectile>().damage);
			}
			if(planetaryHealth < 1 && !NetworkManager.is_gameOver){
				transform.FindChild("PlanetArt").renderer.enabled = false;
				up.GetComponent<Building>().Reset();
				down.GetComponent<Building>().Reset();
				left.GetComponent<Building>().Reset();
				right.GetComponent<Building>().Reset();
				selected.GetComponent<Building>().UnSelected();
				particleSystem.Play();
				GetComponent<AudioSource>().Play();
				GameState.gameOver = true;
				NetworkManager.is_gameOver = true;
				NewGame.is_gameStarted = false;
				AI.is_ai = false;
				if(is_remote){
					NetworkManager.winningPlayerText = "You Won!";
				}
				else{
					NetworkManager.winningPlayerText = "You Lost!";
				}
				GameObject.Find ("MainCamera").GetComponent<NetworkManager>().Disconnect();
			}
		}
	}
	
	[RPC] public void Ready () {
		if(NewGame.readyCount == 1){
			networkView.RPC("Ready", RPCMode.Others);
			NewGame.readyCount++;
		}
		else if (NewGame.readyCount == 2){
			networkView.RPC("Ready", RPCMode.Others);
			GameObject.Find ("NewGame").GetComponent<NewGame>().Tap();
		}
		else{
			NewGame.readyCount = 0;
		}
	}
	
	public void Reset () {
		transform.FindChild("PlanetArt").renderer.enabled = true;
		transform.rotation = Quaternion.identity;
		planetaryHealth = 100;
		healthBar.GetComponent<ProgressBar>().measureCap = planetaryHealth;
		healthBar.GetComponent<ProgressBar>().measure = planetaryHealth;
		up.GetComponent<Building>().Reset();
		down.GetComponent<Building>().Reset();
		left.GetComponent<Building>().Reset();
		right.GetComponent<Building>().Reset();
		lastMoneyTime = Time.time;
		playerMoney = 0;
		transform.position = startPosition;
		selected.GetComponent<Building>().UnSelected();
		selected = up;
		selected.GetComponent<Building>().Selected();
	}
}
 */