using UnityEngine;
using System.Collections;

public class SoundEngine : MonoBehaviour {

	public  int [] gearRatio ; // Gears for sound 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		EngineSound ();
	}

	void EngineSound(){

		audio.pitch = gameObject.GetComponent<AICarScript>().currentSpeed /gameObject.GetComponent<AICarScript>().topSpeed + 1;
		int i;
		for(i=0; i < gearRatio.Length;i++){
			
			
			if(gearRatio[i] > gameObject.GetComponent<AICarScript>().currentSpeed){
				
				break;
			}
		}
		
		float gearMinValue = 0.0f;
		float gearMaxValue = 0.0f;
		
		if(i >=4 ){
			i =4;
		}
		
		if(i==0){
			gearMinValue = 0f;
			gearMaxValue = gearRatio[i];
		}else{
			gearMinValue = gearRatio[i-1];
		}
		
		gearMaxValue = gearRatio[i];
		float enginePitch = ((gameObject.GetComponent<AICarScript>().currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue))+1f;
		audio.pitch = enginePitch;
	}
}
