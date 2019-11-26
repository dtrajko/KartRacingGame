﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController2 : AIController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        brakingSensitivity = 1.0f;
        steeringSensitivity = 0.01f;
        accelSensitivity = 1.0f;
        lookAhead = 24.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }

        ProgressTracker();

        Vector3 localTarget;
        float targetAngle;
        float brake = 0.0f;
        float accel = 1.0f;

        if (drive.rigidBody.velocity.magnitude > 1.0f)
        {
            lastTimeMoving = Time.time;
        }

        if (Time.time > lastTimeMoving + 4.0f)
        {
            if (checkpointManager == null)
            {
                checkpointManager = drive.rigidBody.GetComponent<CheckpointManager>();
            }

            Vector3 reSpawnPosition = checkpointManager.lastCP.transform.position +
                Vector3.up * 2 + // place the car 2m above the road
                Vector3.forward * 4 + // 4m forward
                new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)); // randomize the position around the waypoint

            drive.rigidBody.gameObject.transform.position = reSpawnPosition;
            drive.rigidBody.gameObject.transform.rotation = checkpointManager.lastCP.transform.rotation;
            tracker.transform.position = reSpawnPosition;

            lastTimeMoving = Time.time;

            drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("ReSpawn");
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        float speedFactor = drive.currentSpeed / drive.maxSpeed;

        if (Time.time < drive.rigidBody.GetComponent<AvoidDetector>().avoidTime)
        {
            localTarget = tracker.transform.right * drive.rigidBody.GetComponent<AvoidDetector>().avoidPath;
        }
        else
        {
            localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }

        // Slow down if someone is in front of you
        RaycastHit hit;
        Vector3 raycastDirection = (drive.rigidBody.gameObject.transform.forward + tracker.transform.forward).normalized;
        Vector3 raycastOrigin = drive.rigidBody.gameObject.transform.position + (-Vector3.up * 0.5f);
        bool isHit = Physics.Raycast(raycastOrigin, raycastDirection, out hit, 20.0f);
        Debug.DrawRay(raycastOrigin, raycastDirection * 20.0f, Color.green);
        if (isHit)
        {
            if (hit.collider.gameObject.tag == "car" && speedFactor > 0.2f)
            {
                accel = accel - accel * speedFactor;
                brake = brake + brake * speedFactor;
                localTarget = tracker.transform.right * drive.rigidBody.GetComponent<AvoidDetector>().avoidPath;
            }
        }

        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90.0f;

        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);

        if (corner > 10 && speedFactor > 0.1f)
        {
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);
        }

        if (corner > 20.0f && speedFactor > 0.2f)
        {
            accel = Mathf.Lerp(0, 1 * accelSensitivity, 1 - cornerFactor);
        }

        drive.Go(accel, steering, brake);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
