using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidDetector : MonoBehaviour
{
    public float avoidPath = 0.0f;
    public float avoidTime = 0.0f;
    public float wanderDistance = 4.0f; // avoiding distance
    public float avoidLength = 0.2f;

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag != "car")
        {
            return;
        }

        avoidTime = 0;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag != "car")
        {
            return;
        }

        Rigidbody otherCar = other.rigidbody;
        avoidTime = Time.time + avoidLength;

        Vector3 otherCarLocalTarget = transform.InverseTransformPoint(otherCar.gameObject.transform.position);
        float otherCarAngle = Mathf.Atan2(otherCarLocalTarget.x, otherCarLocalTarget.z);
        avoidPath = wanderDistance * -Mathf.Sin(otherCarAngle);
    }
}
