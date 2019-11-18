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
    float lookAhead = 16.0f;

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

        // Vector3 localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(target);
        Vector3 localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        Vector3 nextLocalTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(nextTarget);
        float distanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;

        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);

        float distanceFactor = distanceToTarget / totalDistanceToTarget;
        float speedFactor = drive.currentSpeed / drive.maxSpeed;
        float speedFactorWeight = 0.85f;

        float acceleration = Mathf.Lerp(accelSensitivity, 1.0f, distanceFactor);
        float nextWaypointFactor = Mathf.Abs(nextTargetAngle) / 180.0f;
        float brakingValue = (speedFactor * speedFactorWeight - distanceFactor + nextWaypointFactor) * brakingSensitivity;
        float braking = Mathf.Lerp(-1.0f, 0.95f, brakingValue);

        if (distanceToTarget < 12.0f && drive.speedPercentage > 0.4f) {
            acceleration = Mathf.Lerp(accelSensitivity, 0.8f, 0.8f - braking);
        }

        if (distanceToTarget < 8.0f) // threshold, make larger if car starts to circle waypoint
        {
            acceleration = Mathf.Lerp(accelSensitivity, 1.0f, 1.0f - braking);

            currentWaypoint++;
            if (currentWaypoint >= circuit.waypoints.Length)
            {
                currentWaypoint = 0;
            }
            nextWaypoint = currentWaypoint + 1;
            if (nextWaypoint >= circuit.waypoints.Length)
            {
                nextWaypoint = 0;
            }

            target = circuit.waypoints[currentWaypoint].transform.position;
            nextTarget = circuit.waypoints[nextWaypoint].transform.position;

            totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

            if (nextTarget.y > 10.0f)
            {
                isJump = true;
            }
            else
            {
                isJump = false;
            }
        }

        if (Mathf.Abs(nextTargetAngle) > 20.0f)
        {
            // braking += 0.8f;
            // acceleration -= 0.8f;
        }

        if (isJump)
        {
            acceleration = 1.0f;
            braking = 0.0f;
        }

        drive.Go(acceleration + accelRandOffset, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
