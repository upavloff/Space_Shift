using System.Collections;
using UnityEngine;

public class HealthCollision : MonoBehaviour
{

	public MeshGenerator meshScript;

	public PlayerMotor motorScript;

	public MeshRenderer renderer;

	public int maxHealth = 3;
	public int currentHealth ;

	Vector3 destination = new Vector3(0,0,0);
	Vector3 supposedDestination = new Vector3(0,0,0);

	private int distToRecover = 200;
	public float timeColorChange = .3f;

    // Start is called before the first frame update
    void Start()
    {
	    renderer = transform.GetChild(0).GetComponent<MeshRenderer>();    
	    motorScript = transform.GetComponent<PlayerMotor>();
	    currentHealth = maxHealth;
    }


    public IEnumerator HandleCollision(){
    	currentHealth --;
    	if (currentHealth<=0){
    		Die();
    		yield break;
    	}
    	float dist = 300000f;
    	//Vector3 destination = new Vector3();
    	Vector3 before_destination = new Vector3();
    	Vector3[] centerMeshOld = meshScript.centerMeshOld;
    	for (int i=1; i<centerMeshOld.Length;i++){
    		float tmp = Vector3.Distance(transform.position,centerMeshOld[i]);
    		if (tmp <= dist){
    			dist = tmp;
    			//if point destination exceed oldmesh length go on the new one
    			supposedDestination = centerMeshOld[i];
    			if (i+distToRecover >= centerMeshOld.Length){
	    			destination = meshScript.centerMeshNew[i+distToRecover-centerMeshOld.Length]; 
	    			Debug.Log("blllluhhhhh");
    			}else{
    				destination = centerMeshOld[i+distToRecover];
    			}
    			//same thing for the point just before
    			if (i+distToRecover-1 >= centerMeshOld.Length){
    				before_destination = centerMeshOld[i-1+distToRecover-centerMeshOld.Length];
    			}else{
    				before_destination = centerMeshOld[i-1+distToRecover];
    			}
    		}else{
    			break;
    		}
    	}
    	StartCoroutine(motorScript.GetBackInPlace(destination, before_destination));
    	for (int i=0; i<8; i++){
			renderer.material.color = Color.red;
			yield return new WaitForSeconds(timeColorChange);
			renderer.material.color = Color.white;
			yield return new WaitForSeconds(timeColorChange);
    	}

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
