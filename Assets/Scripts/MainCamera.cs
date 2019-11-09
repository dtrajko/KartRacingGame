using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private float speed = 0.1f;
    private float speedRotation = 5.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
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

        float angleRotationY = Input.GetAxis("Mouse X") * speedRotation;
        transform.Rotate(new Vector3(0, 1, 0), angleRotationY);
        // float angleRotationX = -Input.GetAxis("Mouse Y") * speedRotation;
        // transform.Rotate(new Vector3(1, 0, 0), angleRotationX);
    }
}
