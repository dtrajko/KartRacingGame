using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Rigidbody rigidBody;
    public float gearLength = 8;
    public float currentSpeed { get { return rigidBody.velocity.magnitude * gearLength; } }
    public float lowPitch = 1.5f;
    public float highPitch = 5.0f;
    public int numGears = 5;
    public float maxSpeed = 200;
    public float speedPercentage;

    public float rpm;
    public int currentGear = 1;
    public float currentGearPerc;

    public string playerName;
    public string networkName;
    public GameObject playerNamePrefab;
    public MeshRenderer vehicleMesh;

    Vector3[] wheelRelativePositions;

    private void Awake()
    {
        wheelRelativePositions = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            wheelRelativePositions[i] = Wheels[i].transform.localPosition;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }

        brakeLight.SetActive(false);

        SetupPlayerName();
    }

    private void SetupPlayerName()
    {
        if (playerNamePrefab == null)
        {
            return;
        }

        GameObject playerNameGO = Instantiate(playerNamePrefab);
        playerNameGO.GetComponent<NameUIController>().target = rigidBody.gameObject.transform;

        if (this.GetComponent<AIController>().enabled)
        {
            if (networkName != "")
            {
                playerNameGO.GetComponent<Text>().text = networkName;
            }
        }
        else if (GetComponentInParent<PlayerController>().enabled)
        {
            if (PlayerPrefs.HasKey("PlayerName"))
            { 
                playerNameGO.GetComponent<Text>().text = PlayerPrefs.GetString("PlayerName");
            }
        }
        else
        {
            playerNameGO.GetComponent<Text>().text = playerName;   
        }
        playerNameGO.GetComponent<NameUIController>().carRenderer = vehicleMesh;
    }

    public void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
            Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5.0f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;

        if (currentGear > 0 && speedPercentage < downGearMax)
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

    public void Go(float acceleration, float steering, float braking)
    {
        acceleration = Mathf.Clamp(acceleration, -1, 1);
        steering = Mathf.Clamp(steering, -1, 1) * maxSteerAngle;
        braking = Mathf.Clamp(braking, 0, 1) * maxBrakeTorque;

        if (braking > 0)
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
                    wheelSteer = steering;
                    thrustTorque = 0.0f;
                    break;
                // back wheels
                case 2:
                case 3:
                    wheelSteer = 0.0f;
                    thrustTorque = 0.0f;
                    if (currentSpeed < maxSpeed)
                    {
                        thrustTorque = acceleration * torque;
                    }
                    break;
                default:
                    wheelSteer = 0.0f;
                    thrustTorque = 0.0f;
                    break;
            }

            WheelColliders[i].motorTorque = thrustTorque;
            WheelColliders[i].brakeTorque = braking;

            WheelColliders[i].steerAngle = wheelSteer;

            WheelColliders[i].GetWorldPose(out position, out quat);
            // Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quat;

            Wheels[i].transform.localPosition = wheelRelativePositions[i];
        }
    }

    public void CheckForSkid()
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

    public bool IsClimbing
    {
        get
        {
            float axleFront = (WheelColliders[0].transform.position.y + WheelColliders[1].transform.position.y) / 2;
            float axleRead = (WheelColliders[2].transform.position.y + WheelColliders[3].transform.position.y) / 2;
            // Debug.Log("(axleFront - axleRead): " + (axleFront - axleRead));
            return (axleFront - axleRead) > 0.1f;
        }
    }
}
