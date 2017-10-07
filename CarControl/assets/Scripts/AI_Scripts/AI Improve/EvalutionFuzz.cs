using UnityEngine;
using System.Collections;

public class EvalutionFuzz : MonoBehaviour {

	public Fuzzification fuzzificadorVelocidad;
	public Fuzzification fuzzificadorAngulo;
	public float minVelocidad;
	public float maxVelocidad;
	public float maxAngulo;
	public float velocidad;
	public float angulo;
	public float[] r= new float[9];
	public float acelerar;
	public float noCambia;
	public float desacelerar;
	public float centro;
	public string resultadoEscrito;
	public float intensidadDesfuzzificacion;
	// Use this for initialization
	void Start () {
		Inicializar ();
	}
	
	// Update is called once per frame
	void Update () {
		Fuzzificar ();
		EvaluacionReglas ();
		ResetearFuzzificadores ();
		Inferencia ();
		Dezzfuzificacion ();
		ResetearFuzzificadores();
	}

	void Inicializar(){
		intensidadDesfuzzificacion = 0.0f;
		for(int i =0; i <r.Length;i++){
			r [i] = 0.0f;
		}
		centro = (minVelocidad + maxVelocidad) / 2;
		fuzzificadorVelocidad = new Fuzzification (maxVelocidad,minVelocidad,velocidad);
		fuzzificadorAngulo = new Fuzzification (maxAngulo,0.0f,angulo);
	}

	void Fuzzificar(){
		fuzzificadorAngulo.fuzzificar ();
		fuzzificadorVelocidad.fuzzificar ();
	}

	void EvaluacionReglas(){

		if(fuzzificadorAngulo.izquierda && fuzzificadorVelocidad.derecha){r[0]=Mathf.Min(fuzzificadorAngulo.izquierdaIntensidad,fuzzificadorVelocidad.derechaIntensidad);}
		if(fuzzificadorAngulo.izquierda && fuzzificadorVelocidad.centro){r[1]=Mathf.Min(fuzzificadorAngulo.izquierdaIntensidad,fuzzificadorVelocidad.centroIntensidad);}
		if(fuzzificadorAngulo.izquierda && fuzzificadorVelocidad.izquierda){r[2]=Mathf.Min(fuzzificadorAngulo.izquierdaIntensidad,fuzzificadorVelocidad.izquierdaIntensidad);}

		if(fuzzificadorAngulo.centro && fuzzificadorVelocidad.derecha){r[3]=Mathf.Min(fuzzificadorAngulo.centroIntensidad,fuzzificadorVelocidad.derechaIntensidad);}
		if(fuzzificadorAngulo.centro && fuzzificadorVelocidad.centro){r[4]=Mathf.Min(fuzzificadorAngulo.centroIntensidad,fuzzificadorVelocidad.centroIntensidad);}
		if(fuzzificadorAngulo.centro && fuzzificadorVelocidad.izquierda){r[5]=Mathf.Min(fuzzificadorAngulo.centroIntensidad,fuzzificadorVelocidad.izquierdaIntensidad);}

		if(fuzzificadorAngulo.derecha && fuzzificadorVelocidad.derecha){r[6]=Mathf.Min(fuzzificadorAngulo.derechaIntensidad,fuzzificadorVelocidad.derechaIntensidad);}
		if(fuzzificadorAngulo.derecha && fuzzificadorVelocidad.centro){r[7]=Mathf.Min(fuzzificadorAngulo.derechaIntensidad,fuzzificadorVelocidad.centroIntensidad);}
		if(fuzzificadorAngulo.derecha && fuzzificadorVelocidad.izquierda){r[8]=Mathf.Min(fuzzificadorAngulo.derechaIntensidad,fuzzificadorVelocidad.izquierdaIntensidad);}
	}

	void  Inferencia(){

		acelerar = Mathf.Pow (r [0], 2.0f) + Mathf.Pow (r [1], 2.0f) + Mathf.Pow (r [2], 2.0f)+ Mathf.Pow (r [5], 2.0f) + Mathf.Pow (r [8], 2.0f);
		noCambia = Mathf.Pow (r [4], 2.0f);
		desacelerar =Mathf.Pow(r [3], 2.0f) +  Mathf.Pow(r [6], 2.0f) + Mathf.Pow (r [7], 2.0f) ;

	}

	void Dezzfuzificacion(){
	
		 intensidadDesfuzzificacion = (minVelocidad * desacelerar + centro * noCambia + maxVelocidad * acelerar) / (acelerar + desacelerar + noCambia);

		if(intensidadDesfuzzificacion == centro){
			Debug.Log("Entre");
			resultadoEscrito="NoCambia";

			return;
		}

		if(intensidadDesfuzzificacion < centro){
			resultadoEscrito="Desacelerar";

			return;
		}

		if(intensidadDesfuzzificacion > centro){
			resultadoEscrito="Acelerar";
		
			return;
		}
	}

	void ResetearFuzzificadores(){
		fuzzificadorAngulo.updateFuzzification(maxAngulo,0.0f,angulo);
		fuzzificadorVelocidad.updateFuzzification(maxVelocidad,minVelocidad,velocidad);
		centro = (minVelocidad + maxVelocidad) / 2;
	}

	public string GetResultdoEscrito(){
		return resultadoEscrito;
	}

	public float GetIntensidadDesfuzzificacion(){
		return intensidadDesfuzzificacion;
	}

	public void setMaxVelocidad(float nuevaVelocidadMax){
		maxVelocidad = nuevaVelocidadMax;
	}

	public void setMinVelocidad(float nuevaVelocidadMin){
		minVelocidad = nuevaVelocidadMin;
	}

	public void setMaxVelocidadActual(float nuevaVelocidadActual){
		velocidad = nuevaVelocidadActual;
	}

	public void setMaxAngulo(float nuevoAngulo){
		maxAngulo = nuevoAngulo;
	}

	public void setMaxAnguloActual(float nuevoAnguloActual){
		angulo = nuevoAnguloActual;
	}
}
