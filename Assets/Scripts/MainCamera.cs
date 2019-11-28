using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A test class, not in use

public class MainCamera : MonoBehaviour
{
    private float speed = 0.1f;
    private float sensitivity = 5.0f;
    private float cameraZoomSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {
        // ProcessInput();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ProcessInput()
    {
        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            transform.position += transform.right * speed;
        }
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            transform.position -= transform.right * speed;
        }
        if (Input.GetKey("up") || Input.GetKey("w"))
        {
            transform.position += transform.forward * speed;
        }
        if (Input.GetKey("down") || Input.GetKey("s"))
        {
            transform.position -= transform.forward * speed;
        }
        if (Input.GetKey("e"))
        {
            transform.position += transform.up * speed;
        }
        if (Input.GetKey("q"))
        {
            transform.position -= transform.up * speed;
        }

        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");
        transform.RotateAround(transform.position, -Vector3.up, -rotateHorizontal * sensitivity);
        transform.RotateAround(Vector3.zero, transform.right, -rotateVertical * sensitivity);

        Camera.main.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;
    }
}
