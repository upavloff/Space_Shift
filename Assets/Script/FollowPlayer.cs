using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FollowPlayer : MonoBehaviour
{

	public Transform cameraTarget;
	public Transform player;
	public float cameraRotationSpeed = 50f;
    public float positionAdaptSpeed = 10f;
	public float rotationAdaptSpeed = 10f;

    public float menuRotation = 5f;

	private bool usingGyroscope=false;

    //private GUIStyle guiStyle = new GUIStyle(); //create a new variable

	private PlayerMotor infoMotor;

    public bool isPlaying = false;   
    public bool cameraTurning = false;  

    public bool hurtMode = false;

    public Vector3 initPosition;
    public Quaternion initRotation;
 
	void Start(){
		if (SystemInfo.supportsGyroscope){
			usingGyroscope=true;
		}
		//guiStyle.fontSize = 30;
		infoMotor = player.GetComponent<PlayerMotor>();

        initPosition = transform.position;
        initRotation = transform.rotation;
	}

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying) return;
        transform.position = Vector3.Lerp(transform.position ,cameraTarget.position,Time.deltaTime*positionAdaptSpeed);
        if (hurtMode){
            //transform.rotation = Quaternion.LookRotation(player.position - transform.position);
            transform.LookAt(player.position);
            return;
        }
        //transform.position =Vector3.Scale( Vector3.Slerp(transform.position ,cameraTarget.position,Time.deltaTime*positionAdaptSpeed),Vector3.up)+new Vector3(cameraTarget.position.x,0,cameraTarget.position.z);
        //transform.position = player.position + offset;
        //transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, cameraRotationSpeed*Time.deltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, cameraRotationSpeed*Time.deltaTime);
        //transform.rotation = player.rotation;

        //----- a revoir
        /*if (usingGyroscope){
        	Vector3 relCoor = infoMotor.relativeCoordinate;
            //transform.rotation = player.rotation*Quaternion.Euler(rotationAdaptSpeed*-relCoor.x,0f,rotationAdaptSpeed*-relCoor.y);
        	transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation*Quaternion.Euler(rotationAdaptSpeed*-relCoor.x,0f,rotationAdaptSpeed*relCoor.z),Time.deltaTime*rotationAdaptSpeed);
        }*/

        transform.rotation = Quaternion.Slerp(transform.rotation ,player.rotation,Time.deltaTime*rotationAdaptSpeed);
    }

    public void PlayButtonClicked(){
        StartCoroutine( Waiter());
    }

    IEnumerator Waiter(){
        yield return new WaitForSeconds(0.3f);
        isPlaying = true;
    }

    public void GetToShopView(){
        StartCoroutine(TurningToShopView());
    }

    public void GetToMenueView(){
        StartCoroutine(TurningToMenuView());
    }

    IEnumerator TurningToShopView(){
        if (cameraTurning) yield break;
        cameraTurning = true;
        while (transform.rotation != Quaternion.Euler(0,112.95f,0)){
            transform.rotation = Quaternion.Slerp(transform.rotation ,Quaternion.Euler(0,112.95f,0),Time.deltaTime*menuRotation);
            yield return null;
        }
        cameraTurning = false;
    }


    IEnumerator TurningToMenuView(){
        if (cameraTurning) yield break ;
        cameraTurning = true;
        while (transform.rotation != Quaternion.Euler(0,0,0)){
            transform.rotation = Quaternion.Slerp(transform.rotation ,Quaternion.Euler(0,0,0),Time.deltaTime*menuRotation);
            yield return null;
        }
        cameraTurning = false;
    }
    
    public void Reset(){
        transform.position = initPosition;
        transform.rotation = initRotation;
    }
}
