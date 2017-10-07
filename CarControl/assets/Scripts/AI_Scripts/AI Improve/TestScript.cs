using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	private Fuzzification velocidad;
	public float minVelocidad;
	public float maxVelocidad;
	public float velocidadCoche;
	// Use this for initialization
	void Start () {

		velocidad = new Fuzzification (maxVelocidad, minVelocidad, velocidadCoche);
	}
	
	// Update is called once per frame
	void Update () {
		velocidad.valorActual = velocidadCoche;
		velocidad.fuzzificar ();
		Debug.Log ("Izquierda Bool:"+velocidad.izquierda+" Intensidad:"+velocidad.izquierdaIntensidad);

		Debug.Log ("Centro Bool:"+velocidad.centro+" Intensidad:"+velocidad.centroIntensidad);
	
		Debug.Log ("Derecho Bool:"+velocidad.derecha+" Intensidad:"+velocidad.derechaIntensidad);

		velocidad.updateFuzzification (maxVelocidad,minVelocidad,velocidadCoche); 
	}
}
