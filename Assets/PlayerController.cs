using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Drive drive;

    // Start is called before the first frame update
    void Start()
    {
        drive = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;
    }

    void Update()
    {
        if (!RaceMonitor.racing)
        {
            return;
        }

        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");

        drive.Go(a, s, b);
        drive.CheckForSkid();
        drive.CalculateEngineSound();
    }
}
