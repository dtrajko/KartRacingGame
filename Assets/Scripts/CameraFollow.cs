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
        ThirdPersonPlayer = 1,
        AerialMode = 2
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
        if (CameraPerspective == PerspectiveModes.ThirdPersonPlayer)
        {
            // Third person
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(0.0f, 3.0f, -10.0f),
                cameraSwitchSpeed * Time.deltaTime);
            cameraObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (CameraPerspective == PerspectiveModes.FirstPersonPlayer)
        {
            // First person
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(-0.4f, 1.0f, 0.6f),
                cameraSwitchSpeed * Time.deltaTime);
            cameraObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (CameraPerspective == PerspectiveModes.AerialMode)
        {
            // Aerial mode
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(0.0f, 30.0f, -40.0f),
                cameraSwitchSpeed * Time.deltaTime);
            cameraObject.transform.localRotation = Quaternion.Euler(20.0f, 0, 0);

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            switch (CameraPerspective)
            {
                case PerspectiveModes.ThirdPersonPlayer:
                    CameraPerspective = PerspectiveModes.FirstPersonPlayer;
                    break;
                case PerspectiveModes.FirstPersonPlayer:
                    CameraPerspective = PerspectiveModes.AerialMode;
                    break;
                case PerspectiveModes.AerialMode:
                    CameraPerspective = PerspectiveModes.ThirdPersonPlayer;
                    break;
                default:
                    CameraPerspective = PerspectiveModes.ThirdPersonPlayer;
                    break;
            }

            PlayerPrefs.SetInt("CameraPerspective", (int)CameraPerspective);
        }
    }
}
