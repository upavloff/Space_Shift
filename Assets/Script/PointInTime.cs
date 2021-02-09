using UnityEngine;

public class PointInTime {

	public Vector3 positionPlayer;
	public Quaternion rotationPlayer;

	public Vector3 positionCamera;
	public Quaternion rotationCamera;

	public PointInTime (Vector3 _positionPlayer, Quaternion _rotationPlayer, Vector3 _positionCamera, Quaternion _rotationCamera)
	{
		positionPlayer = _positionPlayer;
		rotationPlayer = _rotationPlayer;

		positionCamera = _positionCamera;
		rotationCamera = _rotationCamera;
	}

}