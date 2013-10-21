using UnityEngine;
using System.Collections;

public enum PlayerState {
	Menu,
	Building,
	Launching,
	Fighting,
	Recovering,
	RoundEnded
}

public class ShipControls: MonoBehaviour {
	
	public PlayerState thisPlayerState = PlayerState.Fighting;
	
	public Transform up;
	public Transform left;
	public Transform right;
	private Transform mainEngine;
	private Transform rightEngine;
	private Transform leftEngine;
	
	public float healthMultiplier = 1.2F;
	public float engineMultiplier = 1.2F;
	public float moneyMultiplier = 1.5F;
	
	private Transform statusArea;
	
	private string upType;
	private string rightType;
	private string leftType;
	public int upLevel = 0;
	public int rightLevel = 0;
	public int leftLevel = 0;
	public int engineLevel = 0;
	public int healthLevel = 0;
	public int moneyLevel = 0;
	private float maxSpeed = 1.5F;
	private float acceleration = 5;
	
	public float playerHealth = 100;
	public float maxHealth = 100;
	
	public int playerMoney = 100;
	public float moneyRate = 1;
	private float lastMoneyTime;
	
	public int playerXP;
	
	private Vector2 engineFiring;
	private bool is_client = false;
	
	private Transform heavenlyBodyParent;
	private Transform[] heavenlyBodies;
	public float gravitationalConstant = 3F;
	
	private int dpadTouch = -1;
	private float initialX;
	private float initialY;
	private float horizontal;
	private float vertical;
	private bool is_dpadUse = false;
	
	// Use this for initialization
	void Start () {
		if(PlayerPrefs.GetInt("PlayerMoney") > 0){
			playerMoney = PlayerPrefs.GetInt("PlayerMoney");
		}
		else{
			playerMoney += 30;
		}
		if(PlayerPrefs.GetInt("PlayerXP") != null){
			playerXP = PlayerPrefs.GetInt("PlayerXP");
		}
		else{
			playerXP = 0;
		}
		if(networkView.isMine || NetworkAndMenu.is_local){
			is_client = true;
		}
		networkView.group = 1;
		up = transform.FindChild("Up");
		left = transform.FindChild("Left");
		right = transform.FindChild("Right");
		mainEngine = transform.FindChild("MainEngine");
		rightEngine = transform.FindChild("RightEngine");
		leftEngine = transform.FindChild("LeftEngine");
		if(is_client){
			transform.position = new Vector3(5.5F, -2, 0);
			transform.localScale = new Vector3(5,5,0);
			lastMoneyTime = Time.time - moneyRate;
		}
		else if(Network.isServer){
			heavenlyBodyParent = GameObject.Find("Planets").transform;
			heavenlyBodies = new Transform[heavenlyBodyParent.childCount];
			for(int i = 0; i < heavenlyBodyParent.childCount; i++){
				heavenlyBodies[i] = heavenlyBodyParent.GetChild(i);
			}
			Death();
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
		}
		else{
			Destroy(gameObject);
		}
		upType = "Empty";
		rightType = "Empty";
		leftType = "Empty";
		upLevel = 0;
		rightLevel = 0;
		leftLevel = 0;
		engineLevel = 0;
		healthLevel = 0;
		moneyLevel = 0;
	}
	
	void FixedUpdate(){
		if(is_client){
			Engines(horizontal, vertical);
		}
		else if(thisPlayerState == PlayerState.Fighting){
			for(int i = 0; i < heavenlyBodyParent.childCount; i++){
				float force = -(heavenlyBodies[i].localScale.x * gravitationalConstant / Vector3.SqrMagnitude(new Vector3(transform.position.x - heavenlyBodies[i].position.x, transform.position.y - heavenlyBodies[i].position.y, 0)));
				Vector3 direction = Vector3.Normalize(new Vector3(transform.position.x - heavenlyBodies[i].position.x, transform.position.y - heavenlyBodies[i].position.y, 0));
				rigidbody.AddForce(force * direction);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(is_client){
			Income ();
			DPad ();
		}
	}
	
	void Income(){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client && lastMoneyTime + moneyRate < Time.time){
				playerMoney++;
				lastMoneyTime = Time.time;
			}
		}
	}
	
