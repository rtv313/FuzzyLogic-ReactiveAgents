using UnityEngine;
using System.Collections;

public class AICarScript : MonoBehaviour {

	public Transform[] path;
	public Transform pathGroup;
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
	public float maxSteer = 15.0f;
	public Vector3 centerOfMass;
	public int  currentPathObj;
	public float maxTorque = 50f;
	public float currentSpeed;
	public float topSpeed =150f;
	public float minSpeed = 100f;
	public float decellarationSpeed = 10f;
	public Renderer breakingMesh;
	public Material idleBreakLight;
	public Material activeBreakLight;
	public  bool isBreaking;
	public bool inSector;
	public float sensorLength =5f;
	public float frontSensorStartPoint = 5f;
	public float frontSensorSideDist = 5f;
	public float frontSensorAngle = 30f;
	public float sidewaySensorLength=5f;
	private int flag=0;
	public float avoidSpeed = 10f;
	public bool reversing = false;
	public float reverCounter =0.0f;
	public float waitToReverse =2.0f;
	public float reverFor =1.5f;
	public float respawnWait=5f;
	public float respawnCounter =0.0f;

	public bool avoiding =false;

	private float tiempo =0;
	public  float velocidadRotacion=2.0f;
	private EvalutionFuzz componenteFuzzy;
	public bool desacelerando;
	public float intensidadDesaceleracion;

	// Use this for initialization
	void Start () {
		setCenterOfMass ();
		GetPath ();
		getFuzzyControl ();
	}

	void getFuzzyControl(){
		componenteFuzzy = transform.GetComponent<EvalutionFuzz> ();
		componenteFuzzy.setMaxVelocidad (topSpeed);
		componenteFuzzy.setMinVelocidad (minSpeed);
		componenteFuzzy.setMaxVelocidadActual (currentSpeed);
		componenteFuzzy.setMaxAngulo (maxSteer);
		componenteFuzzy.setMaxAnguloActual (Mathf.Abs(wheelFL.steerAngle));

	}

	void updateFuzzyControl(){
		componenteFuzzy.setMaxVelocidad (topSpeed);
		componenteFuzzy.setMaxVelocidadActual (Mathf.Abs(currentSpeed));
		componenteFuzzy.setMaxAnguloActual (Mathf.Abs(wheelFL.steerAngle));
	}

	void setCenterOfMass(){
		rigidbody.centerOfMass = centerOfMass;

	}
	
