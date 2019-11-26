using System.Collections;
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
    protected float lastTimeMoving = 0.0f;
    protected CheckpointManager checkpointManager;

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

        this.GetComponent<Ghost>().enabled = false;
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

    void ResetLayer()
    {
        drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("Default");
        this.GetComponent<Ghost>().enabled = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }

        ProgressTracker();

        Vector3 localTarget;
        float targetAngle;
        float braking = 0.0f;
        float acceleration = 1.0f;
        float speedFactorWeight = 2.8f;
        float targetAngleWeight = 3.6f;
        float rayLength = 20.0f;

        RaycastHit hit;
        Vector3 raycastOrigin = drive.rigidBody.gameObject.transform.position + drive.rigidBody.gameObject.transform.forward * 2 +
            (-drive.rigidBody.gameObject.transform.up * 0.8f);
        Vector3 raycastDirection = drive.rigidBody.gameObject.transform.forward;
        Vector3 avoidDirection = raycastDirection;
        bool avoidObstacleMode = false;
        bool isHit = Physics.Raycast(raycastOrigin, raycastDirection, out hit, rayLength);
        if (isHit)
        {
            if (hit.collider.gameObject.tag == "car")
            {
                avoidObstacleMode = true;
                // Debug.DrawRay(raycastOrigin, raycastDirection * rayLength, Color.green);
                // Debug.DrawRay(raycastOrigin, avoidDirection * rayLength, Color.red);
            }
        }

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
                Vector3.up * 3 + // place the car 2m above the road
                Vector3.forward * 6 + // 6m forward
                new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3)); // randomize the position around the waypoint

            drive.rigidBody.gameObject.transform.position = reSpawnPosition;
            drive.rigidBody.gameObject.transform.rotation = checkpointManager.lastCP.transform.rotation;
            tracker.transform.position = reSpawnPosition;

            lastTimeMoving = Time.time;

            drive.rigidBody.gameObject.layer = LayerMask.NameToLayer("ReSpawn");
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if (Time.time < drive.rigidBody.GetComponent<AvoidDetector>().avoidTime)
        {
            localTarget = tracker.transform.right * drive.rigidBody.GetComponent<AvoidDetector>().avoidPath;
            // Debug.DrawRay(raycastOrigin, localTarget * rayLength, Color.blue);
        }
        else
        {
            localTarget = drive.rigidBody.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }

        if (avoidObstacleMode)
        {
            localTarget = drive.rigidBody.gameObject.transform.forward - tracker.transform.right;
        }

        Vector3 vectorDirection = tracker.transform.position - drive.rigidBody.gameObject.transform.position;
        Debug.DrawRay(raycastOrigin, vectorDirection.normalized * rayLength, Color.green);

        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steering = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(drive.currentSpeed);

        float speedFactor = (drive.currentSpeed / drive.maxSpeed) * speedFactorWeight;
        float targetAngleFactor = (Mathf.Abs(targetAngle) / 90.0f) * targetAngleWeight;

        // Debug.Log("BRAKING - SPEED: "+ (int)(Mathf.Lerp(0, 1, speedFactor) * 100) +
        //     "%, ANGLE: " + (int)(Mathf.Lerp(0, 1, targetAngleFactor) * 100) + 
        //     "% TOTAL: " + (int)(Mathf.Lerp(0, 1, speedFactor * targetAngleFactor) * 100) + "%");

        braking = Mathf.Lerp(-1.0f, 1.0f, speedFactor * targetAngleFactor);
        acceleration = Mathf.Lerp(accelSensitivity, 1.0f, 1.0f + accelSensitivity - braking);

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
