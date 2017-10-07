using UnityEngine;
using System.Collections;

public class Fuzzification  {

	public float maxValue;
	public float minValue;
	public float valorActual;

	public float espacioRangos; //pri
	public float[] rangos; //pri
	public bool izquierda;
	public bool centro;
	public bool derecha;
	public float izquierdaIntensidad;
	public float centroIntensidad;
	public float derechaIntensidad;
	


	public Fuzzification(float maxValue,float minValue,float valorActual){

		this.maxValue= maxValue;
		this.minValue=minValue;
		this.valorActual= valorActual;



		izquierda = false;
		centro = false;
		derecha = false;
		rangos = new float[3];
		rangos [0] = minValue;
		rangos [1] =(maxValue + minValue) / 2;
		rangos [2] = maxValue;
	}

	private float FuncionRecta(float x,float x1,float y1,float x2,float y2){

		return ((y2 - y1) / (x2 - x1))*(x - x1) + y1;
	}

	void fuzzIzquierda(float valor){
		izquierdaIntensidad = 0.0f;
		if(valorActual <= rangos[0]){
			izquierdaIntensidad=1.0f;
			izquierda=true;
			return;
		}
		
		if(valorActual >= rangos[0] && valorActual <= rangos[1]){
			izquierdaIntensidad = FuncionRecta(valor,rangos[0],1.0f,rangos[1],0.0f);
			izquierda=true;
			return;
		}
	}


	void fuzzCentro(float valor){

		centroIntensidad = 0.0f;
		if(valorActual >= rangos[0] && valorActual <= rangos[1]){
			centroIntensidad = FuncionRecta(valor,rangos[0],0.0f,rangos[1],1.0f);
			centro=true;
			return;
		}

		if(valorActual >= rangos[1] && valorActual <= rangos[2]){
			centroIntensidad = FuncionRecta(valor,rangos[1],1.0f,rangos[2],0.0f);
			centro=true;
			return;
		}
	}

	void fuzzDerecha(float valor){
		derechaIntensidad = 0.0f;
		if(valorActual >= rangos[2]){
			derechaIntensidad=1.0f;
			derecha=true;
			return;
		}
		
		if(valorActual >= rangos[1] && valorActual <= rangos[2]){
			derechaIntensidad = FuncionRecta(valor,rangos[1],0.0f,rangos[2],1.0f);
			derecha=true;
			return;
		}
	}

	public void fuzzificar(){
		fuzzIzquierda (valorActual);
		fuzzCentro (valorActual);
		fuzzDerecha (valorActual);
	}


	public void updateFuzzification(float maxValue,float minValue,float nuevoValor){
		this.maxValue= maxValue;
		this.minValue=minValue;
		this.valorActual= nuevoValor;


		izquierda = false;
		centro = false;
		derecha = false;
		rangos [0] = minValue;
		rangos [1] =(maxValue + minValue) / 2;
		rangos [2] = maxValue;
	}
}
