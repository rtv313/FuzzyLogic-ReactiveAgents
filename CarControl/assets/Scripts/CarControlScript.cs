using UnityEngine;
using System.Collections;

public class CarControlScript : MonoBehaviour {

	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;

	public Transform wheelFLTrans;
	public Transform wheelFRTrans;
	public Transform wheelRLTrans;
	public Transform wheelRRTrans;

	public float maxTorque = 50f;
	public float lowestSteerAtSpeed = 50f;
	public float lowSpeedSteerAngle = 10f;
	public float highSpeedSteerAngle = 1f;
	public float desalerationSpeed	= 30f;
	public float currentSpeed;
	public float topSpeed = 150;
	public float maxReverseSpeed = 50f;

	public GameObject backLightObject; // the object that we will asign a new texture to change the lights
	public Material idleLightMaterial;
	public Material breakLightMaterial;
	public Material reverseLightMaterial;

	public bool breaked = false;
	public float maxBreakTorque = 100;

	private float mySidewayFriction;
	private float myForwardFriction;
	private float slipSidewayFriction;
	private float slipForwardFriction;
	
	public  int [] gearRatio ; // Gears for sound 

	public Texture2D speedOMeterDial; // Velocities
	public Texture2D speedOmeterPointer; // Needle for Velocities

	public GameObject spark;//Spark for collisions
	public GameObject collisionSound;//Sound for sparkCollisions



	// Use this for initialization
	void Start(){
		setCenterOfMass();
		SetValues();
	}

	void setCenterOfMass(){
		rigidbody.centerOfMass = new Vector3 (rigidbody.centerOfMass.x, -0.9f, 0.3f);
	}

	void SetValues(){ // set values to the friction 
		myForwardFriction = wheelRR.forwardFriction.stiffness;
		mySidewayFriction = wheelRR.sidewaysFriction.stiffness;
		slipForwardFriction = 0.05f;
		slipSidewayFriction = 0.085f;

	}

	// Update is called once per frame
	void FixedUpdate(){
		Control ();
		HandBreak ();
	}

	void Update(){ 

		WheelsTransRotations();
		BackLight();
		WheelPosition();
		ReverseSlip ();
		EngineSound ();
	
	}

	void WheelsTransRotations(){
		wheelFLTrans.Rotate(new Vector3(wheelFL.rpm/60*360*Time.deltaTime,0f,0f)); //This rotate the tires
		wheelFRTrans.Rotate(new Vector3(wheelFR.rpm/60*360*Time.deltaTime,0f,0f));
		wheelRLTrans.Rotate(new Vector3(wheelRL.rpm/60*360*Time.deltaTime,0f,0f));
		wheelRRTrans.Rotate(new Vector3(wheelRR.rpm/60*360*Time.deltaTime,0f,0f));
		
		//Rotate tires around Y Axis
		wheelFLTrans.localEulerAngles = new Vector3(wheelFLTrans.localEulerAngles.x,wheelFL.steerAngle - wheelFLTrans.localEulerAngles.z ,wheelFLTrans.localEulerAngles.z);
		wheelFRTrans.localEulerAngles = new Vector3(wheelFRTrans.localEulerAngles.x,wheelFR.steerAngle - wheelFRTrans.localEulerAngles.z ,wheelFRTrans.localEulerAngles.z);
	}

