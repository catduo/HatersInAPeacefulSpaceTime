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
	
	private Transform up;
	private Transform left;
	private Transform right;
	private Transform mainEngine;
	private Transform rightEngine;
	private Transform leftEngine;
	
	private Transform statusArea;
	
	private string upType;
	private string rightType;
	private string leftType;
	private int upLevel;
	private int rightLevel;
	private int leftLevel;
	private int engineLevel;
	private float maxSpeed = 1.5F;
	private float acceleration = 5;
	
	public float planetaryHealth = 100;
	public float maxHealth = 100;
	private TextMesh healthText;
	
	public int playerMoney;
	private TextMesh moneyText;
	private float moneyRate = 1;
	private float lastMoneyTime;
	
	public int playerXP;
	private TextMesh xpText;
	
	private Vector2 engineFiring;
	private bool is_client = false;
	
	// Use this for initialization
	void Start () {
		if(PlayerPrefs.GetInt("PlayerMoney") != null){
			playerMoney = PlayerPrefs.GetInt("PlayerMoney");
		}
		else{
			playerMoney = 0;
		}
		if(PlayerPrefs.GetInt("PlayerXP") != null){
			playerXP = PlayerPrefs.GetInt("PlayerXP");
		}
		else{
			playerXP = 0;
		}
		if(networkView.isMine){
			is_client = true;
		}
		up = transform.FindChild("Up");
		left = transform.FindChild("Left");
		right = transform.FindChild("Right");
		mainEngine = transform.FindChild("MainEngine");
		rightEngine = transform.FindChild("RightEngine");
		leftEngine = transform.FindChild("LeftEngine");
		statusArea = GameObject.Find ("StatusArea").transform;
		healthText = statusArea.FindChild("HealthText").GetComponent<TextMesh>();
		moneyText = statusArea.FindChild("Money").GetComponent<TextMesh>();
		lastMoneyTime = Time.time - moneyRate;
	}
	
	void FixedUpdate(){
		Engines();
	}
	
	// Update is called once per frame
	void Update () {
		Income ();
	}
	
	void Income(){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				playerMoney++;
				moneyText.text = "&" + playerMoney.ToString();
			}
		}
	}
	
	void Engines(){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				//show animations, but don't move anything
				rightEngine.GetComponent<Engine>().Power(Mathf.Min( ((0 - DPad.vertical - DPad.horizontal * 2) * 0.2F), 0.2F));
				leftEngine.GetComponent<Engine>().Power(Mathf.Min( ((DPad.horizontal * 2 - DPad.vertical) * 0.2F), 0.2F));
				mainEngine.GetComponent<Engine>().Power(Mathf.Min( ((DPad.vertical) * 0.4F), 0.4F));
				transform.Rotate(0,0, - DPad.horizontal);
			}
			else{
				//animate and move the ship
				rightEngine.GetComponent<Engine>().Power(Mathf.Min( ((0 - DPad.vertical - DPad.horizontal * 2) * 0.2F), 0.2F));
				leftEngine.GetComponent<Engine>().Power(Mathf.Min( ((DPad.horizontal * 2 - DPad.vertical) * 0.2F), 0.2F));
				mainEngine.GetComponent<Engine>().Power(Mathf.Min( ((DPad.vertical) * 0.4F), 0.4F));
				if(DPad.vertical > 0){
					if(rigidbody.velocity.magnitude < maxSpeed){
						rigidbody.AddRelativeForce(0,DPad.vertical * acceleration,0);
					}
					else{
						rigidbody.AddRelativeForce(0,DPad.vertical * acceleration,0);
						rigidbody.velocity *= maxSpeed / rigidbody.velocity.magnitude;
					}
				}
				else{
					rigidbody.AddRelativeForce(0,DPad.vertical * acceleration / 2,0);
				}
				transform.Rotate(0,0, - DPad.horizontal);
			}
		}
	}
	
	[RPC] void Action(string position){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				networkView.RPC("Action", RPCMode.Server);
			}
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
		}
		if(thisPlayerState == PlayerState.Building){
			if(is_client){
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
		
	}
	
	void FactoryUpgrade(){
		
	}
	
	void HealthUpgrade(){
		
	}
	
	[RPC] void Spawn(){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				//change ui and send message to launch, change state
				networkView.RPC("Spawn", RPCMode.Server);
				thisPlayerState = PlayerState.Launching;
			}
			else{
				//launch ship
			}
		}
	}
	
	[RPC] void Damage(float damage){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				//reduce health and update text, check if dead
			}
			else{
				//reduce health and send damage to client, check if dead
				networkView.RPC("Damage", networkView.owner, damage);
			}
		}
	}
	
	[RPC] void GetMoney(int moneyAmount){
		if(thisPlayerState == PlayerState.Fighting){
			if(is_client){
				playerMoney+= moneyAmount;
				moneyText.text = "&" + playerMoney.ToString();
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
				xpText.text = playerXP.ToString() + "XP";
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
	
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 eulerAngles = Vector3.zero;
		float vertical = 0;
		float horizontal = 0;
        if (stream.isWriting) {
			vertical = DPad.vertical;
			horizontal = DPad.horizontal;
            eulerAngles = transform.eulerAngles;
			
            stream.Serialize(ref vertical);
            stream.Serialize(ref horizontal);
            stream.Serialize(ref eulerAngles);
        } else {
            stream.Serialize(ref vertical);
            stream.Serialize(ref horizontal);
            stream.Serialize(ref eulerAngles);
			
			vertical = DPad.vertical;
			horizontal = DPad.horizontal;
            eulerAngles = transform.eulerAngles;
        }
    }
}