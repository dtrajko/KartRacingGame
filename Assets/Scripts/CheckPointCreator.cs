using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 

[ExecuteInEditMode]
public class CheckPointCreator : MonoBehaviour
{
    public Circuit circuit;
    GameObject tracker;
    public GameObject firstCP;
    public GameObject cpPrefab;
    public float cpDistance = 2;
    int currentTrackerWP = 0;
    bool go = false;
    int number = 1;
    float lastCPTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CreateCheckpoints()
    {
        tracker = GameObject.Find("CPPLACER");
        if (tracker == null)
        {
            tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            DestroyImmediate(tracker.GetComponent<Collider>());
        }
        tracker.transform.position = firstCP.transform.position;
        tracker.gameObject.name = "CPPLACER";
        lastCPTime = Time.time + cpDistance;
        currentTrackerWP = 0;
        number = 1;
        go = true;
    }

    void PlaceCheckPoint()
    {
        GameObject cp = Instantiate(cpPrefab);
        cp.transform.position = tracker.transform.position;
        cp.transform.rotation = tracker.transform.rotation;
        cp.transform.parent = this.transform;
        cp.gameObject.name = "" + number;
        number++;
    }

    // Update is called once per frame
    void Update()
    {
        if (!go) return;

        Quaternion rotation = Quaternion.LookRotation(circuit.waypoints[currentTrackerWP].transform.position -
                            tracker.transform.position);
        tracker.transform.rotation = Quaternion.Slerp(tracker.transform.rotation, rotation, Time.deltaTime * 2);

        tracker.transform.Translate(0, 0, 1.0f); 

        if (Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Length)
                go = false; //we've reached the end
        }

        if (lastCPTime < Time.time)
        {
            PlaceCheckPoint();
            lastCPTime = Time.time + cpDistance;
        }
        EditorApplication.QueuePlayerLoopUpdate();
    }
}
#endif