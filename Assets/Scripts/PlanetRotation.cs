using UnityEngine;
using System.Collections;

public class PlanetRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate(0.2F, 0.2F, 0.2F);
	}
}
