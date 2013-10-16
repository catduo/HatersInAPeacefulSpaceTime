using UnityEngine;
using System.Collections;

public class NetworkAndMenu : MonoBehaviour {
	
	private const string typeName = "MiniBrawl";
	private const string gameName = "MiniBrawl";
	private float lastTry;
	private float retryTime = 1;
	static public bool is_online = false;
	static public bool is_local = false;
	private bool is_hosting = false;
	private bool is_connecting = false;
	private bool is_instructions = false;
	private bool is_decidingToHost = false;
	static public bool is_menu = true;
	static public bool is_gameOver = false;
	private bool is_credits = false;
	public Font font;
	public GameObject playerObject;
	 
	private void StartServer(){
	    Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
	    MasterServer.RegisterHost(typeName, gameName);
	}
	
	void OnServerInitialized(){
	    Debug.Log("Server Initializied");
		Network.SetSendingEnabled(0, false);
	}
	
	public void Disconnect () {
		Network.Disconnect();
		is_online = false;
	}
	
	// Use this for initialization
	void Start () {
		lastTry = Time.time;
		if(Application.loadedLevelName == "Game"){
			is_hosting = true;
			StartServer();
			is_online = true;
			is_decidingToHost = false;
			is_menu = false;
		}
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info){
		is_menu = true;
		is_online = false;
		//GameObject.Find ("MainCamera").GetComponent<FollowCharacter>().StopCharacterFollow();
	}
	
	private HostData[] hostList = new HostData[] {};
	 
	private void RefreshHostList(){
	    MasterServer.RequestHostList(typeName);
	}
	 
	void OnMasterServerEvent(MasterServerEvent msEvent){
	    if (msEvent == MasterServerEvent.HostListReceived){
	        hostList = MasterServer.PollHostList();
			if(is_connecting){
				Debug.Log ("try to connect");
				if(hostList.Length > 0){
					JoinServer(hostList[0]);
					is_menu = false;
				}
				else{
					RefreshHostList();
				}
			}
		}
	}
	private void JoinServer(HostData hostData){
	    Network.Connect(hostData);
	}
	 
	void OnConnectedToServer(){
	    Debug.Log("Server Joined");
		is_menu = false;
		is_connecting = false;
		GameObject thisShip = (GameObject) Network.Instantiate(playerObject, Vector3.zero, Quaternion.identity, 0);
		thisShip.GetComponent<ShipControls>().thisPlayerState = PlayerState.Fighting;
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
    {
       Network.RemoveRPCs(player);
       Network.DestroyPlayerObjects(player);
    }
	
	void OnGUI(){
		GUI.skin.font = font;
		if (is_hosting){
			if (GUI.Button(new Rect(0, 0, 100, 50), "Stop")){
				Disconnect();
				Application.LoadLevel("Controller");
			}
		}
		else if(is_gameOver){
			GUI.Box(new Rect(300,100,250,100), "Game Over");
			if (GUI.Button(new Rect(300,300,250,100), "Main Menu")){
				is_gameOver = false;
				is_online = false;
				is_local = false;
				is_menu = true;
			}
		}
		else if (is_credits){
			GUI.Box(new Rect(150,100,600,300), "Thank you for Playing!\nDesigner and Developer: David Geisert\n\nFor information contact dg@catduo.com\n or visit catduo.com");
			if (GUI.Button(new Rect(300,400,250,100), "Main Menu")){
				is_gameOver = false;
				is_online = false;
				is_local = false;
				is_menu = true;
				is_credits = false;
			}
		}
		else if(is_instructions){
	        if (GUI.Button(new Rect(300, 560, 250, 40), "")){
				is_instructions = false;
				is_menu = true;
			}
		}
		else if(is_connecting){
			GUI.Box(new Rect(300,20,250,40), "Mini Brawl!");
			GUI.Box(new Rect(330, 100, 190, 50), "Looking for Players...");
			if (GUI.Button(new Rect(350, 200, 150, 50), "Stop Looking")){
				is_connecting = false;
				is_online = false;
				Disconnect();
			}
	        if (GUI.Button(new Rect(350, 300, 150, 50), "Instructions")){
				is_instructions = true;
			}
	        if (GUI.Button(new Rect(350, 400, 150, 50), "Credits")){
				is_credits = true;
			}
	        if (GUI.Button(new Rect(600, 300, 100, 50), "Mute")){
				if(AudioListener.volume == 1){
					AudioListener.volume = 0;
				}
				else{
					AudioListener.volume = 1;
				}
			}
		}
		else if(is_decidingToHost){
	        if (GUI.Button(new Rect(350, 100, 150, 50), "Play!")){
	            is_online = true;
				is_connecting = true;
				is_decidingToHost = false;
				RefreshHostList();
			}
	        if (GUI.Button(new Rect(350, 200, 150, 50), "Host as Server")){
				Application.LoadLevel("Game");
			}
	        if (GUI.Button(new Rect(350, 300, 150, 50), "Back")){
				is_decidingToHost = false;
				is_menu = true;
			}
		}
		else if(is_menu){
			GUI.Box(new Rect(300,20,250,40), "Nuke 'Em From Orbit!");
	        if (GUI.Button(new Rect(350, 100, 150, 50), "Play Online")){
	            is_online = true;
				is_decidingToHost = true;
			}
	        if (GUI.Button(new Rect(350, 200, 150, 50), "Single Player")){
				is_local = true;
				is_menu = false;
				GameObject thisShip = (GameObject) GameObject.Instantiate(playerObject, Vector3.zero, Quaternion.identity);
				thisShip.GetComponent<ShipControls>().thisPlayerState = PlayerState.Fighting;
			}
	        if (GUI.Button(new Rect(350, 300, 150, 50), "Instructions")){
				is_instructions = true;
			}
	        if (GUI.Button(new Rect(350, 400, 150, 50), "Credits")){
				is_credits = true;
			}
	        if (GUI.Button(new Rect(600, 300, 100, 50), "Mute")){
				if(AudioListener.volume == 1){
					AudioListener.volume = 0;
				}
				else{
					AudioListener.volume = 1;
				}
			}
		    if (GUI.Button(new Rect(0, 0, 100, 50), "Menu")){
				is_menu = false;
			}
		}
	    else if (GUI.Button(new Rect(0, 0, 100, 50), "Menu")){
			is_menu = true;
		}
	}
}
