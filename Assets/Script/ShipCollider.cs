using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipCollider : MonoBehaviour
{
	public Rigidbody rb;
	public HealthCollision healthCollisionScript;
	public RewindTime rewindScript;

	private bool invincible = false;

	void Start(){
		rb = transform.GetComponent<Rigidbody>();
		rb.isKinematic = true;
		rb.useGravity = false;
	}

    void OnTriggerEnter(Collider other){
    	if (invincible || other.tag == "Player") return;
    	invincible = true;
    	Debug.Log("HandleCollision a cause de "+other.tag);
    	healthCollisionScript.HandleCollision();
    	Invoke("Vulnerability", rewindScript.recordTime);
    }

    void Vulnerability(){
    	invincible = false;
    }
}
