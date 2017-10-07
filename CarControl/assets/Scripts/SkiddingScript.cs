using UnityEngine;
using System.Collections;

public class SkiddingScript : MonoBehaviour {

	public float currentFrictionValue;
	public GameObject skidSound;
	public GameObject skidSmoke;
	public float smokeDepth=0.4f;
	public float soundEmision =15f;
	private float soundWait=0;
	private WheelHit hit;
	private WheelCollider wheelHit;
	public float skidAt =0.5f;
	public float markWidth = 0.2f;
	public bool rearWheel=false;
	private int skidding ;
	private Vector3 [] lastPos = new Vector3[2];
	public Material skidMaterial;
	// Use this for initialization
	void Start () {
		//skidSmoke.transform.position = transform.position;
		skidSmoke.transform.position = new Vector3(skidSmoke.transform.position.x,skidSmoke.transform.position.y-smokeDepth,transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		wheelHit = (WheelCollider)transform.GetComponent(typeof(WheelCollider));
		wheelHit.GetGroundHit (out hit);
		currentFrictionValue = Mathf.Abs(hit.sidewaysSlip);
		WheelCollider rpmCollider = (WheelCollider) transform.GetComponent(typeof(WheelCollider)); 
		float rpm = rpmCollider.rpm;

		if ( skidAt <= currentFrictionValue && soundWait <=0f || rpm < 300f && Input.GetAxis("Vertical") > 0f && soundWait <=0f && rearWheel && hit.collider){

			Instantiate(skidSound,hit.point,Quaternion.identity);
			soundWait=1;
		}

		soundWait -= Time.deltaTime * soundEmision;

		if ( skidAt <= currentFrictionValue ||  rpm < 300f && Input.GetAxis("Vertical") > 0f && rearWheel && hit.collider){
			SkidMesh();
			skidSmoke.particleEmitter.emit = true;
		}else{
			skidSmoke.particleEmitter.emit = false;
			skidding =0;
		}

	}

	void SkidMesh(){
		GameObject mark = new GameObject ("Mark");
		MeshFilter filter =(MeshFilter) mark.AddComponent (typeof(MeshFilter));
	    mark.AddComponent (typeof(MeshRenderer)) ;
		Mesh markMesh = new Mesh ();
		Vector3 [] vertices = new Vector3[4];
		int [] triangles;

		if(skidding == 0){
			vertices[0]= hit.point +  Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)* new Vector3(markWidth,0.01f,0);
			vertices[1]= hit.point +  Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)* new Vector3(-markWidth,0.01f,0);
			vertices[2]= hit.point +  Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)* new Vector3(-markWidth,0.01f,0);
			vertices[3]= hit.point +  Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)* new Vector3(markWidth,0.01f,0);

			lastPos[0]=vertices[2];
			lastPos[1]=vertices[3];

			skidding = 1;
		}else{
			vertices[1]=lastPos[0];
			vertices[0]=lastPos[1];
			vertices[2]= hit.point + Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)* new Vector3(-markWidth,0.01f,0);
			vertices[3]= hit.point + Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)* new Vector3(markWidth,0.01f,0);
			lastPos[0]=vertices[2];
			lastPos[1]=vertices[3];
		}
		triangles = new int[6]{0,1,2,2,3,0};
		markMesh.vertices = vertices;
		markMesh.triangles = triangles;
		markMesh.RecalculateNormals();
		Vector2 [] uvm = new Vector2[4];

		uvm [0] = new Vector2 (1, 0);
		uvm [1] = new Vector2 (0, 0);
		uvm [2] = new Vector2 (0, 1);
		uvm [3] = new Vector2 (1, 1);

		markMesh.uv = uvm;
		filter.mesh = markMesh;
		mark.renderer.material = skidMaterial;
		mark.AddComponent (typeof(DestroyTimerScript));
	}
}
