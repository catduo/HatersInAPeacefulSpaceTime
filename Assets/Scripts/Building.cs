using UnityEngine;
using System.Collections;

public enum BuildingType{
	Empty = 0,
	Factory = 5,
	Repair = 30,
	Rocket = 25,
	Cannon = 15,
	Laser = 10
}

public struct BuildingStatus{
	public float cooldownTime, lastFiredTime;
	public int level, upgradeCost, damage;
	public bool ready;
}

public class Building : MonoBehaviour {
	
	private Transform hudSlot;
	private Transform hudHighlight;
	private TextMesh buildingLevelTextMesh;
	
	public Material cannonMaterial;
	public Material rocketMaterial;
	public Material laserMaterial;
	public Material factoryMaterial;
	public Material repairMaterial;
	private Transform buildingArt;
	
	public GameObject cannon1;
	public GameObject rocket1;
	public GameObject laser1;
	private Vector3 fireDirection;
	private BuildingType thisType = BuildingType.Empty;
	private float buildingLevel = 0;
	public int cost ;
	
	private float buildingCooldown = 0.1F;
	public float lastBuildingUse;
	public bool is_buildingReady = true;
	private Transform progressBar;
	
	private bool is_actionDelayed = false;
	private float delayTime = 0;
	private float delayDuration = 0;
	
