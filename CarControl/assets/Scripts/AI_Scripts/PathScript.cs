using UnityEngine;
using System.Collections;

public class PathScript : MonoBehaviour {

	public Transform[]path;
	public Color rayColor = Color.white;

	void OnDrawGizmos(){
		Gizmos.color = rayColor;
		Transform[] path_objs = transform.GetComponentsInChildren<Transform>();
		path= new Transform[path_objs.Length-1];
		int posicionArregloPath = 0;

		foreach(Transform path_obj in path_objs){
			if(path_obj != transform){
				path[posicionArregloPath]= path_obj;
				posicionArregloPath++;
			}
		}

		for (int i = 0 ;i < path.Length; i++){

			Vector3 pos = path[i].position;

			path[i].GetComponent<UpdateWayPoint>().WaypointID=i;

			if(i>0){
				Vector3 prev = path[i-1].position;
				Gizmos.DrawLine(prev,pos);
				Gizmos.DrawWireSphere(pos,0.3f);
			}else{
				Gizmos.DrawWireSphere(pos,0.3f);
			}
		}
	}
}
