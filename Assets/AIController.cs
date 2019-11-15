using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive drive;
    public float steeringSensitivity = 0.01f;
    Vector3 target;
    int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWaypoint].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(target);
        float distanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);
        float acceleration = 0.9f;
        float braking = 0.0f;

        if (distanceToTarget < 12.0 && drive.speedPercentage > 0.6f) {
            acceleration = 0.5f;
            braking = 0.8f;
        }

        drive.Go(acceleration, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();

        if (distanceToTarget < 8.0f) // threshold, make larger if car starts to circle waypoint
        {
            acceleration = 0.9f;
            braking = 0.0f;

            currentWaypoint++;
            if (currentWaypoint >= circuit.waypoints.Length)
            {
                currentWaypoint = 0;
            }
            target = circuit.waypoints[currentWaypoint].transform.position;
            Debug.Log("Current waypoint:" + currentWaypoint);
        }

    }
}