	// Update is called once per frame
	void Update() {

		if(flag ==0){
			GetSteer();
		}
		updateFuzzyControl ();
		StartCoroutine (CoroutineDesacelerar());
		Move();
		BreakingEffect();
		Sensors ();
		Respawn ();
		CarSpeed ();


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

	void GetSteer(){
		Vector3 steerVector = transform.InverseTransformPoint (new Vector3(path[currentPathObj].position.x,transform.position.y,path[currentPathObj].position.z));
		float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
		float newAngle;
		newAngle = Mathf.Lerp (wheelFL.steerAngle , newSteer, (tiempo+Time.deltaTime)*velocidadRotacion); 

		if(newSteer>0){
			if (wheelFL.steerAngle >= newSteer ) {
				tiempo=0f;
			}
		}else{
			if (wheelFL.steerAngle <= newSteer ) {
				tiempo=0f;
			}
		}

		wheelFL.steerAngle = newAngle;
		wheelFR.steerAngle = newAngle;

	
	
	}



	void Acelerar(){
		rigidbody.mass = 3500;
		wheelRL.brakeTorque = 0f;
		wheelRR.brakeTorque = 0f;
		wheelRL.motorTorque = maxTorque;
		wheelRR.motorTorque = maxTorque;
		wheelFL.brakeTorque = 0f;
		wheelFR.brakeTorque = 0f;
	}

	void Reversa(){

		wheelRL.brakeTorque = 0f;
		wheelRR.brakeTorque = 0f;
		wheelFL.brakeTorque = 0f;
		wheelFR.brakeTorque = 0f;
		wheelRL.motorTorque = -maxTorque;
		wheelRR.motorTorque = -maxTorque;

	}

	void Desacelerar(){
		rigidbody.mass = 4000;
		wheelRL.brakeTorque = decellarationSpeed;
		wheelRR.brakeTorque = decellarationSpeed;
		wheelRL.motorTorque = 0f;
		wheelRR.motorTorque = 0f;

		wheelFL.brakeTorque = decellarationSpeed;
		wheelFR.brakeTorque = decellarationSpeed;
		wheelFL.motorTorque = 0f;
		wheelFR.motorTorque = 0f;
	}

	IEnumerator CoroutineDesacelerar(){
		if(componenteFuzzy.GetResultdoEscrito()=="Desacelerar" && currentSpeed >= minSpeed){
			desacelerando=true;
			isBreaking=true;
			intensidadDesaceleracion= componenteFuzzy.GetIntensidadDesfuzzificacion();
			intensidadDesaceleracion= topSpeed/intensidadDesaceleracion;
			yield return new WaitForSeconds (1f);
			updateFuzzyControl ();
			if(componenteFuzzy.GetResultdoEscrito()!="Desacelerar" || currentSpeed <= minSpeed){
				desacelerando = false;
				isBreaking=false;
			}

		}

	
	}

	void Move(){

		if(currentSpeed <= topSpeed && desacelerando==false  || currentSpeed <= minSpeed){
			if(!reversing){
				Acelerar();
			}else{

				Reversa();
			}

			
		}else if((desacelerando==true || currentSpeed >= topSpeed)&& currentSpeed >= minSpeed){

			Desacelerar();
		}
	}

	void BreakingEffect(){

		if(isBreaking){
			breakingMesh.material = activeBreakLight;
		}else{
			breakingMesh.material = idleBreakLight;
		}

	}

	void  Sensors(){
		flag = 0;
		float avoidSensitivity = 0;
		Vector3 pos;
		RaycastHit hit;
		Vector3 rightAngle = Quaternion.AngleAxis (frontSensorAngle,transform.up)*transform.forward;
		Vector3 leftAngle = Quaternion.AngleAxis (-frontSensorAngle,transform.up)*transform.forward;

		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;

		//Breaking Sensor

		if(Physics.Raycast(pos,transform.forward,out hit,sensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint" && hit.transform.tag != "Road" ){
				flag++;
				Debug.DrawLine(pos,hit.point,Color.red);
			
			}
		}else{
			wheelRL.brakeTorque = 0f;
			wheelRR.brakeTorque = 0f;
		}

	//Front Straight Right Sensor
		pos += transform.right * frontSensorSideDist;

		if(Physics.Raycast(pos,transform.forward,out hit,sensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint" && hit.transform.tag != "Road"){
				flag++;
				avoidSensitivity -=1f;
				//Debug.Log("Avoiding");
				Debug.DrawLine(pos,hit.point,Color.white);
			

		}

		}else if(Physics.Raycast(pos,rightAngle,out hit,sensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint" && hit.transform.tag != "Road"){
				avoidSensitivity -=0.5f;
				flag++;
				Debug.DrawLine(pos,hit.point,Color.white);


				

			}
		}

		//Front Straight Left Sensor
		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;
		pos -= transform.right * frontSensorSideDist;

		if(Physics.Raycast(pos,transform.forward,out hit,sensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint" && hit.transform.tag != "Road"){
				flag++;
				avoidSensitivity +=1f;
				//Debug.Log("Avoiding");
				Debug.DrawLine(pos,hit.point,Color.white);
			

				

			}

		
		}else if(Physics.Raycast(pos,leftAngle,out hit,sensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint" && hit.transform.tag != "Road"){
				flag++;
				avoidSensitivity +=0.5f;
				Debug.DrawLine(pos,hit.point,Color.white);

				

			}
		}

		//Right SideWaySensor

		if(Physics.Raycast(transform.position,transform.right,out hit,sidewaySensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint"&& hit.transform.tag != "Road"){
				flag++;
				avoidSensitivity-=0.5f;
				Debug.DrawLine(transform.position,hit.point,Color.white);
			
				

			}
		}

		//Left SideWaySensor
		
		if(Physics.Raycast(transform.position,-transform.right,out hit,sidewaySensorLength)){
			if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint"&& hit.transform.tag != "Road"){
				flag++;
				avoidSensitivity+=0.5f;
				Debug.DrawLine(transform.position,hit.point,Color.white);

				

			}
		}


		//Front Mid Sensor

		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;

		if(avoidSensitivity == 0f){
			if(Physics.Raycast(pos,transform.forward,out hit,sensorLength)){
				if(hit.transform.tag != "Terrain" && hit.transform.tag != "WayPoint"&& hit.transform.tag != "Road"){
					if(hit.normal.x <0f){
						avoidSensitivity=1.0f;
					}else{
						avoidSensitivity=-1.0f;
					}
					Debug.DrawLine(pos,hit.point,Color.white);

					

				}
			}
		}

		if(rigidbody.velocity.magnitude <2f && !reversing){

			reverCounter += Time.deltaTime;
			if(reverCounter >= waitToReverse){
				reverCounter =0f;
				reversing = true;

			}
		}else if(!reversing){
			reverCounter =0f;
		}

		if (reversing) {
			avoidSensitivity *=-1f;	
			reverCounter += Time.deltaTime;

			if(reverCounter >= reverFor){
				reverCounter = 0f;
				reversing = false;
			}
		}

		if(flag != 0){
			AvoidSteer(avoidSensitivity);
		}else{

		}
	
	}

	void AvoidSteer(float senstivity ){
		//avoiding = true;
		if(!reversing){
			Desacelerar ();
		}
		wheelFL.steerAngle = avoidSpeed*senstivity;
		wheelFR.steerAngle = avoidSpeed*senstivity;
	}

	void  Respawn(){
		if(rigidbody.velocity.magnitude <2f){
			respawnCounter += Time.deltaTime;
		}
		if(respawnCounter >= respawnWait){
			if(currentPathObj==0){
				transform.position=path[path.Length-1].position;
			}else{
				transform.position=path[currentPathObj-1].position;
			}
			respawnCounter=0f;
		}
	}

	void  CarSpeed(){
		currentSpeed = 2 * Mathf.PI * wheelRL.radius * wheelRL.rpm * 60 / 1000;
		currentSpeed = Mathf.Round (currentSpeed);
	}

}
