using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	
	public Transform start;
	public Transform middle;
	public Transform end;
	public float measure=1;
	public float measureCap=1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(measure > measureCap){
			measure = measureCap;
		}
		if(measure < 0){
			measure = 0;
		}
		middle.localPosition = new Vector3( - ((measure - measureCap) / measureCap) / 2 * 8 - 0.1F, 0.5F, 0);
		end.localPosition = new Vector3( - ((measure - measureCap) / measureCap) * 8 - 4 - end.lossyScale.x * 5  - 0.35F, 0.5F, 0);
		middle.localScale = new Vector3(measure / measureCap * 0.8F, 1, 1);
	}
}
