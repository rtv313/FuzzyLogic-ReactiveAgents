using UnityEngine;
using System.Collections;

public class AIWheelScript : MonoBehaviour {

	public WheelCollider myWheelCollider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate (myWheelCollider.rpm/60*360*Time.deltaTime,0f,0f);
		transform.localEulerAngles= new Vector3 (transform.localEulerAngles.x,myWheelCollider.steerAngle-transform.localEulerAngles.z,transform.localEulerAngles.z);

	/*	RaycastHit hit;
		Vector3 wheelPos;
		if(Physics.Raycast(myWheelCollider.transform.position,-myWheelCollider.transform.up,out hit,myWheelCollider.radius + myWheelCollider.suspensionDistance)){
			wheelPos = hit.point + myWheelCollider.transform.up * myWheelCollider.radius;
		}else{
			wheelPos = myWheelCollider.transform.position - myWheelCollider.transform.up * myWheelCollider.suspensionDistance;
		}
		transform.position = wheelPos;
		*/
	}
}