	[RPC] void Engines(float thisHorizontal, float thisVertical){
		if(thisPlayerState == PlayerState.Fighting){
			rigidbody.angularVelocity = Vector3.zero;
			if(is_client){
				thisVertical = vertical;
				thisHorizontal = horizontal;
				//show animations, but don't move anything
				rightEngine.GetComponent<Engine>().Power(Mathf.Min( ((0 - thisVertical - thisHorizontal * 2) * 2F), 2F));
				leftEngine.GetComponent<Engine>().Power(Mathf.Min( ((thisHorizontal * 2 - thisVertical) * 2F), 2F));
				mainEngine.GetComponent<Engine>().Power(Mathf.Min( ((thisVertical) * 4F), 4F));
				networkView.RPC("Engines", RPCMode.Server, thisHorizontal, thisVertical);
			}
			else{
				//animate and move the ship
				rightEngine.GetComponent<Engine>().Power(Mathf.Min( ((0 - thisVertical - thisHorizontal * 2) * 0.2F), 0.2F));
				leftEngine.GetComponent<Engine>().Power(Mathf.Min( ((thisHorizontal * 2 - thisVertical) * 0.2F), 0.2F));
				mainEngine.GetComponent<Engine>().Power(Mathf.Min( ((thisVertical) * 0.4F), 0.4F));
				if(vertical > 0){
					if(rigidbody.velocity.magnitude < maxSpeed){
						rigidbody.AddRelativeForce(0,thisVertical * acceleration,0);
					}
					else{
						rigidbody.AddRelativeForce(0,thisVertical * acceleration,0);
						rigidbody.velocity *= maxSpeed / rigidbody.velocity.magnitude;
					}
				}
				else{
					rigidbody.AddRelativeForce(0,thisVertical * acceleration / 2,0);
				}
				transform.Rotate(0,0, - thisHorizontal * acceleration / 10);
			}
		}
	}
	
	[RPC] public void Action(string position){
		if(thisPlayerState == PlayerState.Fighting){
			switch(position){
			case "up":
				up.GetComponent<ComponentScript>().Action();
				break;
			case "right":
				right.GetComponent<ComponentScript>().Action();
				break;
			case "left":
				left.GetComponent<ComponentScript>().Action();
				break;
			default:
				Debug.Log ("error - bad position send to action script in ship controls");
				break;
			}
			if(is_client){
				networkView.RPC("Action", RPCMode.Server, position);
			}
		}
		if(thisPlayerState == PlayerState.Building){
			if(is_client){
				switch(position){
				case "up":
					up.GetComponent<ComponentScript>().Action();
					upLevel = Mathf.RoundToInt(up.GetComponent<ComponentScript>().componentLevel);
					upType = up.GetComponent<ComponentScript>().thisType.ToString();
					break;
				case "right":
					right.GetComponent<ComponentScript>().Action();
					rightLevel = Mathf.RoundToInt(right.GetComponent<ComponentScript>().componentLevel);
					rightType = right.GetComponent<ComponentScript>().thisType.ToString();
					break;
				case "left":
					left.GetComponent<ComponentScript>().Action();
					leftLevel = Mathf.RoundToInt(left.GetComponent<ComponentScript>().componentLevel);
					leftType = left.GetComponent<ComponentScript>().thisType.ToString();
					break;
				case "engine":
					EngineUpgrade();
					break;
				case "money":
					FactoryUpgrade();
					break;
				case "health":
					HealthUpgrade();
					break;
				default:
					Debug.Log ("error - bad position send to action script in ship controls");
					break;
				}
			}
		}
	}
	
	void EngineUpgrade(){
		if(playerMoney >= Mathf.RoundToInt(Mathf.Pow (1.5F, engineLevel) * 15)){
			acceleration *= engineMultiplier;
			maxSpeed *= engineMultiplier;
			playerMoney -= Mathf.RoundToInt(Mathf.Pow (1.5F, engineLevel) * 15);
			engineLevel++;
		}
	}
	
	void FactoryUpgrade(){
		if(playerMoney >= Mathf.RoundToInt(Mathf.Pow (1.5F, moneyLevel) * 15)){
			moneyRate /= moneyMultiplier;
			playerMoney -= Mathf.RoundToInt(Mathf.Pow (1.5F, moneyLevel) * 15);
			moneyLevel++;
		}
	}
	
