using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipCollider : MonoBehaviour
{
	public Rigidbody rb;
	public HealthCollision healthCollisionScript;
	public PlayerMotor motorScript;


	void Start(){
		rb = transform.GetComponent<Rigidbody>();
		rb.isKinematic = true;
		rb.useGravity = false;
	}

    void OnTriggerEnter(Collider other){
    	if (other.tag == "tagPlayer" || !motorScript.isPlaying ) return;
    	Debug.Log("other tag"+other.tag);
		motorScript.isPlaying = false;
    	StartCoroutine( healthCollisionScript.HandleCollision() );
    }
}
