using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Rigidbody rb;

    private float baseSpeed = 150.0f;
    private float rotSpeedHorizontal = 5.0f;
    private float rotSpeedVertical = 2.0f;

    public bool usingGyroscope=false;

    private Quaternion initGyroInput;

    public MeshGenerator meshScript;

    //private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    public Vector3 relativeCoordinate = new Vector3(0,0,0);
    
	//public Scrollbar scrollbar;

	public bool isPlaying = false;    

	public Vector3 initPosition;
    public Quaternion initRotation;

    //utilisateur rotation
    private float angle = 0f;
    public float maxAngle = 0f;

    public bool modificationDueToAngle = false;

    void Start(){
    	controller = GetComponent<CharacterController>();
    	rb = GetComponent<Rigidbody>();
    	
    	if (SystemInfo.supportsGyroscope){
    		Input.gyro.enabled = true;
    		//Debug.Log("supports Gyroscope");
    		Screen.orientation = ScreenOrientation.LandscapeLeft;
    		Input.gyro.updateInterval = 0.0167F;
    		usingGyroscope=true;
    		initGyroInput = Input.gyro.attitude;
    	}else{
    		rotSpeedHorizontal *= 5f;
    		rotSpeedVertical *= 5f;
    		//Debug.Log(" don't supports Gyroscope or Accelerometer");
    	}
    	//guiStyle.fontSize = 30; //change the font size
		//scrollbar.onValueChanged.AddListener((float val) => ScrollbarCallback(val));
    	initPosition = transform.position;
    	initRotation = transform.rotation;
    }

    /*
    void OnGUI(){
    	if (usingGyroscope) {
			GUI.Box(new Rect(380, Screen.height - 200,400,200),"");

			GUI.Label(new Rect(400, Screen.height - 200,200,100),"Gyroscope X = "+initGyroInput.x,guiStyle);
			GUI.Label(new Rect(400,Screen.height - 120,200,50),"Gyroscope Y = "+initGyroInput.y,guiStyle);
			GUI.Label(new Rect(400,Screen.height - 60,200,50),"Gyroscope Z = "+initGyroInput.z,guiStyle);

			GUI.Label(new Rect(800,Screen.height - 120,200,50)," relativeCoordinate = "+relativeCoordinate,guiStyle);
			
		}else{
			GUI.Box(new Rect(20, Screen.height - 200,400,200),"");
			GUI.Label(new Rect(40, Screen.height - 200,200,100),"test 1",guiStyle);
			GUI.Label(new Rect(40,Screen.height - 120,200,50),"test 2",guiStyle);
			GUI.Label(new Rect(40,Screen.height - 60,200,50),"test 3",guiStyle);
		}
    }*/
    
    void Update(){
    	if (!isPlaying) return;
	    
	    controller.Move(transform.forward * baseSpeed * Time.deltaTime);
    	
    	if (usingGyroscope){
    		angle = Quaternion.Angle(initGyroInput, Input.gyro.attitude);
    		if (angle > 60f  && !modificationDueToAngle){
    			Debug.Log("Player make big rotation");
    			modificationDueToAngle = true;
    			meshScript.girth =  Mathf.RoundToInt(meshScript.girth/1.1f); 
    		}
    		if (angle>maxAngle) maxAngle = angle;
    		
    		Quaternion relativeRotation = Quaternion.Inverse(initGyroInput)*Input.gyro.attitude;

	    	float coordX = relativeRotation.eulerAngles.x; 
	    	float coordY = relativeRotation.eulerAngles.y;
	    	float coordZ = relativeRotation.eulerAngles.z;

	    	//euleur coordinate can't be negative they go straight from 0 to 360 this
	    	//how you fix it :
	    	if (coordX>180){
	    		coordX=coordX-360;
	    	}if (coordY>180){
	    		coordY= coordY-360;
	    	}if (coordZ>180){
	    		coordZ= coordZ-360;
	    	}
	    	relativeCoordinate = new Vector3(coordX,coordY,0f)*Time.deltaTime;
	    	//transform.rotation *= Quaternion.Euler(Time.deltaTime*rotSpeedVertical*-coordX,0f,Time.deltaTime*rotSpeedHorizontal*-coordY);		//use deltaTime ?
	    	transform.rotation *= Quaternion.Euler(Time.deltaTime*rotSpeedVertical*-coordX,0f,Time.deltaTime*rotSpeedHorizontal*coordZ);		//use deltaTime ?
	    }else{	    	
	    	transform.Rotate( rotSpeedVertical*Input.GetAxis("Vertical")*0.1f,0f,rotSpeedHorizontal * - Input.GetAxis("Horizontal")*0.1f);
	    }

	    //transform.position +=  transform.forward * baseSpeed * Time.deltaTime;
	    //rb.AddRelativeForce(transform.forward * baseSpeed /* Time.deltaTime*/);
	    /*if ( controller.Move(transform.forward * baseSpeed * Time.deltaTime) > 0){
	   		isPlaying = false;
	   		StartCoroutine(HandleCollision());
	    }*/
    }

    public void PlayButtonClicked(){
    	Debug.Log("Update Coordinate");
    	if (usingGyroscope){
    		initGyroInput = Input.gyro.attitude;
    	}
    	isPlaying = true;
    }

    /*public void ScrollbarCallback(float value)
	{
	    Debug.Log((value+1)*100);
	    baseSpeed = (value+1)*100;
	}*/


	/*void OnGUI(){
		GUI.Label(new Rect(Screen.width-500, 40,200,100),"angle = "+angle);
	}*/



	/*public IEnumerator GetBackInPlace(Vector3 destination, Vector3 before_destination){
		Debug.Log("destination is "+destination);
		Vector3 posInit = transform.position;
		Vector3 after_posInit = transform.position+transform.forward*baseSpeed;
		Vector3 previousTransform;
		//for (float f = 0.05f; f<=1; f=f+0.05f){
		float tParam = 0f;
		while (tParam <1 ){
			tParam += Time.deltaTime * 0.5f;
			previousTransform = transform.position;
			transform.position = CubicCurve(posInit, after_posInit, before_destination, destination, tParam);
			transform.rotation = Quaternion.LookRotation( (CubicCurve(posInit, after_posInit, before_destination, destination, tParam+0.03f) - transform.position) );
			yield return new WaitForEndOfFrame();
		}
		isPlaying = true;
	}*/


	//part to handle the get back in place
	/*public Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

    public Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t){
    	Vector3 p0 = Lerp(a, b, t);
		Vector3 p1 = Lerp(b, c, t);
		return Lerp(p0,p1, t); 
    }

    public Vector3 CubicCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t){
    	Vector3 p0 = QuadraticCurve(a, b, c, t);
		Vector3 p1 = QuadraticCurve(b, c, d, t);
		return Lerp(p0,p1, t); 
    }*/

    public void Reset(){
    	transform.position = initPosition;
    	transform.rotation = initRotation;
    }

}
