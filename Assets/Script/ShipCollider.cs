using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipCollider : MonoBehaviour
{
	public Rigidbody rb;
	public HealthCollision healthCollisionScript;
	public PlayerMotor motorScript;
	public FollowPlayer followScript;

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
    	Invoke("Vulnerability", 3.2f);
    }

    void Vulnerability(){
    	invincible = false;
    }
}
