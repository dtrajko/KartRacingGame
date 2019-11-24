using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public float antiRoll = 5000.0f;
    public WheelCollider WC_FrontLeft;
    public WheelCollider WC_FrontRight;
    public WheelCollider WC_BackLeft;
    public WheelCollider WC_BackRight;
    Rigidbody rigidBody;
    public GameObject centerOfMass;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass.transform.localPosition;
    }

    void GroundWheels(WheelCollider WC_Left, WheelCollider WC_Right)
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        bool groundedLeft = WC_Left.GetGroundHit(out hit);
        if (groundedLeft)
        {
            travelLeft = (-WC_Left.transform.InverseTransformPoint(hit.point).y - WC_Left.radius) / WC_Left.suspensionDistance;
        }

        bool groundedRight = WC_Right.GetGroundHit(out hit);
        if (groundedRight)
        {
            travelRight = (-WC_Right.transform.InverseTransformPoint(hit.point).y - WC_Right.radius) / WC_Right.suspensionDistance;
        }

        float antiRollForce = (travelLeft - travelRight) * antiRoll;

        if (groundedLeft)
        {
            rigidBody.AddForceAtPosition(WC_Left.transform.up * -antiRollForce, WC_Left.transform.position);
        }

        if (groundedRight)
        {
            rigidBody.AddForceAtPosition(WC_Right.transform.up * antiRollForce, WC_Right.transform.position);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundWheels(WC_FrontLeft, WC_FrontRight);
        GroundWheels(WC_BackLeft, WC_BackRight);
    }
}
