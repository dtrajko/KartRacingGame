using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive drive;
    public float steeringSensitivity = 0.02f;
    public float brakingSensitivity = 1.19f;
    Vector3 target;
    Vector3 nextTarget;
    int currentWaypoint = 0;
    int nextWaypoint = 0;
    float totalDistanceToTarget;

    // NPCs can have different max acceleration or breaking values
    float accelRandOffset = 0.0f;
    float brakingRandOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWaypoint].transform.position;
        nextTarget = circuit.waypoints[currentWaypoint + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        // NPCs can have different max acceleration or breaking values
        accelRandOffset = Random.Range(-0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(target);
        Vector3 nextLocalTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(nextTarget);
        float distanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;

        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);

        float distanceFactor = distanceToTarget / totalDistanceToTarget;
        float speedFactor = drive.currentSpeed / drive.maxSpeed;
        float speedFactorWeight = 0.85f;

        float acceleration = 1.0f;
        float nextWaypointFactor = Mathf.Abs(nextTargetAngle) / 180.0f;
        float brakingValue = (speedFactor * speedFactorWeight - distanceFactor + nextWaypointFactor) * brakingSensitivity;
        float braking = Mathf.Lerp(-1.0f, 0.95f, brakingValue);

        if (distanceToTarget < 12.0f && drive.speedPercentage > 0.4f) {
            acceleration = Mathf.Lerp(0.0f, 0.8f, 0.8f - braking);
        }

        if (distanceToTarget < 8.0f) // threshold, make larger if car starts to circle waypoint
        {
            acceleration = Mathf.Lerp(0.0f, 1.0f, 1.0f - braking);

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

            Debug.Log("Waypoints: " + circuit.waypoints.Length + " CurrentWaypoint: " + currentWaypoint + " nextWaypoint: " + nextWaypoint);

            target = circuit.waypoints[currentWaypoint].transform.position;
            nextTarget = circuit.waypoints[nextWaypoint].transform.position;

            totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);
        }

        drive.Go(acceleration + accelRandOffset, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
