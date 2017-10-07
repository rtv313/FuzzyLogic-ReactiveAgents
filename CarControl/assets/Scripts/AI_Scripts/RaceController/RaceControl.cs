using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class RaceControl : MonoBehaviour {

	public List<Transform> cars = new List<Transform>();
	public List<CarControlPosition> posiciones = new List<CarControlPosition>();

	// Use this for initialization
	void Start () {
		InicializarListas ();
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(OrderByLap ());
	}

	void InicializarListas(){

		for(int i=0;i<cars.Count;i++){
			posiciones[i]= cars[i].GetComponent<CarControlPosition>();
			posiciones[i].racePosition=0;
		}
	}

	IEnumerator OrderByLap(){
	
		posiciones=posiciones.OrderByDescending (L => L.Lap).ThenByDescending (W => W.WayPointID).ThenBy (D => D.WayPointDistance).ToList();
		insertarPosiciones ();
		yield return new WaitForSeconds (0.3f);
	}

	void insertarPosiciones(){
		for(int i=0;i<cars.Count;i++){
			posiciones[i].racePosition= i+1;
		}
	}

}
