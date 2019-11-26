using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int lap = 0;
    public int checkPoint = -1;
    public float timeEntered = 0;
    public int checkPointCount;
    int nextCheckPoint;
    public GameObject lastCP;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("checkpoint");
        checkPointCount = checkpoints.Length;
        foreach (GameObject checkpoint in checkpoints)
        {
            if (checkpoint.name == "0")
            {
                lastCP = checkpoint;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "checkpoint")
        {
            int thisCPNumber = int.Parse(other.gameObject.name);
            if (thisCPNumber == nextCheckPoint)
            {
                lastCP = other.gameObject;
                checkPoint = thisCPNumber;
                timeEntered = Time.time;
                if (checkPoint == 0)
                {
                    lap++;
                }

                nextCheckPoint++;
                if (nextCheckPoint >= checkPointCount)
                {
                    nextCheckPoint = 0;
                }
            }
        }
    }
}
