using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;

    private float baseSpeed = 10.0f;
    private float rotSpeedHorizontal = 2.0f;
    private float rotSpeedVertical = 1.0f;

    public bool usingGyroscope=false;

    private Quaternion initGyroInput;

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    private Vector3 relativeCoordinate = new Vector3(0,0,0);
    

    

    void Start(){
    	controller = GetComponent<CharacterController>();
    	if (SystemInfo.supportsGyroscope){
    		Input.gyro.enabled = true;
    		Debug.Log("supports Gyroscope");
    		Screen.orientation = ScreenOrientation.LandscapeLeft;
    		Input.gyro.updateInterval = 0.0167F;
    		usingGyroscope=true;
    		initGyroInput = Input.gyro.attitude;
    	}else{
    		Debug.Log(" don't supports Gyroscope or Accelerometer");
    	}
    	guiStyle.fontSize = 30; //change the font size

    }

    
    void OnGUI(){
    	if (usingGyroscope) {
			GUI.Box(new Rect(380, Screen.height - 200,400,200),"gyro");

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
    }
    
    void Update(){
    	if (usingGyroscope){
    		Quaternion relativeRotation = Quaternion.Inverse(initGyroInput)*Input.gyro.attitude;

	    	float coordX = relativeRotation.eulerAngles.x;
	    	float coordY = relativeRotation.eulerAngles.y;

	    	//euleur coordinate can't be negative they go straight from 0 to 360 this
	    	//how you fix it :
	    	if (coordX>180){
	    		coordX=coordX-360;
	    	}if (coordY>180){
	    		coordY= coordY-360;
	    	}
	    	relativeCoordinate = new Vector3(coordX,coordY,0f);
	    	transform.rotation *= Quaternion.Euler(0.1f*-coordX,0f,0.1f*-coordY);		//use deltaTime ?
	    }else{	    	
	    	transform.Rotate( rotSpeedVertical*Input.GetAxis("Vertical"),0f,rotSpeedHorizontal * - Input.GetAxis("Horizontal"));
	    }

	    transform.position +=  transform.forward * baseSpeed * Time.deltaTime;
    }

    public void SwitchControl(){
    	Debug.Log("Update Coordinate");
    	if (usingGyroscope){
    		initGyroInput = Input.gyro.attitude;
    	}
    }

}
