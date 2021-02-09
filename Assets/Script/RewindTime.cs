using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTime : MonoBehaviour
{
	bool isRewinding = false;

	public float recordTime = 1.5f;

	List<PointInTime> pointsInTime;

	public Transform cameraTransform;

	public GameObject postProcessing;

    // Start is called before the first frame update
    void Start()
    {
        pointsInTime = new List<PointInTime>();
    }

    void FixedUpdate ()
	{
		if (isRewinding)
			Rewind();
		else
			Record();
	}

	void Rewind ()
	{
		if (pointsInTime.Count > 0)
		{
			PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.positionPlayer;
			transform.rotation = pointInTime.rotationPlayer;
			cameraTransform.position = pointInTime.positionCamera;
			cameraTransform.rotation = pointInTime.rotationCamera;
			pointsInTime.RemoveAt(0);
		} else
		{
			StopRewind();
		}
		
	}

	void Record ()
	{
		//player
		if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
		{
			pointsInTime.RemoveAt(pointsInTime.Count - 1);
		}

		pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation, cameraTransform.position, cameraTransform.rotation));
	}

	public void StartRewind ()
	{
		isRewinding = true;
		Debug.Log("start rewind");
		postProcessing.SetActive(true);
	}

	public void StopRewind ()
	{
		isRewinding = false;
		postProcessing.SetActive(false);
	}
}