	void Control(){

		currentSpeed = 2 * Mathf.PI * wheelRL.radius * wheelRL.rpm * 60 / 1000;
		currentSpeed = Mathf.Round (currentSpeed); //Round the speed 

		if(currentSpeed < topSpeed && currentSpeed > -maxReverseSpeed && !breaked){
			wheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical"); // Asign speed
			wheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");
		}else{
			wheelRR.motorTorque = 0f; // Asign speed 0 if we reach the max speed
			wheelRL.motorTorque = 0f;
		}

		//Stop the car
		if (Input.GetButton ("Vertical") == false) {
			wheelRR.brakeTorque = desalerationSpeed;
			wheelRL.brakeTorque = desalerationSpeed;
		}else{
			wheelRR.brakeTorque = 0f;
			wheelRL.brakeTorque = 0f;
		}
		
		
		float speedFactor = rigidbody.velocity.magnitude / lowestSteerAtSpeed;
		float currentSteerAngle = Mathf.Lerp (lowSpeedSteerAngle,highSpeedSteerAngle,speedFactor);
		currentSteerAngle *= Input.GetAxis ("Horizontal");
		wheelFL.steerAngle = currentSteerAngle;
		wheelFR.steerAngle = currentSteerAngle;

	}

	void BackLight(){
		if (currentSpeed > 0 && Input.GetAxis ("Vertical") < 0 && !breaked) {

			backLightObject.renderer.material = breakLightMaterial;

		}else if(currentSpeed < 0 && Input.GetAxis ("Vertical") > 0 && !breaked){

			backLightObject.renderer.material = breakLightMaterial;

		}else if(currentSpeed < 0 && Input.GetAxis ("Vertical") < 0 && !breaked){

			backLightObject.renderer.material = reverseLightMaterial;

		}else if(!breaked){

			backLightObject.renderer.material = idleLightMaterial;
		}
	}

	void WheelPosition(){ //Simulates suspension
		RaycastHit hit;
		Vector3 wheelPos;

		if(Physics.Raycast(wheelFL.transform.position, -wheelFL.transform.up, out hit,wheelFL.radius+wheelFL.suspensionDistance)){
			wheelPos = hit.point + wheelFL.transform.up * wheelFL.radius;
		}else{
			wheelPos = wheelFL.transform.position - wheelFL.transform.up * wheelFL.suspensionDistance;
		}

		wheelFLTrans.position = wheelPos;

		if(Physics.Raycast(wheelFR.transform.position, -wheelFR.transform.up, out hit,wheelFR.radius+wheelFR.suspensionDistance)){
			wheelPos = hit.point + wheelFR.transform.up * wheelFR.radius;
		}else{
			wheelPos = wheelFR.transform.position - wheelFR.transform.up * wheelFR.suspensionDistance;
		}
		
		wheelFRTrans.position = wheelPos;

		if(Physics.Raycast(wheelRL.transform.position, -wheelRL.transform.up, out hit,wheelRL.radius+wheelRL.suspensionDistance)){
			wheelPos = hit.point + wheelRL.transform.up * wheelRL.radius;
		}else{
			wheelPos = wheelRL.transform.position - wheelRL.transform.up * wheelRL.suspensionDistance;
		}
		
		wheelRLTrans.position = wheelPos;
		
		if(Physics.Raycast(wheelRR.transform.position, -wheelRR.transform.up, out hit,wheelRR.radius+wheelRR.suspensionDistance)){
			wheelPos = hit.point + wheelRR.transform.up * wheelRR.radius;
		}else{
			wheelPos = wheelRR.transform.position - wheelRR.transform.up * wheelRR.suspensionDistance;
		}
		
		wheelRRTrans.position = wheelPos;
	}

	void HandBreak(){
		if (Input.GetButton("Jump")){
			breaked = true;
		}
		else{
			breaked = false;
		}
		if (breaked){
			if (currentSpeed > 1f){
				wheelFR.brakeTorque = maxBreakTorque;
				wheelFL.brakeTorque = maxBreakTorque;
				wheelRR.motorTorque =0f;
				wheelRL.motorTorque =0f;
				SetRearSlip(slipForwardFriction ,slipSidewayFriction);
			}
			else if (currentSpeed < 0f){
				wheelRR.brakeTorque = maxBreakTorque;
				wheelRL.brakeTorque = maxBreakTorque;
				wheelRR.motorTorque =0f;
				wheelRL.motorTorque =0f;
				SetRearSlip(1f ,1f);
			}
			else {
				SetRearSlip(1f ,1f);
			}
			if (currentSpeed < 1f && currentSpeed > -1f){
				backLightObject.renderer.material = idleLightMaterial;
			}
			else {
				backLightObject.renderer.material = breakLightMaterial;
			}
		}
		else {
			wheelFR.brakeTorque = 0f;
			wheelFL.brakeTorque = 0f;
			SetRearSlip(myForwardFriction ,mySidewayFriction);
		}
	}

