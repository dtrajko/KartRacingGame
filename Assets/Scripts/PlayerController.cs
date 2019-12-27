using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : BaseController
{
    float lastTimeMoving = 0.0f;
    Vector3 lastPosition;
    Quaternion lastRotation;
    CheckpointManager checkpointManager;
    float finishSteer;
    public bool racing = false;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;

        lastPosition = drive.rigidBody.gameObject.transform.position;
        lastRotation = drive.rigidBody.gameObject.transform.rotation;

        finishSteer = Random.Range(-0.2f, 0.2f);
    }

    void ResetLayer()
    {
        drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("Default");
        this.GetComponent<Ghost>().enabled = false;
    }

    void FixedUpdate()
    {
        float acceleration = CrossPlatformInputManager.GetAxis("Vertical");
        float steering     = CrossPlatformInputManager.GetAxis("Horizontal");
        float braking      = CrossPlatformInputManager.GetAxis("Jump");

        if (checkpointManager == null)
        {
            checkpointManager = drive.rigidBody.GetComponent<CheckpointManager>();
        }

        // Game Over condition
        if (checkpointManager != null && FindObjectOfType<RaceMonitor>() != null &&
            checkpointManager.lap == FindObjectOfType<RaceMonitor>().totalLaps + 1)
        {
            drive.highAccel.Stop();
            drive.Go(0.0f, steering, 1.0f);
            return;
        }

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

        if (Time.time > lastTimeMoving + 4.0f || withinSceneBoundaries())
        {
            if (checkpointManager.lastCP != null)
            { 
                Vector3 reSpawnPosition = checkpointManager.lastCP.transform.position +
                    Vector3.up * 3 + // place the car 2m above the road
                    Vector3.forward * 6 + // 6m forward
                    new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3)); // randomize the position around the waypoint

                drive.rigidBody.gameObject.transform.position = reSpawnPosition;
                drive.rigidBody.gameObject.transform.rotation = checkpointManager.lastCP.transform.rotation;
                drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("ReSpawn");
                this.GetComponent<Ghost>().enabled = true;
                Invoke("ResetLayer", 3);            
            }
        }

        if (!RaceMonitor.racing && !racing)
        {
            acceleration = 0.0f;
        }

        drive.Go(acceleration, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