	public AudioClip cannonFire;
	public AudioClip laserFire;
	public AudioClip rocketFire;
	public AudioClip factoryActivate;
	public AudioClip repairActivate;
	public AudioClip construction;
	public AudioClip notEnoughFunds;
	private AudioSource audioSource;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		hudSlot = transform.parent.GetComponent<PlanetaryControls>().statusArea.FindChild("Locations").FindChild(name);
		hudHighlight = hudSlot.FindChild("Highlight");
		buildingLevelTextMesh = hudSlot.FindChild("Level").GetComponent<TextMesh>();
		buildingArt = transform.FindChild("Art");
		buildingArt.renderer.enabled = false;
		progressBar = hudSlot.FindChild("ProgressBar");
		progressBar.GetComponent<ProgressBar>().measure = 1;
		progressBar.GetComponent<ProgressBar>().measureCap = 1;
		lastBuildingUse = Time.time - buildingCooldown;
		if(name == "Up"){
			Selected();
		}
		cost = 15;
	}
	
	// Update is called once per frame
	void Update () {
		if(!is_buildingReady){
			BuildingReady();
		}
		if(is_actionDelayed && (delayTime + delayDuration < Time.time)){
			Action ();
			is_actionDelayed = false;
		}
	}
	
	public void BuildingReady () {
		if(buildingCooldown + lastBuildingUse < Time.time){
			is_buildingReady = true;
			progressBar.GetComponent<ProgressBar>().measure = buildingCooldown;
		}
		else{
			progressBar.GetComponent<ProgressBar>().measure = Time.time - lastBuildingUse;
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
		if(thisType == BuildingType.Empty){
			hudSlot.GetComponent<HUDSlot>().ScrollOnSelect(direction);
		}
		cost = hudSlot.GetComponent<HUDSlot>().cost;
	}
	
	public void Upgrade() {
		if(transform.parent.GetComponent<PlanetaryControls>().playerMoney >= Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, buildingLevel))){
			Construct(hudSlot.GetComponent<HUDSlot>().selectedType);
			transform.parent.GetComponent<PlanetaryControls>().playerMoney -= Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, buildingLevel));
			transform.parent.GetComponent<PlanetaryControls>().moneyText.text = "&" + transform.parent.GetComponent<PlanetaryControls>().playerMoney.ToString();
			buildingLevel ++;
			buildingLevelTextMesh.text = "Lvl" + buildingLevel.ToString();
			cost = Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, buildingLevel));
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
		if(thisType == BuildingType.Empty){
			if(transform.parent.GetComponent<PlanetaryControls>().playerMoney >= (int) hudSlot.GetComponent<HUDSlot>().selectedType){
				Construct(hudSlot.GetComponent<HUDSlot>().selectedType);
				buildingLevel ++;
				cost = Mathf.RoundToInt((float) hudSlot.GetComponent<HUDSlot>().selectedType * Mathf.Pow(1.5F, buildingLevel));
				hudSlot.GetComponent<HUDSlot>().title.text = "Upgrade " + name + " (&" + cost.ToString() + ")";
				buildingLevelTextMesh.text = "Lvl" + buildingLevel.ToString();
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

	
	public void Construct(BuildingType type){
		thisType = type;
		if(type == BuildingType.Empty){
			buildingLevel = 0;
			buildingLevelTextMesh.text = "";
		}
		switch(type){
		case BuildingType.Cannon:
			buildingArt.renderer.enabled = true;
			buildingArt.renderer.material = cannonMaterial;
			break;
		case BuildingType.Rocket:
			buildingArt.renderer.enabled = true;
			buildingArt.renderer.material = rocketMaterial;
			break;
		case BuildingType.Laser:
			buildingArt.renderer.enabled = true;
			buildingArt.renderer.material = laserMaterial;
			break;
		case BuildingType.Factory:
			buildingArt.renderer.enabled = true;
			buildingArt.renderer.material = factoryMaterial;
			break;
		case BuildingType.Repair:
			buildingArt.renderer.enabled = true;
			buildingArt.renderer.material = repairMaterial;
			break;
		default:
			buildingArt.renderer.enabled = false;
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
		if(is_buildingReady){
			progressBar.GetComponent<ProgressBar>().measure = 1;
			progressBar.GetComponent<ProgressBar>().measureCap = 1;		
			is_buildingReady = false;
			lastBuildingUse = Time.time;
			switch(thisType){
			case BuildingType.Cannon:
				FireCannon();
				break;
				
			case BuildingType.Factory:
				CollectFactory();
				break;
				
			case BuildingType.Laser:
				FireLaser();
				break;
				
			case BuildingType.Repair:
				RepairPlanet();
				break;
				
			case BuildingType.Rocket:
				FireRocket();
				break;
				
			default:
				break;
			}
			progressBar.GetComponent<ProgressBar>().measureCap = buildingCooldown;	
		}
	}
		
	void FireCannon () {
		buildingCooldown = 0.3F;	
		GameObject newProjectile = (GameObject) Instantiate(cannon1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.1F * buildingLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (buildingLevel - 1)));
		audioSource.clip = cannonFire;
		audioSource.Play ();
	}
	
	void FireRocket () {
		buildingCooldown = 2;
		GameObject newProjectile = (GameObject) Instantiate(rocket1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.1F * buildingLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (buildingLevel - 1)));
		audioSource.clip = rocketFire;
		audioSource.Play ();
	}
	
	void CollectFactory () {
		buildingCooldown = 5;
		transform.parent.GetComponent<PlanetaryControls>().playerMoney += 5 * (int) buildingLevel;
		transform.parent.GetComponent<PlanetaryControls>().moneyText.text = "&" + transform.parent.GetComponent<PlanetaryControls>().playerMoney.ToString();
		audioSource.clip = factoryActivate;
		audioSource.Play ();
	}
	
	void RepairPlanet () {
		if(transform.parent.GetComponent<PlanetaryControls>().planetaryHealth < (100 - (5 + 2 * buildingLevel))){
			transform.parent.GetComponent<PlanetaryControls>().planetaryHealth += (5 + 2 * buildingLevel);
		}
		else if(transform.parent.GetComponent<PlanetaryControls>().planetaryHealth < 100){
			transform.parent.GetComponent<PlanetaryControls>().planetaryHealth = 100;
		}
		buildingCooldown = 10;
		audioSource.clip = repairActivate;
		audioSource.Play ();
	}
	
	void FireLaser () {
		buildingCooldown = 0.1F;
		GameObject newProjectile = (GameObject) Instantiate(laser1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.25F * buildingLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (buildingLevel - 1)));
		newProjectile.transform.RotateAround(transform.parent.position, new Vector3(0,0,1), 45);
		newProjectile = (GameObject) Instantiate(laser1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.25F * buildingLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (buildingLevel - 1)));
		newProjectile = (GameObject) Instantiate(laser1, transform.position + (transform.position - transform.parent.position) * 1.3F * (1 + (0.25F * buildingLevel)), Quaternion.LookRotation(transform.up));
		newProjectile.transform.localScale *= (1 + (0.5F * (buildingLevel - 1)));
		newProjectile.transform.RotateAround(transform.parent.position, new Vector3(0,0,1), -45);
		audioSource.clip = laserFire;
		audioSource.Play ();
	}
	
	public void Reset () {
		progressBar.GetComponent<ProgressBar>().measure = 1;
		progressBar.GetComponent<ProgressBar>().measureCap = 1;
		Construct(BuildingType.Empty);
		hudSlot.GetComponent<HUDSlot>().Reset();
	}
}