	void ReverseSlip(){
		if (currentSpeed <0f){
			SetFrontSlip(slipForwardFriction ,slipSidewayFriction);
		}
		else {
			SetFrontSlip(myForwardFriction ,mySidewayFriction);
		}
	}

	void SetRearSlip (float currentForwardFriction  , float currentSidewayFriction  ){
		WheelFrictionCurve rr = wheelRR.forwardFriction;
		WheelFrictionCurve rl = wheelRL.forwardFriction;

		rr.stiffness = currentForwardFriction;
		rl.stiffness = currentForwardFriction;

		wheelRR.forwardFriction = rr;
		wheelRL.forwardFriction = rl;

		rr = wheelRR.sidewaysFriction;
		rl = wheelRL.sidewaysFriction;

		rr.stiffness = currentSidewayFriction;
		rl.stiffness = currentSidewayFriction;

		wheelRR.sidewaysFriction = rr;
		wheelRL.sidewaysFriction = rl;


	}

	void SetFrontSlip (float currentForwardFriction , float currentSidewayFriction ){
		WheelFrictionCurve fr = wheelFR.forwardFriction;
		WheelFrictionCurve fl = wheelFL.forwardFriction;

		fr.stiffness = currentForwardFriction;
		fl.stiffness = currentForwardFriction;

		wheelFR.forwardFriction = fr;
		wheelFL.forwardFriction = fl;

		fr = wheelFR.sidewaysFriction;
		fl = wheelFL.sidewaysFriction;

		fr.stiffness = currentSidewayFriction;
		fl.stiffness = currentSidewayFriction;

		wheelFR.sidewaysFriction = fr;
		wheelFL.sidewaysFriction = fl;


	}

	void EngineSound(){
		audio.pitch = currentSpeed / topSpeed + 1;
		int i;
		for(i=0; i < gearRatio.Length;i++){

		
			if(gearRatio[i] > currentSpeed){

				break;
			}
		}

		float gearMinValue = 0.0f;
		float gearMaxValue = 0.0f;

		if(i >=4 ){
			i =4;
		}

		if(i==0){
			gearMinValue = 0f;
			gearMaxValue = gearRatio[i];
		}else{
			gearMinValue = gearRatio[i-1];
		}

		gearMaxValue = gearRatio[i];
		float enginePitch = ((currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue))+1f;
		audio.pitch = enginePitch;
	}


	void OnGUI(){ // Draws the GUI for the speed
		GUI.DrawTexture(new Rect(Screen.width-300,Screen.height-150,300,150),speedOMeterDial); //Draw Texture
		float speedFactor = currentSpeed / topSpeed;
		float rotationAngle = Mathf.Lerp (0, 180, Mathf.Abs(speedFactor));
		GUIUtility.RotateAroundPivot(rotationAngle,new Vector2(Screen.width -150,Screen.height));
		GUI.DrawTexture(new Rect(Screen.width-300,Screen.height-150,300,300),speedOmeterPointer); //Draw Texture
	}

	void OnCollisionEnter(Collision other){

		if(other.transform != transform && other.contacts.Length != 0){
			for(int i =0; i < other.contacts.Length; i++){

				if(other.relativeVelocity.magnitude > 10){
					Instantiate(spark,other.contacts[i].point,Quaternion.identity);
					GameObject clone = (GameObject)Instantiate(collisionSound,other.contacts[i].point,Quaternion.identity);
					clone.transform.parent =transform;
				}
			}
		}

	}





}
