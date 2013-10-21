using UnityEngine;
using System.Collections;
using System;

public class NetworkAndMenu : MonoBehaviour {
	private const string typeName = "MiniBrawl";
	static public string gameName = "Enter Game ID";
	private float lastTry;
	private float retryTime = 1;
	static public bool is_online = false;
	static public bool is_local = false;
	private bool is_hosting = false;
	private bool is_connecting = false;
	private bool failedToJoin = false;
	private bool is_instructions = false;
	private bool is_decidingToHost = false;
	private bool is_controller = false;
	private bool is_customizing = false;
	private bool has_serverName = false;
	static public bool is_menu = true;
	static public bool is_gameOver = false;
	private bool is_credits = false;
	public Font font;
	public GameObject shipObject;
	private GameObject playerObject;
	private String joinServerCode;
	
	private Color shipColor = new Color(1,1,1,1);
	private Color characterColor = new Color(1,1,1,1);
	private string character = "D";
	private float r1 = 1;
	private float r2 = 1;
	private float g1 = 1;
	private float g2 = 1;
	private float b1 = 1;
	private float b2 = 1;
	
	static public ComponentType selectedType;
	public Texture2D cannonTexture;
	public Texture2D rocketTexture;
	public Texture2D laserTexture;
	public Texture2D shieldTexture;
	public Texture2D ramTexture;
	public Texture2D repairTexture;
	public Texture2D selected;
	private string componentDescription;
	 
	private void StartServer(){
	    Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		gameName = Guid.NewGuid().ToString().Split('-')[1];
		Debug.Log (gameName);
	    MasterServer.RegisterHost(typeName, gameName);
	}
	
	void OnServerInitialized(){
	    Debug.Log("Server Initializied");
	}
	
	public void Disconnect () {
		Network.Disconnect();
		is_online = false;
	}
	
	// Use this for initialization
	void Start () {
		MasterServer.ipAddress = "192.168.1.57";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "192.168.1.57";
		Network.natFacilitatorPort = 23466;
		Application.targetFrameRate = 24;
		lastTry = Time.time;
		if(Application.loadedLevelName == "Game"){
			is_hosting = true;
			StartServer();
			is_online = true;
			is_decidingToHost = false;
			is_menu = false;
		}
		selected = cannonTexture;
		selectedType = ComponentType.Cannon;
		componentDescription = DescriptorText();
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info){
		is_menu = true;
		is_online = false;
		is_controller = false;
		has_serverName = false;
		Destroy(playerObject);
	}
	
	private void RefreshHostList(){
	    MasterServer.RequestHostList(typeName);
	}
	 
	void OnMasterServerEvent(MasterServerEvent msEvent){
	    if (msEvent == MasterServerEvent.HostListReceived){
			if(is_connecting){
				is_connecting = false;
				HostData[] hostList = MasterServer.PollHostList();
				foreach (HostData hostData in hostList) {
					if (hostData.gameName.Equals(joinServerCode)) {
						Network.Connect(hostData);
						return;
					}
				}

				failedToJoin = true;
			}
		}
	}
	
	private void JoinServer(String serverCode){
		failedToJoin = false;
		joinServerCode = serverCode;
		is_connecting = true;
		RefreshHostList();
	}
	 
