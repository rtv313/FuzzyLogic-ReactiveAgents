using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarCameraScript : MonoBehaviour {

	public Transform car;
	public float distance = 6.4f;
	public float height = 1.4f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	public float zoomRatio = 0.5f;
	public float defaultFOV = 60f;
	private Vector3 rotationVector;
	public Transform[] cars;
	public Text MaxSpeed;
	public Text CurrentSpeed;
	public Text CurrentRacePosition;
	public Text Lap;

	
	// Update is called once per frame

	void Update(){
		StartCoroutine (ChangeCar ());
		updateUI ();
	}

	void LateUpdate () { // Camera Control 
	
		float wantedAngle = rotationVector.y;
		float wantedHeight = car.position.y + height;
		float myAngle = transform.eulerAngles.y;
		float myHeight = transform.position.y;

		myAngle = Mathf.LerpAngle (myAngle, wantedAngle, rotationDamping * Time.deltaTime);
		myHeight = Mathf.Lerp (myHeight, wantedHeight, heightDamping * Time.deltaTime);
		Quaternion currentRotation =  Quaternion.Euler (0f,myAngle,0f);
		transform.position = car.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		transform.position = new Vector3 (transform.position.x, myHeight, transform.position.z);
		transform.LookAt (car);
	}


	void updateUI(){
		MaxSpeed.text = "Maxima Velocida: " + car.GetComponent<AICarScript> ().topSpeed;
		CurrentSpeed.text = "Velocidad Actual: " + car.GetComponent<AICarScript> ().currentSpeed;
		CurrentRacePosition.text = "Posicion: " + car.GetComponent<CarControlPosition> ().racePosition;
		Lap.text = "Vuelta: " + car.GetComponent<CarControlPosition> ().Lap;
	}

	void FixedUpdate(){ // set back the camera when the car gets speed given the effect of aceleration

		Vector3 localVelocity = car.InverseTransformDirection (car.rigidbody.velocity);
		if (localVelocity.z < -0.5) {  //Calculates if the car its going to reverse and flip the camera
			//rotationVector.Set(rotationVector.x,car.eulerAngles.y + 180f,rotationVector.z);
		} else {
			rotationVector.Set(rotationVector.x,car.eulerAngles.y,rotationVector.z);
		}


		float acc = car.rigidbody.velocity.magnitude;
		camera.fieldOfView = defaultFOV + acc * zoomRatio;
	}

	IEnumerator ChangeCar(){

		if (Input.GetKey(KeyCode.A)){

			car= cars[0];
		}

		if (Input.GetKey(KeyCode.S)){
			
			car= cars[1];
		}

		if (Input.GetKey(KeyCode.D)){

			car= cars[2];
		}



		if (Input.GetKey(KeyCode.R)){
			
			Application.LoadLevel(0);
		}

		if (Input.GetKey(KeyCode.Escape)){
			
			Application.Quit();
		}

		yield return new WaitForSeconds (0.1f);
			
	}
}
