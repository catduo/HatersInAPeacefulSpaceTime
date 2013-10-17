using UnityEngine;
using System.Collections;

public enum ComponentType{
	Empty,
	Ram,
	Shield,
	Repair,
	Rocket,
	Cannon,
	Laser
}

public class ComponentScript : MonoBehaviour {
	
	private Transform hudSlot;
	private Transform hudHighlight;
	
	public Material cannonMaterial;
	public Material rocketMaterial;
	public Material laserMaterial;
	public Material shieldMaterial;
	public Material ramMaterial;
	public Material repairMaterial;
	private Transform componentArt;
	
	public float cannonMultiplier = 1.5F;
	public float rocketMultiplier = 2;
	public float laserMultiplier = 1.2F;
	public float shieldMultiplier = 1.2F;
	public float ramMultiplier = 1.5F;
	public float repairMultiplier = 2;
	
	public GameObject cannon1;
	public GameObject rocket1;
	public GameObject laser1;
	private Vector3 fireDirection;
	public ComponentType thisType = ComponentType.Empty;
	public float componentLevel = 0;
	public int cost;
	public string actionWord = "Construct in";
	
	private float componentCooldown = 0.1F;
	public float lastComponentUse;
	public bool is_componentReady = true;
	private Transform progressBar;
	
	private bool is_actionDelayed = false;
	private float delayTime = 0;
	private float delayDuration = 0;
	
	public AudioClip cannonFire;
	public AudioClip laserFire;
	public AudioClip rocketFire;
	public AudioClip shieldActivate;
	public AudioClip ramHit;
	public AudioClip repairActivate;
	public AudioClip construction;
	public AudioClip notEnoughFunds;
	private AudioSource audioSource;
	
	void Start () {
		audioSource = GetComponent<AudioSource>();
		componentArt = transform.FindChild("Art");
		componentArt.renderer.enabled = false;
		//progressBar = transform.FindChild("ProgressBar");
		//progressBar.GetComponent<ProgressBar>().measure = 1;
		//progressBar.GetComponent<ProgressBar>().measureCap = 1;
		lastComponentUse = Time.time - componentCooldown;
		cost = 15;
		actionWord = "Construct " + NetworkAndMenu.selectedType.ToString() + "\nin " + name + "\nfor &" + cost.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if(!is_componentReady){
			ComponentReady();
		}
	}
	
	public void ActionTextUpdate(){
		actionWord = "Construct " + NetworkAndMenu.selectedType.ToString() + "\nin " + name + "\nfor &" + cost.ToString();
	}
	
	public void ComponentReady () {
		if(componentCooldown + lastComponentUse < Time.time){
			is_componentReady = true;
			//progressBar.GetComponent<ProgressBar>().measure = componentCooldown;
		}
		else{
			//progressBar.GetComponent<ProgressBar>().measure = Time.time - lastComponentUse;
		}
	}
	
	public void Upgrade() {
		if(transform.parent.GetComponent<ShipControls>().playerMoney >= cost){
			transform.parent.GetComponent<ShipControls>().playerMoney -= cost;
			componentLevel ++;
			cost = Mathf.RoundToInt(cost * 1.5F);
			audioSource.clip = construction;
			audioSource.Play ();
			actionWord = "Upgrade " + thisType + "\nto Lvl" + (componentLevel + 1).ToString() + "\nfor &" + cost.ToString();
		}
		else{
			NotEnoughFunds();
		}
	}
	
	void NotEnoughFunds () {
		audioSource.clip = notEnoughFunds;
		audioSource.Play ();
		Debug.Log ("not enough funds");
	}
	
	public void Construct() {
		if(thisType == ComponentType.Empty){
			if(transform.parent.GetComponent<ShipControls>().playerMoney >= cost){
				Construct(NetworkAndMenu.selectedType);
				componentLevel ++;
				transform.parent.GetComponent<ShipControls>().playerMoney -= cost;
				cost = Mathf.RoundToInt(cost * 1.5F);
				audioSource.clip = construction;
				audioSource.Play ();
				actionWord = "Upgrade " + thisType + "\nto Lvl" + (componentLevel + 1).ToString() + "\nfor &" + cost.ToString();
			}
			else{
				NotEnoughFunds();
			}
		}
		else{
			Upgrade();
		}
	}	
	
