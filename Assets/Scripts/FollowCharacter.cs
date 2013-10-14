using UnityEngine;
using System.Collections;

public class FollowCharacter : MonoBehaviour {
	
	private Transform playerCharacter;
	private bool is_characterFollowSet = false;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(is_characterFollowSet){
			transform.position = playerCharacter.position - new Vector3( 0, 0, 15);
		}
	}
	
	public void SetCharacterFollow(Transform character){
		is_characterFollowSet = true;
		playerCharacter = character;
	}
	
	public void StopCharacterFollow(){
		is_characterFollowSet = false;
	}
}
