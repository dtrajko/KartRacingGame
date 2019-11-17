using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive drive;
    public float steeringSensitivity = 0.02f;
    Vector3 target;
    int currentWaypoint = 0;
    float totalDistanceToTarget;

    // NPCs can have different max acceleration or breaking values
    float accelRandOffset = 0.0f;
    float brakingRandOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWaypoint].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        // NPCs can have different max acceleration or breaking values
        accelRandOffset = Random.Range(-0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(target);
        float distanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);

        float distanceFactor = distanceToTarget / totalDistanceToTarget;

        float acceleration = 1.0f;
        float braking = Mathf.Lerp(-1.0f, 1.0f, 0.4f - distanceFactor);

        if (distanceToTarget < 12.0f && drive.speedPercentage > 0.4f) {
            acceleration = 0.2f;
            braking = 0.8f;
        }

        if (distanceToTarget < 4.0f) // threshold, make larger if car starts to circle waypoint
        {
            acceleration = 1.0f;
            braking = 0.0f;

            currentWaypoint++;
            if (currentWaypoint >= circuit.waypoints.Length)
            {
                currentWaypoint = 0;
            }
            target = circuit.waypoints[currentWaypoint].transform.position;
            totalDistanceToTarget = Vector3.Distance(target, drive.rigidBody.gameObject.transform.position);

        }

        Debug.Log("AIControler.braking:" + braking);

        drive.Go(acceleration + accelRandOffset, steering, braking);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