	void HealthUpgrade(){
		if(playerMoney >= Mathf.RoundToInt(Mathf.Pow (1.5F, healthLevel) * 15)){
			maxHealth *= healthMultiplier;
			playerMoney -= Mathf.RoundToInt(Mathf.Pow (1.5F, healthLevel) * 15);
			healthLevel++;
		}
	}
	
	[RPC] public void Spawn(int newUpLevel, string newUpType, int newRightLevel, string newRightType, int newLeftLevel, string newLeftType, int newEngineLevel, float r1, float g1, float b1, float r2, float g2, float b2, string thisCharacter){
		thisPlayerState = PlayerState.Fighting;
		if(is_client){
			playerHealth = maxHealth;
			//change ui and send message to launch, change state
			networkView.RPC("Spawn", RPCMode.Server, upLevel, upType, rightLevel, rightType, leftLevel, leftType, engineLevel, r1, g1, b1, r2, g2, b2, thisCharacter);
		}
		else{
			transform.parent = GameObject.Find("Planets").transform;
			Color thisCharacterColor = new Color(r1, g1, b1, 1);
			Color thisShipColor = new Color(r2, g2, b2, 1);
			Customize(thisShipColor, thisCharacterColor, thisCharacter);
			for(int i = 0; i < transform.childCount; i++){
				Transform thisChild = transform.GetChild(i);
				if(thisChild.renderer != null){
					thisChild.renderer.enabled = true;
				}
				if(thisChild.collider != null){
					thisChild.collider.enabled = true;
				}
				for(int j = 0; j < thisChild.childCount; j++){
					if(thisChild.GetChild(j).renderer != null){
						thisChild.GetChild(j).renderer.enabled = true;
					}
					if(thisChild.GetChild(j).collider != null){
						thisChild.GetChild(j).collider.enabled = true;
					}
				}
			}
			if(renderer != null){
				renderer.enabled = true;
			}
			if(collider != null){
				collider.enabled = true;
			}
			up.GetComponent<ComponentScript>().SetComponent(newUpType, newUpLevel);
			right.GetComponent<ComponentScript>().SetComponent(newRightType, newRightLevel);
			left.GetComponent<ComponentScript>().SetComponent(newLeftType, newLeftLevel);
			engineLevel = 0;
			for(int i = 0; i < newEngineLevel; i++){
				acceleration *= engineMultiplier;
				maxSpeed *= engineMultiplier;
				engineLevel++;
			}
		}
	}
	
