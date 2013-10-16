using UnityEngine;
using System.Collections;

public enum ComponentType{
	Empty = 0,
	Factory = 5,
	Repair = 30,
	Rocket = 25,
	Cannon = 15,
	Laser = 10
}

public class ComponentScript : MonoBehaviour {
	
	private Transform hudSlot;
	private Transform hudHighlight;
	private TextMesh componentLevelTextMesh;
	
	public Material cannonMaterial;
	public Material rocketMaterial;
	public Material laserMaterial;
	public Material shieldMaterial;
	public Material ramMaterial;
	public Material repairMaterial;
	private Transform componentArt;
	
	public GameObject cannon1;
	public GameObject rocket1;
	public GameObject laser1;
	private Vector3 fireDirection;
	private ComponentType thisType = ComponentType.Empty;
	private float componentLevel = 0;
	public int cost;
	
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
	
	public void Action(){
		Debug.Log ("Action for " + name);
	}
	/*
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		hudSlot = transform.parent.GetComponent<PlanetaryControls>().statusArea.FindChild("Locations").FindChild(name);
		hudHighlight = hudSlot.FindChild("Highlight");
		componentLevelTextMesh = hudSlot.FindChild("Level").GetComponent<TextMesh>();
		componentArt = transform.FindChild("Art");
		componentArt.renderer.enabled = false;
		progressBar = hudSlot.FindChild("ProgressBar");
		progressBar.GetComponent<ProgressBar>().measure = 1;
		progressBar.GetComponent<ProgressBar>().measureCap = 1;
		lastComponentUse = Time.time - componentCooldown;
		if(name == "Up"){
			Selected();
		}
		cost = 15;
	}
	
	// Update is called once per frame
	void Update () {
		if(!is_componentReady){
			ComponentReady();
		}
		if(is_actionDelayed && (delayTime + delayDuration < Time.time)){
			Action ();
			is_actionDelayed = false;
		}
	}
	
	public void ComponentReady () {
		if(componentCooldown + lastComponentUse < Time.time){
			is_componentReady = true;
			progressBar.GetComponent<ProgressBar>().measure = componentCooldown;
		}
		else{
			progressBar.GetComponent<ProgressBar>().measure = Time.time - lastComponentUse;
		}
	}
	
	public void Selected() {
		transform.FindChild("Bracket").renderer.enabled = true;
		transform.FindChild("Bracket").GetChild(0).renderer.enabled = true;
		transform.FindChild("Bracket").GetChild(1).renderer.enabled = true;
		hudHighlight.renderer.enabled = true;
	}
	
	public void UnSelected() {
		transform.FindChild("Bracket").renderer.enabled = false;
		transform.FindChild("Bracket").GetChild(0).renderer.enabled = false;
		transform.FindChild("Bracket").GetChild(1).renderer.enabled = false;
		hudHighlight.renderer.enabled = false;
	}
	
	public void Scroll(int direction){
		if(thisType == ComponentType.Empty){
			hudSlot.GetComponent<HUDSlot>().ScrollOnSelect(direction);
		}
		cost = hudSlot.GetComponent<HUDSlot>().cost;
	}
	
	public void Upgrade() {
		if(transform.parent.GetComponent<PlanetaryControls>().playerMoney >= Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, componentLevel))){
			Construct(hudSlot.GetComponent<HUDSlot>().selectedType);
			transform.parent.GetComponent<PlanetaryControls>().playerMoney -= Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, componentLevel));
			transform.parent.GetComponent<PlanetaryControls>().moneyText.text = "&" + transform.parent.GetComponent<PlanetaryControls>().playerMoney.ToString();
			componentLevel ++;
			comoponentLevelTextMesh.text = "Lvl" + componentLevel.ToString();
			cost = Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, componentLevel));
			hudSlot.GetComponent<HUDSlot>().title.text = "Upgrade " + name + " (&" + cost.ToString() + ")";
			audioSource.clip = construction;
			audioSource.Play ();
		}
		else{
			NotEnoughFunds();
		}
	}
	
	void NotEnoughFunds () {
		if(!transform.parent.GetComponent<PlanetaryControls>().is_remote){
			audioSource.clip = notEnoughFunds;
			audioSource.Play ();
			Debug.Log ("not enough funds");
		}
	}
	
	public void Construct() {
		if(thisType == ComponentType.Empty){
			if(transform.parent.GetComponent<PlanetaryControls>().playerMoney >= (int) hudSlot.GetComponent<HUDSlot>().selectedType){
				Construct(hudSlot.GetComponent<HUDSlot>().selectedType);
				componentLevel ++;
				cost = Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, componentLevel));
				hudSlot.GetComponent<HUDSlot>().title.text = "Upgrade " + name + " (&" + cost.ToString() + ")";
				componentLevelTextMesh.text = "Lvl" + componentLevel.ToString();
				hudSlot.GetComponent<HUDSlot>().Construct();
				transform.parent.GetComponent<PlanetaryControls>().playerMoney -= (int) hudSlot.GetComponent<HUDSlot>().selectedType;
				transform.parent.GetComponent<PlanetaryControls>().moneyText.text = "&" + transform.parent.GetComponent<PlanetaryControls>().playerMoney.ToString();
				audioSource.clip = construction;
				audioSource.Play ();
			}
			else{
				NotEnoughFunds();
			}
		}
		else{
			Upgrade();
		}
	}	

	
	public void Construct(ComponentType type){
		thisType = type;
		if(type == ComponentType.Empty){
			componentLevel = 0;
			componentLevelTextMesh.text = "";
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
		case ComponentType.Factory:
			componentArt.renderer.enabled = true;
			componentArt.renderer.material = factoryMaterial;
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
	
	public void DelayAction(float thisDelay) {
		if(!is_actionDelayed){
			is_actionDelayed = true;
			delayDuration = thisDelay;
			delayTime = Time.time;
		}
	}
	
	public void Action() { 
		if(is_componentReady){
			progressBar.GetComponent<ProgressBar>().measure = 1;
			progressBar.GetComponent<ProgressBar>().measureCap = 1;		
			is_componentReady = false;
			lastComponentUse = Time.time;
			switch(thisType){
			case ComponentType.Cannon:
				FireCannon();
				break;
				
			case ComponentType.Factory:
				CollectFactory();
				break;
				
			case ComponentType.Laser:
				FireLaser();
				break;
				
			case ComponentType.Repair:
				RepairPlanet();
				break;
				
			case ComponentType.Rocket:
				FireRocket();
				break;
				
			default:
				break;
			}
			progressBar.GetComponent<ProgressBar>().measureCap = componentCooldown;	
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
	
	void CollectFactory () {
		componentCooldown = 5;
		transform.parent.GetComponent<PlanetaryControls>().playerMoney += 5 * (int) componentLevel;
		transform.parent.GetComponent<PlanetaryControls>().moneyText.text = "&" + transform.parent.GetComponent<PlanetaryControls>().playerMoney.ToString();
		audioSource.clip = factoryActivate;
		audioSource.Play ();
	}
	
	void RepairPlanet () {
		if(transform.parent.GetComponent<PlanetaryControls>().planetaryHealth < (100 - (5 + 2 * componentLevel))){
			transform.parent.GetComponent<PlanetaryControls>().planetaryHealth += (5 + 2 * componentLevel);
		}
		else if(transform.parent.GetComponent<PlanetaryControls>().planetaryHealth < 100){
			transform.parent.GetComponent<PlanetaryControls>().planetaryHealth = 100;
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
		progressBar.GetComponent<ProgressBar>().measure = 1;
		progressBar.GetComponent<ProgressBar>().measureCap = 1;
		Construct(ComponentType.Empty);
		hudSlot.GetComponent<HUDSlot>().Reset();
	}
*/
}