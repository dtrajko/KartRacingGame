using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider[] WheelColliders;
    public GameObject[] Wheels;
    public float torque = 500.0f;
    public float maxSteerAngle = 35.0f;
    public float maxBrakeTorque = 2000.0f;

    public AudioSource skidSound;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);
        }

        skidTrails[i].parent = WheelColliders[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WheelColliders[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            return;
        }

        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        float thrustTorque;
        float wheelSteer;
        Quaternion quat;
        Vector3 position;

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                // front wheels
                case 0:
                case 1:
                    wheelSteer = steer;
                    thrustTorque = 0.0f;
                    break;
                // back wheels
                case 2:
                case 3:
                    wheelSteer = 0.0f;
                    thrustTorque = accel * torque;
                    break;
                default:
                    wheelSteer = 0.0f;
                    thrustTorque = 0.0f;
                    break;
            }

            WheelColliders[i].motorTorque = thrustTorque;
            WheelColliders[i].brakeTorque = brake;

            WheelColliders[i].steerAngle = wheelSteer;

            WheelColliders[i].GetWorldPose(out position, out quat);
            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quat;        
        }
    }

    void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WheelColliders[i].GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= 0.8f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.8f)
            {
                numSkidding++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                // StartSkidTrail(i);
            }
            else
            {
                // EndSkidTrail(i);
            }
        }

        if (numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        Go(a, s, b);

        CheckForSkid();
    }
}
