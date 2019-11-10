using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider WC;
    public float torque = 200.0f;

    // Start is called before the first frame update
    void Start()
    {
        WC = this.GetComponent<WheelCollider>();
    }

    void Go(float accel)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        float thrustTorque = accel * torque;
        WC.motorTorque = thrustTorque;

        Quaternion quat;
        Vector3 position;
        WC.GetWorldPose(out position, out quat);
        // this.transform.position = position;
        // this.transform.rotation = quat;
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        Go(a);
    }
}