	[RPC] void Damage(float damage){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				//reduce health and update text, check if dead
				playerHealth -= damage;
				if (playerHealth <= 0){
					Death();
				}
			}
			else{
				//send damage to client
				networkView.RPC("Damage", networkView.owner, damage);
			}
		}
	}
	
	[RPC] public void Death(){
		if(is_client){
			thisPlayerState = PlayerState.Building;
			up.GetComponent<ComponentScript>().Reset();
			right.GetComponent<ComponentScript>().Reset();
			left.GetComponent<ComponentScript>().Reset();
			engineLevel = 0;
			healthLevel = 0;
			maxHealth = 100;
			acceleration = 5;
			maxSpeed = 1.5F;
			networkView.RPC("Death", RPCMode.Server);
		}
		else{
			thisPlayerState = PlayerState.Building;
			for(int i = 0; i < transform.childCount; i++){
				Transform thisChild = transform.GetChild(i);
				if(thisChild.renderer != null){
					thisChild.renderer.enabled = false;
				}
				if(thisChild.collider != null){
					thisChild.collider.enabled = false;
				}
				for(int j = 0; j < thisChild.childCount; j++){
					if(thisChild.GetChild(j).renderer != null){
						thisChild.GetChild(j).renderer.enabled = false;
					}
					if(thisChild.GetChild(j).collider != null){
						thisChild.GetChild(j).collider.enabled = false;
					}
				}
			}
			if(renderer != null){
				renderer.enabled = false;
			}
			if(collider != null){
				collider.enabled = false;
			}
		}
	}
	
	[RPC] void GetMoney(int moneyAmount){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				playerMoney+= moneyAmount;
			}
			else{
				networkView.RPC("GetMoney", networkView.owner, moneyAmount);
			}
		}
	}
	
	[RPC] void GetXP(int xpAmount){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				playerXP+= xpAmount;
			}
			else{
				networkView.RPC("GetXP", networkView.owner, xpAmount);
			}
		}
	}
	
	[RPC] public void SetState(string stateString){
		if(is_client){
			networkView.RPC("SetState", RPCMode.Server, stateString);
		}
		switch(stateString){
		case "roundEnded":
			thisPlayerState = PlayerState.RoundEnded;
			break;
		case "building":
			thisPlayerState = PlayerState.Building;
			break;
		case "fighting":
			thisPlayerState = PlayerState.Fighting;
			break;
		case "launching":
			thisPlayerState = PlayerState.Launching;
			break;
		case "menu":
			thisPlayerState = PlayerState.Menu;
			break;
		case "recovering":
			thisPlayerState = PlayerState.Recovering;
			break;
		default:
			Debug.Log ("error in setting state in the ship controls script");
			thisPlayerState = PlayerState.Fighting;
			break;
		}
	}
	
	void OnCollisionEnter(Collision collision){
		if(collision.transform.GetComponent<Projectile>() != null){
			Damage(collision.transform.GetComponent<Projectile>().damage);
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.name == "RHBounds" && rigidbody.velocity.x > 0){
			transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
		}
		else if(other.name == "LHBounds" && rigidbody.velocity.x < 0){
			transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
		}
		else if(other.name == "TVBounds" && rigidbody.velocity.y > 0){
			transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
		}
		else if(other.name == "BVBounds" && rigidbody.velocity.y < 0){
			transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(other.name == "Up" || other.name == "Right" || other.name == "Left"){
			if (other.GetComponent<ComponentScript>().thisType == ComponentType.Ram){
				Damage(other.GetComponent<ComponentScript>().componentLevel * 30);
			}
		}
	}
	
	void DPad(){
		if(thisPlayerState == PlayerState.Fighting){
			for (int i = 0; i < Input.touchCount; i++){
				Touch touch = Input.GetTouch(i);
				if(touch.phase == TouchPhase.Began && Camera.main.ScreenToWorldPoint(touch.position).x < -2){
					dpadTouch = touch.fingerId;
					initialX = touch.position.x;
					initialY = touch.position.y;
				}
				is_dpadUse = true;
			}
			if(dpadTouch > -1){
				if(Input.GetTouch(dpadTouch).phase == TouchPhase.Canceled || Input.GetTouch(dpadTouch).phase == TouchPhase.Ended){
					is_dpadUse = false;
				}
				if(is_dpadUse){
					Touch touch = Input.GetTouch(dpadTouch);
					if(touch.position.x - initialX > 100){
						horizontal = 1;
					}
					else if(touch.position.x - initialX < -100){
						horizontal = -1;
					}
					else{
						horizontal = (touch.position.x - initialX)/100;
					}
					if(touch.position.y - initialY > 100){
						vertical = 1;
					}
					else if(touch.position.y - initialY < -100){
						vertical = -1;
					}
					else{
						vertical = (touch.position.y - initialY)/100;
					}
				}
				else{
					vertical = 0;
					horizontal = 0;
				}
			}
		}
		if(thisPlayerState == PlayerState.Fighting){
			if(Input.GetMouseButtonDown(0) && Camera.main.ScreenToWorldPoint(Input.mousePosition).x < -2){
				is_dpadUse = true;
				initialX = Input.mousePosition.x;
				initialY = Input.mousePosition.y;
			}
			if(Input.GetMouseButtonUp(0)){
				is_dpadUse = false;
			}
			if(is_dpadUse){
				if(Input.mousePosition.x - initialX > 100){
					horizontal = 1;
				}
				else if(Input.mousePosition.x - initialX < -100){
					horizontal = -1;
				}
				else{
					horizontal = (Input.mousePosition.x - initialX)/100;
				}
				if(Input.mousePosition.y - initialY > 100){
					vertical = 1;
				}
				else if(Input.mousePosition.y - initialY < -100){
					vertical = -1;
				}
				else{
					vertical = (Input.mousePosition.y - initialY)/100;
				}
			}
			else{
				vertical = 0;
				horizontal = 0;
			}
		}
	}
	
	public void Customize(Color shipColor, Color characterColor, string character){
		renderer.material.color = shipColor;
		transform.FindChild("Character").GetComponent<TextMesh>().color = characterColor;
		transform.FindChild("Character").GetComponent<TextMesh>().text = character;
	}
}