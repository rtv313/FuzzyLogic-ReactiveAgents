using UnityEngine;
using System.Collections;

public class UpdateWayPoint : MonoBehaviour {
	public int WaypointID;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.tag == "AI"){

			AICarScript AIcar =	other.transform.root.GetComponent<AICarScript>();
			/*if(WaypointID ==0 && AIcar.currentPathObj==0){
				other.GetComponent<CarControlPosition>().ActualizarLap();
			}*/
			AIcar.currentPathObj=WaypointID+1;

			if(AIcar.currentPathObj >= AIcar.path.Length){
				other.GetComponent<CarControlPosition>().ActualizarLap();
				AIcar.currentPathObj = 0;
			}

		}
	}


}
