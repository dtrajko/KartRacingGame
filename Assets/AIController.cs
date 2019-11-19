﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    public float brakingSensitivity = 1.0f;
    public float steeringSensitivity = 0.01f;
    public float accelSensitivity = 1.0f;
    public float lookAhead = 24.0f;

    protected Drive drive;
    protected Vector3 target;
    protected Vector3 nextTarget;
    protected int currentWaypoint = 0;
    protected int nextWaypoint = 0;
    protected GameObject tracker;
    protected int currentTrackerWP = 0;
    protected float trackerPrevHeight = 0.0f;

    private float totalDistanceToTarget;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        drive = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWaypoint].transform.position;
        nextTarget = circuit.waypoints[currentWaypoint + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.gameObject.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = drive.rigidBody.transform.position;
        tracker.transform.rotation = drive.rigidBody.transform.rotation;
    }

    protected void ProgressTracker()
    {
        Debug.DrawLine(drive.rigidBody.transform.position, tracker.transform.position);

        if (Vector3.Distance(drive.rigidBody.gameObject.transform.position, tracker.transform.position) > lookAhead)
        {
            return;
        }

        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1.0f); // speed of tracker

        if (Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1.0f)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Length)
            {
                currentTrackerWP = 0;
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        ProgressTracker();

        Vector3 localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);

        float speedFactorWeight = 2.8f;
        float targetAngleWeight = 3.6f;
        float speedFactor = (drive.currentSpeed / drive.maxSpeed) * speedFactorWeight;
        float targetAngleFactor = (Mathf.Abs(targetAngle) / 90.0f) * targetAngleWeight;

        // Debug.Log("BRAKING - SPEED: "+ (int)(Mathf.Lerp(0, 1, speedFactor) * 100) +
        //     "%, ANGLE: " + (int)(Mathf.Lerp(0, 1, targetAngleFactor) * 100) + 
        //     "% TOTAL: " + (int)(Mathf.Lerp(0, 1, speedFactor * targetAngleFactor) * 100) + "%");

        float braking = Mathf.Lerp(-1.0f, 1.0f, speedFactor * targetAngleFactor);
        float acceleration = Mathf.Lerp(accelSensitivity, 1.0f, 1.0f + accelSensitivity - braking);

        if (drive.currentSpeed < 4.0f || drive.IsClimbing)
        {
            braking = 0.0f;
            acceleration = 1.0f;
        }

        // Debug.Log("DRIVE.GO - ACCEL: " + (int)(acceleration * 100) +
        //     "%, BRAKE: " + (int)(Mathf.Clamp(braking, 0, 1) * 100) + "%");

        drive.Go(acceleration, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
