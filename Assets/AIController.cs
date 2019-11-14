using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    Drive ds;

    // Start is called before the first frame update
    void Start()
    {
        ds = this.GetComponent<Drive>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        ds.Go(a, s, b);

        ds.CheckForSkid();

        ds.CalculateEngineSound();
    }
}
