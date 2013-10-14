using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	
	private Vector2 dPadInput = new Vector2(0,0);
	private Vector3 thisPosition = new Vector3(0,0,0);
	private CharacterController characterController;
	
	// Use this for initialization
	void Start () {
		if(networkView.isMine){
			characterController = GetComponent<CharacterController>();
			GameObject.Find ("MainCamera").GetComponent<FollowCharacter>().SetCharacterFollow(transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(networkView.isMine){
			GetInput();
			SetPosition(thisPosition);
		}
	}
	
	void GetInput(){
		dPadInput = new Vector2(DPad.horizontal * 5 + 3, DPad.vertical * 5);
	}
	
	void SetPosition(Vector3 rpcInput){
		characterController.Move(new Vector3(0, 0, DPad.vertical * 5));
		transform.Rotate(0, DPad.horizontal, 0);
	}
}