	void OnConnectedToServer(){
	    Debug.Log("Server Joined");
		is_menu = false;
		is_connecting = false;
		Destroy(playerObject);
		playerObject = (GameObject) Network.Instantiate(shipObject, new Vector3(-5, 5, 0), Quaternion.identity, 0);
		playerObject.GetComponent<ShipControls>().SetState("building");
		is_controller = true;
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
    {
       Network.RemoveRPCs(player);
       Network.DestroyPlayerObjects(player);
    }
	
	void OnGUI(){
		GUI.skin.font = font;
		if (is_hosting){
			if (GUI.Button(WorldRect(new Rect(-13,10,3,2)), "Stop")){
				Disconnect();
				Application.LoadLevel("Controller");
			}
			GUI.Box(WorldRect(new Rect(-13,8,4,2)), "Game: " + gameName);
		}
		else if(is_gameOver){
			GUI.Box(WorldRect(new Rect(-3,8,6,2)), "Game Over");
			if (GUI.Button(WorldRect(new Rect(-3,5,6,2)), "Main Menu")){
				is_gameOver = false;
				is_online = false;
				is_local = false;
				is_menu = true;
			}
		}
		else if (is_credits){
			GUI.Box(WorldRect(new Rect(-10,8,20,10)), "Thank you for Playing!\nDesigner and Developer: David Geisert\n\nFor information contact dg@catduo.com\n or visit catduo.com");
			if (GUI.Button(WorldRect(new Rect(-3,-2,6,2)), "Main Menu")){
				is_gameOver = false;
				is_online = false;
				is_local = false;
				is_menu = true;
				is_credits = false;
			}
		}
		else if(is_instructions){
	        if (GUI.Button(WorldRect(new Rect(-3,-8,6,2)), "Return to Menu")){
				is_instructions = false;
				is_menu = true;
			}
		}
		else if(is_connecting){
			if(has_serverName){
				GUI.Box(WorldRect(new Rect(-4,10,8,1)), "Haters Gonna Hate");
				GUI.Box(WorldRect(new Rect(-3,8,6,2)), "Looking for Server...");
				if (GUI.Button(WorldRect(new Rect(-3,5,6,2)), "Stop Looking")){
					is_connecting = false;
					is_online = false;
					Disconnect();
				}
		        if (GUI.Button(WorldRect(new Rect(-3,2,6,2)), "Instructions")){
					is_instructions = true;
				}
		        if (GUI.Button(WorldRect(new Rect(-3,-1,6,2)), "Credits")){
					is_credits = true;
				}
		        if (GUI.Button(WorldRect(new Rect(-3,-4,6,2)), "Mute")){
					if(AudioListener.volume == 1){
						AudioListener.volume = 0;
					}
					else{
						AudioListener.volume = 1;
					}
				}
			}
			else{
				GUI.Box(WorldRect(new Rect(-4,6,8,10)), "Type in Host Code");
				gameName = GUI.TextField(WorldRect(new Rect(-2,4,2,1)), gameName);
		        if (GUI.Button(WorldRect(new Rect(-3,2,6,2)), "Back")){
					is_decidingToHost = false;
					is_menu = true;
				}
		        if (GUI.Button(WorldRect(new Rect(-3,-1,6,2)), "Play!")){
					has_serverName = true;
					JoinServer(gameName);
				}
			}
		}
		else if(is_decidingToHost){
			GUI.Box(WorldRect(new Rect(-4,10,8,1)), "Haters Gonna Hate");
	        if (GUI.Button(WorldRect(new Rect(-3,8,6,2)), "Find Game!")){
	            is_online = true;
				is_connecting = true;
				is_decidingToHost = false;
			}
	        if (GUI.Button(WorldRect(new Rect(-3,5,6,2)), "Host as Server")){
				Application.LoadLevel("Game");
			}
	        if (GUI.Button(WorldRect(new Rect(-3,2,6,2)), "Back")){
				is_decidingToHost = false;
				is_menu = true;
			}
		}
		else if(is_menu){
			GUI.Box(WorldRect(new Rect(-4,10,8,1)), "Haters Gonna Hate");
	        if (GUI.Button(WorldRect(new Rect(-3,8,6,2)), "Play Online")){
	            is_online = true;
				is_decidingToHost = true;
			}
	        if (GUI.Button(WorldRect(new Rect(-3,5,6,2)), "Testing")){
				is_local = true;
				is_menu = false;
				is_controller = true;
				Destroy(playerObject);
				playerObject = (GameObject) GameObject.Instantiate(shipObject, Vector3.zero, Quaternion.identity);
				playerObject.GetComponent<ShipControls>().SetState("building");
			}
	        if (GUI.Button(WorldRect(new Rect(-3,2,6,2)), "Instructions")){
				is_instructions = true;
			}
	        if (GUI.Button(WorldRect(new Rect(-3,-1,6,2)), "Credits")){
				is_credits = true;
			}
	        if (GUI.Button(WorldRect(new Rect(-3,-4,6,2)), "Mute")){
				if(AudioListener.volume == 1){
					AudioListener.volume = 0;
				}
				else{
					AudioListener.volume = 1;
				}
			}
		    if (GUI.Button(WorldRect(new Rect(-13,10,3,1)), "Menu")){
				is_menu = false;
			}
		}
		else if(is_customizing){
			GUI.Box(WorldRect(new Rect(-10,8,20,10)), "Choose your colors and character");
			if (GUI.Button(WorldRect(new Rect(-3,-2,6,2)), "Done")){
				is_customizing = false;
				GameObject.Find ("PlayerShip(Clone)").GetComponent<ShipControls>().Customize(shipColor, characterColor, character);
			}
			GUI.TextArea(WorldRect(new Rect(-9,7,4,1)), "Choose a Character");
			character = GUI.TextField(WorldRect(new Rect(-5,7,2,1)), character);
			GUI.TextArea(WorldRect(new Rect(-9,5,8,1)), "Character Color (RGB)");
			r1 = GUI.VerticalSlider(WorldRect(new Rect(-9,3,2,4)), r1, 1, 0);
			g1 = GUI.VerticalSlider(WorldRect(new Rect(-6,3,2,4)), g1, 1, 0);
			b1 = GUI.VerticalSlider(WorldRect(new Rect(-3,3,2,4)), b1, 1, 0);
			GUI.TextArea(WorldRect(new Rect(-0,5,8,1)), "Ship Color (RGB)");
			r2 = GUI.VerticalSlider(WorldRect(new Rect(0,3,2,4)), r2, 1, 0);
			g2 = GUI.VerticalSlider(WorldRect(new Rect(3,3,2,4)), g2, 1, 0);
			b2 = GUI.VerticalSlider(WorldRect(new Rect(6,3,2,4)), b2, 1, 0);
			shipColor = new Color(r2, g2, b2, 1);
			characterColor = new Color (r1, g1, b1, 1);
			character = character[0].ToString();
		}
		else if(is_controller){
			switch(playerObject.GetComponent<ShipControls>().thisPlayerState){
			case PlayerState.Building:
			    if (GUI.Button(WorldRect(new Rect(-8,10,5,2)), "Fight")){
					playerObject.GetComponent<ShipControls>().Spawn(0, "", 0, "", 0, "", 0, r1, g1, b1, r2, g2, b2, character);
				}
			    if (GUI.Button(WorldRect(new Rect(-13,8,2,2)), cannonTexture)){
					selectedType = ComponentType.Cannon;
					selected = cannonTexture;
					componentDescription = DescriptorText();
					playerObject.GetComponent<ShipControls>().up.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().right.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().left.GetComponent<ComponentScript>().ActionTextUpdate ();
				}
			    if (GUI.Button(WorldRect(new Rect(-11,8,2,2)), rocketTexture)){
					selectedType = ComponentType.Rocket;
					selected = rocketTexture;
					componentDescription = DescriptorText();
					playerObject.GetComponent<ShipControls>().up.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().right.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().left.GetComponent<ComponentScript>().ActionTextUpdate ();
				}
			    if (GUI.Button(WorldRect(new Rect(-9,8,2,2)), laserTexture)){
					selectedType = ComponentType.Laser;
					selected = laserTexture;
					componentDescription = DescriptorText();
					playerObject.GetComponent<ShipControls>().up.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().right.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().left.GetComponent<ComponentScript>().ActionTextUpdate ();
				}
			    if (GUI.Button(WorldRect(new Rect(-7,8,2,2)), ramTexture)){
					selectedType = ComponentType.Ram;
					selected = ramTexture;
					componentDescription = DescriptorText();
					playerObject.GetComponent<ShipControls>().up.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().right.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().left.GetComponent<ComponentScript>().ActionTextUpdate ();
				}
			    if (GUI.Button(WorldRect(new Rect(-5,8,2,2)), repairTexture)){
					selectedType = ComponentType.Repair;
					selected = repairTexture;
					componentDescription = DescriptorText();
					playerObject.GetComponent<ShipControls>().up.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().right.GetComponent<ComponentScript>().ActionTextUpdate ();
					playerObject.GetComponent<ShipControls>().left.GetComponent<ComponentScript>().ActionTextUpdate ();
				}
				GUI.DrawTexture(WorldRect(new Rect(-10,6,4,4)), selected);
				GUI.Box(WorldRect(new Rect(-13,2,10,12)), componentDescription);
			    if (GUI.Button(WorldRect(new Rect(3,0,5,5)), "Customize")){
					is_customizing = true;
				}
			    if (GUI.Button(WorldRect(new Rect(3,5,5,5)), playerObject.GetComponent<ShipControls>().up.GetComponent<ComponentScript>().actionWord)){
					playerObject.GetComponent<ShipControls>().Action("up");
				}
			    if (GUI.Button(WorldRect(new Rect(8,0,5,5)), playerObject.GetComponent<ShipControls>().right.GetComponent<ComponentScript>().actionWord)){
					playerObject.GetComponent<ShipControls>().Action("right");
				}
			    if (GUI.Button(WorldRect(new Rect(-2,0,5,5)), playerObject.GetComponent<ShipControls>().left.GetComponent<ComponentScript>().actionWord)){
					playerObject.GetComponent<ShipControls>().Action("left");
				}
				if (GUI.Button(WorldRect(new Rect(3,-5,5,5)), "&" + Mathf.RoundToInt(Mathf.Pow (1.5F, playerObject.GetComponent<ShipControls>().engineLevel) * 15).ToString() + "\nUpgrade Engine\nfor better\nManeuvering")){
					playerObject.GetComponent<ShipControls>().Action("engine");
				}
				if (GUI.Button(WorldRect(new Rect(-3,10,4,4)), "&" + Mathf.RoundToInt(Mathf.Pow (1.5F, playerObject.GetComponent<ShipControls>().healthLevel) * 15).ToString() + "\nUpgrade\nMax Health of\n" + playerObject.GetComponent<ShipControls>().maxHealth.ToString() + "\nto\n" + (playerObject.GetComponent<ShipControls>().maxHealth * 1.2F).ToString()) ){
					playerObject.GetComponent<ShipControls>().Action("health");
				}
				GUI.Box(WorldRect(new Rect(1,10,4,4)), "XP\n###/###");
				if (GUI.Button(WorldRect(new Rect(5,10,4,4)), "&" + Mathf.RoundToInt(Mathf.Pow (1.5F, playerObject.GetComponent<ShipControls>().moneyLevel) * 15).ToString() + "\nUpgrade\n&" + (1/playerObject.GetComponent<ShipControls>().moneyRate).ToString() + "/sec" + "\nto\n&" + (1.5F/playerObject.GetComponent<ShipControls>().moneyRate).ToString() + "/sec")){
					playerObject.GetComponent<ShipControls>().Action("money");
				}
				GUI.Box(WorldRect(new Rect(9,10,4,4)), "Money\n&" + playerObject.GetComponent<ShipControls>().playerMoney.ToString());
				break;
				
			case PlayerState.Fighting:
				GUI.Box(WorldRect(new Rect(-13,2,10,12)), "Use this area\nas a Dpad");
			    if (GUI.Button(WorldRect(new Rect(-8,10,5,2)), "Die")){
					playerObject.GetComponent<ShipControls>().Death();
				}
			    if (GUI.Button(WorldRect(new Rect(3,5,5,5)), "Fire Up")){
					playerObject.GetComponent<ShipControls>().Action("up");
				}
			    if (GUI.Button(WorldRect(new Rect(8,0,5,5)), "Fire Right")){
					playerObject.GetComponent<ShipControls>().Action("right");
				}
			    if (GUI.Button(WorldRect(new Rect(-2,0,5,5)), "Fire Left")){
					playerObject.GetComponent<ShipControls>().Action("left");
				}
				GUI.Box(WorldRect(new Rect(-3,10,4,4)), "Health\n" + playerObject.GetComponent<ShipControls>().playerHealth.ToString() + "/" + playerObject.GetComponent<ShipControls>().maxHealth.ToString());
				GUI.Box(WorldRect(new Rect(1,10,4,4)), "XP\n###/###");
				GUI.Box(WorldRect(new Rect(5,10,4,4)), "Income\n" + (1/playerObject.GetComponent<ShipControls>().moneyRate).ToString() + "/sec");
				GUI.Box(WorldRect(new Rect(9,10,4,4)), "Money\n&" + playerObject.GetComponent<ShipControls>().playerMoney.ToString());
				break;
				
			case PlayerState.Recovering:
			    if (GUI.Button(WorldRect(new Rect(5,10,3,1)), "Up")){
				}
				break;
				
			case PlayerState.Launching:
			    if (GUI.Button(WorldRect(new Rect(5,10,3,1)), "Up")){
				}
				break;
				
			case PlayerState.RoundEnded:
			    if (GUI.Button(WorldRect(new Rect(5,10,3,1)), "Up")){
				}
				break;
				
			default:
				break;
			}
		    if (GUI.Button(WorldRect(new Rect(-13,10,5,2)), "Menu")){
				is_menu = true;
			}
		}
	    else if (GUI.Button(WorldRect(new Rect(-13,10,3,1)), "Menu")){
			is_menu = true;
		}
	}
		
	
	Rect WorldRect(Rect rect){
		Vector3 pos;
		Vector3 dim;
		pos = Camera.main.WorldToScreenPoint(new Vector2(rect.x, -rect.y));
		dim = Camera.main.WorldToScreenPoint(new Vector2(rect.xMax, -rect.yMax));
		rect = new Rect(pos.x, pos.y, dim.x - pos.x, pos.y - dim.y);
		return rect;
	}
	
	string DescriptorText(){
		string descriptorText;
		switch(selectedType){
		case ComponentType.Cannon:
			descriptorText = "Cannons\nRapid Firing Rate\nAffected by Gravity\nLow Damage";
			break;
		case ComponentType.Laser:
			descriptorText = "Laser\nRapid Firing Rate\nNot Affected by Gravity\nVery Low Damage";
			break;
		case ComponentType.Rocket:
			descriptorText = "Rocket\nSlow Firing Rate\nAffected by Gravity\nHigh Damage";
			break;
		case ComponentType.Repair:
			descriptorText = "Repair Station\nHeals Small Amounts of Damage\nMust be Activated for Repairs";
			break;
		case ComponentType.Ram:
			descriptorText = "Ram\nCan be used to Ram Enemy Ships\nExtremely High Damage";
			break;
		case ComponentType.Shield:
			descriptorText = "Shield\nCan be used to Block Projectiles\nDoes not Affect Ram";
			break;
			
		default:
			descriptorText = "Choose an item from above\nto learn more about it";
			break;
		}
		return descriptorText;
	}
}
