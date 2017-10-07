using UnityEngine;
using System.Collections;

public class CarControlPosition : MonoBehaviour {

	public int Lap;
	public int WayPointID;
	public float WayPointDistance;
	public Transform pathGroup;
	public Transform[]path;
	public int racePosition;
	private float velocidadStandard;
	// Use this for initialization
	void Start () {
		GetPath ();
		InitValues ();
	}
	
	// Update is called once per frame
	void Update () {
		ActualizarValores ();
		SetVelocidades ();
	}

	void InitValues(){
		Lap = 0;
		WayPointID = 0;
		Vector3 TargeWayPoint = path [0].position;
		WayPointDistance = TargeWayPoint.magnitude;
		velocidadStandard = transform.GetComponent<AICarScript> ().topSpeed;
	}

	void ActualizarValores(){
		WayPointID = transform.GetComponent<AICarScript> ().currentPathObj;
		WayPointDistance = Vector3.Distance(transform.position, path [WayPointID].position); 
	}

	void GetPath(){
		
		Transform[] path_objs = pathGroup.GetComponentsInChildren<Transform>();
		path= new Transform[path_objs.Length-1];
		int posicionArregloPath = 0;
		
		foreach(Transform path_obj in path_objs){
			if(path_obj != pathGroup){
				path[posicionArregloPath]= path_obj;
				posicionArregloPath++;
			}
		}
	}

	public void ActualizarLap(){
		Lap++;
	}

	public void  SetVelocidades(){
		if (racePosition == 1) {
			transform.GetComponent<AICarScript> ().topSpeed=velocidadStandard-25f;
			return;
		}
		if (racePosition == 2) {
			transform.GetComponent<AICarScript> ().topSpeed=velocidadStandard-10f;
			return;
		}

		if (racePosition == 3) {
			transform.GetComponent<AICarScript> ().topSpeed=velocidadStandard-5f;
			return;
		}

		transform.GetComponent<AICarScript> ().topSpeed=velocidadStandard;
		return;
	}
}
