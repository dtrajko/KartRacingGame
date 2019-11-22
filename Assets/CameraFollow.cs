using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera cameraObject;

    float cameraSwitchSpeed = 10.0f;

    enum PerspectiveModes
    { 
        FirstPersonPlayer = -1,
        ThirdPersonPlayer = 1
    }

    PerspectiveModes CameraPerspective = PerspectiveModes.ThirdPersonPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("CameraPerspective"))
        { 
            CameraPerspective = (PerspectiveModes)PlayerPrefs.GetInt("CameraPerspective");
        }

        if (cameraObject == null)
        {
            cameraObject = GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (CameraPerspective == PerspectiveModes.FirstPersonPlayer)
        {
            // First person
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(-0.4f, 1.0f, 0.5f), 
                cameraSwitchSpeed * Time.deltaTime);
        }
        else
        {
            // Third person
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(0.0f, 3.0f, -9.0f),
                cameraSwitchSpeed * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            switch (CameraPerspective)
            {
                case PerspectiveModes.FirstPersonPlayer:
                    CameraPerspective = PerspectiveModes.ThirdPersonPlayer;
                    break;
                case PerspectiveModes.ThirdPersonPlayer:
                    CameraPerspective = PerspectiveModes.FirstPersonPlayer;
                    break;
                default:
                    CameraPerspective = PerspectiveModes.ThirdPersonPlayer;
                    break;
            }

            PlayerPrefs.SetInt("CameraPerspective", (int)CameraPerspective);
        }
    }
}
