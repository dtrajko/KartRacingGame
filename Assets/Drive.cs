using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider[] WheelColliders;
    public GameObject[] Wheels;
    public float torque = 300.0f;
    public float maxSteerAngle = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Go(float accel, float steer)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        float thrustTorque = accel * torque;

        Quaternion quat;
        Vector3 position;

        for (int i = 0; i < 4; i++)
        {
            WheelColliders[i].motorTorque = thrustTorque;

            float wheelSteer = i < 2 ? steer : 0.0f;
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
        Go(a, s);
    }
}
