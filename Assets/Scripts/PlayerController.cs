using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive drive;

    float lastTimeMoving = 0.0f;
    Vector3 lastPosition;
    Quaternion lastRotation;

    CheckpointManager checkpointManager;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;

        lastPosition = drive.rigidBody.gameObject.transform.position;
        lastRotation = drive.rigidBody.gameObject.transform.rotation;
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
            if (checkpointManager == null)
            {
                checkpointManager = drive.rigidBody.GetComponent<CheckpointManager>();
            }

            Vector3 reSpawnPosition = checkpointManager.lastCP.transform.position +
                Vector3.up * 2 + // place the car 2m above the road
                Vector3.forward * 4 + // 6m forward
                new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)); // randomize the position around the waypoint

            drive.rigidBody.gameObject.transform.position = reSpawnPosition;
            drive.rigidBody.gameObject.transform.rotation = checkpointManager.lastCP.transform.rotation;
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