	public void SetComponent(string type, float level){
		componentLevel = level;
		switch(type){
		case "Cannon":
			thisType = ComponentType.Cannon;
			break;
		case "Rocket":
			thisType = ComponentType.Rocket;
			break;
		case "Laser":
			thisType = ComponentType.Laser;
			break;
		case "Repair":
			thisType = ComponentType.Repair;
			break;
		case "Ram":
			thisType = ComponentType.Ram;
			break;
		case "Shield":
			thisType = ComponentType.Shield;
			break;
		default:
			thisType = ComponentType.Empty;
			break;
		}
	}
	
	public void Construct(ComponentType type){
		thisType = type;
		if(type == ComponentType.Empty){
			componentLevel = 0;
		}
		switch(type){
		case ComponentType.Cannon:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = cannonMaterial;
			break;
		case ComponentType.Rocket:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = rocketMaterial;
			break;
		case ComponentType.Laser:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = laserMaterial;
			break;
		case ComponentType.Ram:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = ramMaterial;
			break;
		case ComponentType.Shield:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = shieldMaterial;
			break;
		case ComponentType.Repair:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = repairMaterial;
			break;
		default:
			componentArt.renderer.enabled = false;
			break;
		}
	}
	
	public void Action() { 
		switch(transform.parent.GetComponent<ShipControls>().thisPlayerState){
		case PlayerState.Building:
			Construct();
			break;
		case PlayerState.Fighting:
			if(is_componentReady){
				//progressBar.GetComponent<ProgressBar>().measure = 1;
				//progressBar.GetComponent<ProgressBar>().measureCap = 1;		
				is_componentReady = false;
				lastComponentUse = Time.time;
				switch(thisType){
				case ComponentType.Cannon:
					FireCannon();
					break;
					
				case ComponentType.Laser:
					FireLaser();
					break;
					
				case ComponentType.Repair:
					RepairShip();
					break;
					
				case ComponentType.Rocket:
					FireRocket();
					break;
					
				case ComponentType.Shield:
					break;
					
				case ComponentType.Ram:
					break;
					
				default:
					break;
				}
			}
				//progressBar.GetComponent<ProgressBar>().measureCap = componentCooldown;	
			break;
				
		default:
			break;
		}
	}
		
	void FireCannon () {
		componentCooldown = 0.3F;	
		GameObject newProjectile = (GameObject) Instantiate(cannon1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.1F * componentLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (componentLevel - 1)));
		audioSource.clip = cannonFire;
		audioSource.Play ();
	}
	
	void FireRocket () {
		componentCooldown = 2;
		GameObject newProjectile = (GameObject) Instantiate(rocket1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.1F * componentLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (componentLevel - 1)));
		audioSource.clip = rocketFire;
		audioSource.Play ();
	}
	
	void RepairShip () {
		if(transform.parent.GetComponent<ShipControls>().playerHealth < (100 - (5 + 2 * componentLevel))){
			transform.parent.GetComponent<ShipControls>().playerHealth += (5 + 2 * componentLevel);
		}
		else if(transform.parent.GetComponent<ShipControls>().playerHealth < 100){
			transform.parent.GetComponent<ShipControls>().playerHealth = 100;
		}
		componentCooldown = 10;
		audioSource.clip = repairActivate;
		audioSource.Play ();
	}
	
	void FireLaser () {
		componentCooldown = 0.1F;
		GameObject newProjectile = (GameObject) Instantiate(laser1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.25F * componentLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (componentLevel - 1)));
		newProjectile.transform.RotateAround(transform.parent.position, new Vector3(0,0,1), 45);
		newProjectile = (GameObject) Instantiate(laser1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.25F * componentLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (componentLevel - 1)));
		newProjectile = (GameObject) Instantiate(laser1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.25F * componentLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (componentLevel - 1)));
		newProjectile.transform.RotateAround(transform.parent.position, new Vector3(0,0,1), -45);
		audioSource.clip = laserFire;
		audioSource.Play ();
	}
	
	public void Reset () {
		//progressBar.GetComponent<ProgressBar>().measure = 1;
		//progressBar.GetComponent<ProgressBar>().measureCap = 1;
		Construct(ComponentType.Empty);
		cost = 15;
		actionWord = "Construct " + NetworkAndMenu.selectedType.ToString() + "\nin " + name + "\nfor &" + cost.ToString();
	}
}