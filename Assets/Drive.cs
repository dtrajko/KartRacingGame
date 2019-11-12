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

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");
        Go(a, s, b);
    }
}
