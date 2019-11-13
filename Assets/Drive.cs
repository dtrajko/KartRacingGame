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
    public AudioSource highAccel;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];

    public GameObject brakeLight;

    public Rigidbody rb;
    public float gearLength = 8;
    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    public float lowPitch = 1.0f;
    public float highPitch = 5.0f;
    public int numGears = 5;
    public float maxSpeed = 200;

    public float rpm;
    public int currentGear = 1;
    public float currentGearPerc;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
        brakeLight.SetActive(false);
    }

    void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
            Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5.0f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;

        if (currentGear > 1 && speedPercentage < downGearMax)
        {
            currentGear--;
        }
        if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
        {
            currentGear++;
        }

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

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

    void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        if (brake > 0)
        {
            brakeLight.SetActive(true);
        }
        else
        {
            brakeLight.SetActive(false);
        }

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
                    thrustTorque = 0.0f;
                    if (currentSpeed < maxSpeed)
                    {
                        thrustTorque = accel * torque;
                    }
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
                skidSmoke[i].transform.position = WheelColliders[i].transform.position -
                    WheelColliders[i].transform.up * WheelColliders[i].radius;
                skidSmoke[i].Emit(1);
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

        CalculateEngineSound();
    }
}
