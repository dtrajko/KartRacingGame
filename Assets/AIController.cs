using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive drive;
    public float accelSensitivity = 0.3f;
    public float steeringSensitivity = 0.02f;
    public float brakingSensitivity = 1.19f;
    Vector3 target;
    Vector3 nextTarget;
    int currentWaypoint = 0;
    int nextWaypoint = 0;
    float totalDistanceToTarget;
    bool isJump = false;

    // NPCs can have different max acceleration or breaking values
    float accelRandOffset = 0.0f;
    float brakingRandOffset = 0.0f;

    GameObject tracker;
    int currentTrackerWP = 0;
    float lookAhead = 20.0f;
    float trackerPrevHeight = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWaypoint].transform.position;
        nextTarget = circuit.waypoints[currentWaypoint + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        // NPCs can have different max acceleration or breaking values
        accelRandOffset = Random.Range(-0.2f, 0.2f);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.gameObject.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = drive.rigidBody.transform.position;
        tracker.transform.rotation = drive.rigidBody.transform.rotation;
    }

    void ProgressTracker()
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
    void Update()
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

        drive.Go(acceleration, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
