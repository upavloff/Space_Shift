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

    public bool usingAccelerometer=false;
    public bool usingGyroscope=false;

    //private 

    void Start(){
    	controller = GetComponent<CharacterController>();
    	if (SystemInfo.supportsAccelerometer){
    		Debug.Log("supportsAccelerometer");
    		Screen.orientation = ScreenOrientation.LandscapeLeft;
    		usingAccelerometer=true;
    	}
    	if (SystemInfo.supportsGyroscope){
    		Debug.Log("supportsGyroscope");
    		Screen.orientation = ScreenOrientation.LandscapeLeft;
    		Input.gyro.enabled = true;
    		Input.gyro.updateInterval = 0.0167F;
    		usingGyroscope=true;
    	}else{
    		Debug.Log(" don't supports Gyroscope or Accelerometer");
    	}

    }

    void OnGUI(){
    	if (usingAccelerometer){
    		GUI.Box(new Rect(20, Screen.height - 100,200,100),"");
	    	GUI.Label(new Rect(40, Screen.height - 100,200,100),"acceleration X = "+Input.acceleration.x);
			GUI.Label(new Rect(40,Screen.height - 60,200,50),"acceleration Y = "+Input.acceleration.y);
			GUI.Label(new Rect(40,Screen.height - 30,200,50),"acceleration Z = "+Input.acceleration.z);
		}else if (usingGyroscope) {
			GUI.Box(new Rect(20, Screen.height - 100,200,100),"");
			GUI.Label(new Rect(40, Screen.height - 100,200,100),"Gyroscope X = "+Input.gyro.rotationRate.x);
			GUI.Label(new Rect(40,Screen.height - 60,200,50),"Gyroscope Y = "+Input.gyro.rotationRate.y);
			GUI.Label(new Rect(40,Screen.height - 30,200,50),"Gyroscope Z = "+Input.gyro.rotationRate.z);
		}else{
			GUI.Box(new Rect(20, Screen.height - 100,200,100),"");
			GUI.Label(new Rect(40, Screen.height - 100,200,100),"test 1");
			GUI.Label(new Rect(40,Screen.height - 60,200,50),"test 2");
			GUI.Label(new Rect(40,Screen.height - 30,200,50),"test 3");
		}
    }
    
    void Update(){
    	//forward velocity
    	//Vector3 inputs = Input.acceleration;
 
    	if (usingAccelerometer){
	    	transform.Rotate( rotSpeedVertical*Input.acceleration.y,0f,rotSpeedHorizontal * (-Input.acceleration.x ));
    	}else if (usingGyroscope){
	    	transform.Rotate( rotSpeedVertical*(-Input.gyro.rotationRate.x),0f,rotSpeedHorizontal * (-Input.gyro.rotationRate.y));
	    }else{
	    	transform.Rotate( rotSpeedVertical*Input.GetAxis("Vertical"),0f,rotSpeedHorizontal * - Input.GetAxis("Horizontal"));
	    }

	    transform.position +=  transform.forward * baseSpeed * Time.deltaTime;
    	//player input
    	/*Vector3 moveVector = transform.forward * baseSpeed;
    	//get the delta direction
    	//Vector3 yaw = (inputs.z + Input.GetAxis("Horizontal") ) * transform.forward * rotSpeedZ * Time.deltaTime;
    	Vector3 pitch = (inputs.y + Input.GetAxis("Vertical") ) * transform.up * rotSpeedY * Time.deltaTime;

    	//Vector3 dir = yaw + pitch;

    	float maxX = Quaternion.LookRotation(moveVector + dir).eulerAngles.x;

    	if (maxX < 90 && maxX > 70 || maxX >270 && maxX < 290){
    		//too far don't do anything
    	}else{
    		moveVector += dir;

    		transform.rotation = Quaternion.LookRotation(moveVector);
    	}

    	controller.Move(moveVector * Time.deltaTime);*/


    }

    public void SwitchControl(){
    	Debug.Log("Control Switching");
    	if (usingAccelerometer){
    		usingAccelerometer=false;
    		usingGyroscope=true;
    	}else if( usingGyroscope){
    		usingGyroscope=false;
    		usingAccelerometer=true;
    	}
    }

}
