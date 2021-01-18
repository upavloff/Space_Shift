using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;

    private float baseSpeed = 10.0f;
    private float rotSpeedX = 5.0f;
    private float rotSpeedY = 2.5f;

    private bool IsSupportingAccelerometer=true;

    //private 

    void Start(){
    	controller = GetComponent<CharacterController>();
    	if (SystemInfo.supportsAccelerometer){
    		Debug.Log("supportsAccelerometer");
    	}else{
    		Debug.Log(" don't supportsAccelerometer");
    		IsSupportingAccelerometer=false;
    	}

    }
    
    void Update(){
    	if (IsSupportingAccelerometer){
    		//forward velocity
	    	Vector3 moveVector = transform.forward * baseSpeed;

	    	//player input
	    	Vector3 inputs = Input.acceleration;
	    	//get the delta direction
	    	Vector3 yaw = inputs.x * transform.right * rotSpeedX * Time.deltaTime;
	    	Vector3 pitch = inputs.y * transform.up * rotSpeedY * Time.deltaTime;

	    	Vector3 dir = yaw + pitch;

	    	float maxX = Quaternion.LookRotation(moveVector + dir).eulerAngles.x;

	    	if (maxX < 90 && maxX > 70 || maxX >270 && maxX < 290){
	    		//too far don't do anything
	    	}else{
	    		moveVector += dir;

	    		transform.rotation = Quaternion.LookRotation(moveVector);
	    	}

	    	controller.Move(moveVector * Time.deltaTime);
    	}else{
    		Debug.Log("nan");
    	}
    	

    }

}
