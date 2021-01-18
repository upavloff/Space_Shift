using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

	public Transform cameraTarget;
	public Transform player;
	public float cameraRotationSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraTarget.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation, cameraRotationSpeed*Time.deltaTime);

    }
}
