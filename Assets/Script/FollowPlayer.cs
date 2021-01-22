using UnityEngine;
using UnityEngine.UI;

public class FollowPlayer : MonoBehaviour
{

	public Transform cameraTarget;
	public Transform player;
	public float cameraRotationSpeed = 50f;
	public float extraRotSpeed=10f;

	private bool usingGyroscope=false;

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

	public Scrollbar scrollbar;

	private PlayerMotor infoMotor;


 
	void Start(){
		if (SystemInfo.supportsGyroscope){
			usingGyroscope=true;
		}
		scrollbar.onValueChanged.AddListener((float val) => ScrollbarCallback(val));
		guiStyle.fontSize = 30;
		infoMotor = player.GetComponent<PlayerMotor>();
	}

    // Update is called once per frame
    void Update()
    {
        transform.position =Vector3.Slerp(transform.position ,cameraTarget.position,Time.deltaTime*extraRotSpeed);
        //transform.position =Vector3.Scale( Vector3.Slerp(transform.position ,cameraTarget.position,Time.deltaTime*extraRotSpeed),Vector3.up)+new Vector3(cameraTarget.position.x,0,cameraTarget.position.z);
        //transform.position = player.position + offset;
        //transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, cameraRotationSpeed*Time.deltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, cameraRotationSpeed*Time.deltaTime);
        //transform.rotation = player.rotation;
        if (usingGyroscope){
        	Vector3 relCoor = infoMotor.relativeCoordinate;
        	transform.rotation = player.rotation*Quaternion.Euler(extraRotSpeed*-relCoor.x,0f,extraRotSpeed*-relCoor.y);
        }
        transform.rotation = player.rotation*Quaternion.Euler(10,10,0);
        
    }

    void ScrollbarCallback(float value)
	{
	    Debug.Log(value*20+1);
	    extraRotSpeed = value*20+1;
	}

	public void SetToZero(){
		extraRotSpeed=5f;
	}

	void OnGUI(){
		GUI.Label(new Rect(Screen.width-500, 40,200,100),"RotSpeed = "+extraRotSpeed,guiStyle);
	}
}
