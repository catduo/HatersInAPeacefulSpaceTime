using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour {
	
	private ParticleSystem particleSystem;
	private bool is_burning = false;
	private float size;
	
	// Use this for initialization
	void Start () {
		particleSystem = GetComponentInChildren<ParticleSystem>();
		size = particleSystem.startSize;
	}
	
	// Update is called once per frame
	void Update () {
		if(is_burning){
			particleSystem.Play();
		}
		else{
			particleSystem.Stop();
		}
		is_burning = false;
	}
	
	public void Power(float power){
		if(power > 0){
			is_burning = true;
			particleSystem.startSpeed = power * 5;
			particleSystem.startSize = size * power * 4;
		}
		else{
			is_burning = false;
		}
	}
}
