using UnityEngine;
using System.Collections;

public class HUDSlot : MonoBehaviour {
	
	public Transform cannonBuilding;
	public Transform rocketBuilding;
	public Transform laserBuilding;
	public Transform factoryBuilding;
	public Transform repairBuilding;
	public TextMesh title;
	private int selectedTitle = 0;
	private Transform[] buildingList;
	private Transform[] buildingPrefabList;
	private BuildingType[] buildingTypeList;
	private string[] titles;
	private bool is_built;
	private bool is_selected;
	public BuildingType selectedType;
	public int cost;
	public Material unbuilt;
	public Material built;
	
	// Use this for initialization
	void Start () {
		titles = new string[] {"Cannon", "Rocket", "Laser", "Factory", "Repair"};
		title = transform.FindChild("Title").GetComponent<TextMesh>();
		buildingPrefabList = new Transform[] {cannonBuilding, rocketBuilding, laserBuilding, factoryBuilding, repairBuilding};
		buildingList = new Transform[5];
		buildingTypeList = new BuildingType[] {BuildingType.Cannon, BuildingType.Rocket, BuildingType.Laser, BuildingType.Factory, BuildingType.Repair};
		for(int i = 0; i < buildingList.Length; i++){
			Transform buildingListObject = (Transform) Transform.Instantiate(buildingPrefabList[i], new Vector3(transform.position.x + 1 * i - 2F, transform.position.y - 0.05F * i, transform.position.z), transform.rotation);
			buildingListObject.parent = transform;
			buildingListObject.localScale = new Vector3(1 / Mathf.Sqrt(i + 1)/ transform.lossyScale.x, 1 / Mathf.Sqrt(i + 1)/transform.lossyScale.y, 1 / Mathf.Sqrt(i + 1)/transform.lossyScale.z);
			buildingList[i] = buildingListObject;
		}
		selectedType = buildingTypeList[0];
		title.text = "Construct " + titles [selectedTitle] + " (&" + ((int)selectedType).ToString() + ")";
		for( int i = 0; i < transform.FindChild("ProgressBar").childCount; i++){
			transform.FindChild("ProgressBar").GetChild(i).renderer.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Construct () {
		for(int i = 1; i < buildingList.Length; i ++){
			Destroy(buildingList[i].gameObject);
		}
		for( int i = 0; i < transform.FindChild("ProgressBar").childCount; i++){
			transform.FindChild("ProgressBar").GetChild(i).renderer.enabled = true;
		}
		renderer.material = built;
	}
	
	public void ScrollOnSelect(int direction){
		selectedTitle += direction;
		if(selectedTitle > 4){
			selectedTitle = 0;
		}
		else if(selectedTitle < 0){
			selectedTitle = 4;
		}
		if(direction > 0){
			buildingList = new Transform[] {buildingList[1], buildingList[2], buildingList[3], buildingList[4], buildingList[0]};
			buildingTypeList = new BuildingType[] {buildingTypeList[1], buildingTypeList[2], buildingTypeList[3], buildingTypeList[4], buildingTypeList[0]};
		}
		else{
			buildingList = new Transform[] {buildingList[4], buildingList[0], buildingList[1], buildingList[2], buildingList[3]};
			buildingTypeList = new BuildingType[] {buildingTypeList[4], buildingTypeList[0], buildingTypeList[1], buildingTypeList[2], buildingTypeList[3]};
		}
		for(int i = 0; i < buildingList.Length; i++){
			buildingList[i].position = new Vector3(transform.position.x + 1 * i - 2F, transform.position.y - 0.05F * i, transform.position.z);
			buildingList[i].localScale = new Vector3(1 / Mathf.Sqrt(i + 1)/ transform.lossyScale.x, 1 / Mathf.Sqrt(i + 1)/transform.lossyScale.y, 1 / Mathf.Sqrt(i + 1)/transform.lossyScale.z);
		}
		selectedType = buildingTypeList[0];
		title.text = "Construct " + titles [selectedTitle] + " (&" + (((int)selectedType).ToString()) + ")";
	}
	
	public void Reset(){
		for(int i = 0; i < buildingList.Length; i ++){
			if(buildingList[i] != null){
				Destroy(buildingList[i].gameObject);
			}
		}
		titles = new string[] {"Cannon", "Rocket", "Laser", "Factory", "Repair"};
		selectedTitle = 0;
		title = transform.FindChild("Title").GetComponent<TextMesh>();
		buildingList = new Transform[5];
		buildingTypeList = new BuildingType[] {BuildingType.Cannon, BuildingType.Rocket, BuildingType.Laser, BuildingType.Factory, BuildingType.Repair};
		for(int i = 0; i < buildingPrefabList.Length; i++){
			Transform buildingListObject = (Transform) Transform.Instantiate(buildingPrefabList[i], new Vector3(transform.position.x + 1 * i - 2F, transform.position.y - 0.05F * i, transform.position.z), transform.rotation);
			buildingListObject.parent = transform;
			buildingListObject.localScale = new Vector3(1 / Mathf.Sqrt(i + 1)/ transform.lossyScale.x, 1 / Mathf.Sqrt(i + 1)/transform.lossyScale.y, 1 / Mathf.Sqrt(i + 1)/transform.lossyScale.z);
			buildingList[i] = buildingListObject;
		}
		selectedType = buildingTypeList[0];
		title.text = "Construct " + titles [selectedTitle] + " (&" + (((int)selectedType).ToString()) + ")";
		for( int i = 0; i < transform.FindChild("ProgressBar").childCount; i++){
			transform.FindChild("ProgressBar").GetChild(i).renderer.enabled = false;
		}
		renderer.material = unbuilt;
	}
}
