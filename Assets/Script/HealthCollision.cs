using System.Collections;
using UnityEngine;

public class HealthCollision : MonoBehaviour
{

	public MeshGenerator meshScript;

	public PlayerMotor motorScript;

	public FollowPlayer followScript;

	public MeshRenderer renderer;

	public RewindTime rewindScript;


	private int maxHealth = 4;
	private int currentHealth ;

	Vector3 destination = new Vector3(0,0,0);
	Vector3 supposedDestination = new Vector3(0,0,0);

	private int distToRecover = 300;
	private int timeToRecover = 12;

	public float timeColorChange = .3f;

    // Start is called before the first frame update
    void Start()
    {
	    renderer = transform.GetChild(0).GetComponent<MeshRenderer>();    
	    motorScript = transform.GetComponent<PlayerMotor>();
	    rewindScript = transform.GetComponent<RewindTime>();
	    currentHealth = maxHealth;
    }


    public void HandleCollision(){
    	currentHealth --;
    	Debug.Log("currentHealth "+currentHealth);
    	motorScript.isPlaying = false;
		followScript.isPlaying = false;
    	if (currentHealth<0){
    		Die();
    		//yield break;
    		return;
    	}
    	rewindScript.StartRewind();
    	motorScript.isPlaying = true;
		followScript.isPlaying = true;
	}

	void Die(){
		Debug.Log("you ded");
	}

    void OnGUI(){
		GUI.Label(new Rect(Screen.width-500, 60,200,100),"currentHealth = "+currentHealth);
	}

    private void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(destination, 12f);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(supposedDestination, 12f);
		
	}
}
