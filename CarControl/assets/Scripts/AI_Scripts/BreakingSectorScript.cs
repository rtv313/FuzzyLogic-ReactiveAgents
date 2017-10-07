using UnityEngine;
using System.Collections;

public class BreakingSectorScript : MonoBehaviour {

	public float maxBreakTorque;
	public float minCarSpeed;



	void OnTriggerStay(Collider other){

		if(other.tag == "AI"){
			float controlCurrentSpeed = other.transform.root.GetComponent<AICarScript>().currentSpeed;

			if(controlCurrentSpeed >= minCarSpeed){
				other.transform.root.GetComponent<AICarScript>().inSector=true;
				other.transform.root.GetComponent<AICarScript>().wheelRR.brakeTorque=maxBreakTorque;
			    other.transform.root.GetComponent<AICarScript>().wheelRL.brakeTorque=maxBreakTorque;
				other.transform.root.GetComponent<AICarScript>().isBreaking=true;
			}else{
				other.transform.root.GetComponent<AICarScript>().inSector=false;
				other.transform.root.GetComponent<AICarScript>().wheelRR.brakeTorque=0f;
				other.transform.root.GetComponent<AICarScript>().wheelRL.brakeTorque=0f;

			}

		}
	}

	void OnTriggerExit(Collider other){

		if(other.tag == "AI"){
			other.transform.root.GetComponent<AICarScript>().inSector=false;
			other.transform.root.GetComponent<AICarScript>().wheelRR.brakeTorque=0f;
			other.transform.root.GetComponent<AICarScript>().wheelRL.brakeTorque=0f;
			other.transform.root.GetComponent<AICarScript>().isBreaking=false;
		}
	}


}
