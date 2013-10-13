using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	
	private Vector2 dPadInput = new Vector2(0,0);
	private Vector3 thisPosition = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetInput();
		thisPosition = new Vector3(dPadInput.x, dPadInput.y, 0);
		SetPosition(thisPosition);
	}
	
	void GetInput(){
		dPadInput = new Vector2(DPad.horizontal * 5 + 3, DPad.vertical * 5);
	}
	
	[RPC] void SetPosition(Vector3 rpcInput){
		transform.position = rpcInput;
		if(Network.isClient){
			networkView.RPC("SetPosition", RPCMode.Server, rpcInput);
		}
	}
}
