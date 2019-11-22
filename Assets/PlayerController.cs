using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive drive;

    float lastTimeMoving = 0.0f;
    Vector3 lastPosition;
    Quaternion lastRotation;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;
    }

    void ResetLayer()
    {
        drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("Default");
        this.GetComponent<Ghost>().enabled = false;
    }

    void Update()
    {
        float acceleration = Input.GetAxis("Vertical");
        float steering     = Input.GetAxis("Horizontal");
        float braking      = Input.GetAxis("Jump");

        RaycastHit hit;
        if (Physics.Raycast(drive.rigidBody.gameObject.transform.position, -Vector3.up, out hit, 10.0f))
        {
            if (hit.collider.gameObject.tag == "road")
            {
                lastPosition = drive.rigidBody.gameObject.transform.position;
                lastRotation = drive.rigidBody.gameObject.transform.rotation;

                if (drive.rigidBody.velocity.magnitude > 1 || !RaceMonitor.racing)
                {
                    lastTimeMoving = Time.time;
                }
            }
        }

        if (Time.time > lastTimeMoving + 4.0f)
        {
            drive.rigidBody.gameObject.transform.position = lastPosition;
            drive.rigidBody.gameObject.transform.rotation = lastRotation;
            drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("ReSpawn");
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if (!RaceMonitor.racing)
        {
            acceleration = 0.0f;
        }

        drive.Go(acceleration, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
