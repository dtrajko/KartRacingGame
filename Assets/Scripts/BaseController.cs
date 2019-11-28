using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Drive drive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected bool withinSceneBoundaries()
    {
        if (drive.rigidBody.gameObject.transform.position.y < -10.0f)
        {
            return false;
        }
        return false;
    }
}
