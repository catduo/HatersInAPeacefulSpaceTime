using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {
	
	public float orbitSpeed;
	public Transform centerObject;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(centerObject.position, new Vector3(0,0,1), orbitSpeed);
	}
}
