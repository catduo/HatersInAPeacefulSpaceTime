using UnityEngine;
using System.Collections;

public class DPad : MonoBehaviour {
	
	private bool held = false;
	private bool heldPrevious = false;
	static public float vertical = 0;
	static public float horizontal = 0;
	public bool binaryInputs;
	public float diagonalSensitivity;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(heldPrevious){
			vertical = (GameState.InputXYs[0].y - transform.position.y) / transform.lossyScale.z * 0.2F;
			horizontal = (GameState.InputXYs[0].x - transform.position.x) / transform.lossyScale.x * 0.2F;
			if(binaryInputs){
				if(vertical > diagonalSensitivity * Mathf.Abs(horizontal)){
					vertical = 1;
				}
				else if(vertical < - diagonalSensitivity * Mathf.Abs(horizontal)){
					vertical = -1;
				}
				else{
					vertical = 0;
				}
				if(horizontal > diagonalSensitivity * Mathf.Abs(vertical)){
					horizontal = 1;
				}
				else if(horizontal < - diagonalSensitivity * Mathf.Abs(vertical)){
					horizontal = -1;
				}
				else{
					horizontal = 0;
				}
			}
		}
		else{
			vertical = 0;
			horizontal = 0;
		}
		if(!held){
			heldPrevious = false;
		}
		held = false;
	}
	
	void Hold () {
		held = true;
		heldPrevious = true;
	}
	
	void Tap () {
		Hold();
	}
}
