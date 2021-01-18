using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f;

    public float rotationSpeed = 100.0f;

    // Update is called once per frame
    void Update()
    {
        float translation = CrossPlatformInputManager.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotation = CrossPlatformInputManager.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        transform.Translate(0,0,translation);
        transform.Rotate(0,rotation,0);
    }
}
