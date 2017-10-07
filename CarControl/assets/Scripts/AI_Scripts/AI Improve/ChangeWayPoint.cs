using UnityEngine;
using System.Collections;

public class ChangeWayPoint : MonoBehaviour {
	public Transform[] path;
	public int currentPathObj;
	public float distanciaActual;
	public Vector3 currentWayPoint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void CompararDistancias(){
		int currentPathObjLocal;
		Vector3 currentWayPoint= transform.InverseTransformPoint (new Vector3(path[currentPathObj].position.x,transform.position.y,path[currentPathObj].position.z));
		distanciaActual = currentWayPoint.magnitude;
		float distancia;
		currentPathObjLocal= currentPathObj;
		currentPathObjLocal++;

		for(int i = currentPathObjLocal; i < path.Length ; i++){

			currentWayPoint= transform.InverseTransformPoint (new Vector3(path[i].position.x,transform.position.y,path[i].position.z));
			distancia = currentWayPoint.magnitude;

			if(distancia <= distanciaActual){
				Vector3 wayPointZero = transform.InverseTransformPoint (new Vector3(path[0].position.x,transform.position.y,path[0].position.z));

				if(distancia <= wayPointZero.magnitude && currentPathObj > 0){
					currentPathObj =0;
					return;
				}

				currentPathObj=i;

				return;
			}

		}

	}



}
