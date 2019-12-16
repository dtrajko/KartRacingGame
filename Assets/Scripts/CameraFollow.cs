using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class CameraFollow : MonoBehaviour
{
    public Camera cameraObject;

    float cameraSwitchSpeed = 10.0f;

    Transform[] targets;
    public static Transform playerCar;
    public RenderTexture frontCamView;
    int switchTargetIndex = 0;

    float inputAxisTimer;
    float inputAxisCooldown = 0.5f;
    bool inputAxisUnlocked;

    enum PerspectiveModes
    {
        ThirdPersonPlayer = 1,
        FirstPersonIndoor = 2,
        FirstPersonPlayer = 3,
        AerialMode = 4
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

        inputAxisTimer = 0.0f;
        inputAxisUnlocked = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targets == null)
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
            targets = new Transform[cars.Length - 1];
            int targetIndex = 0;
            for (int i = 0; i < cars.Length; i++)
            {
                Camera remoteCamera = cars[i].transform.Find("FrontCamera").gameObject.GetComponent<Camera>();
                if (remoteCamera.tag != "MainCamera")
                { 
                    targets[targetIndex] = cars[i].transform;
                    targetIndex++;
                    targets[switchTargetIndex].Find("FrontCamera").gameObject.GetComponent<Camera>().targetTexture = frontCamView;
                }
            }
        }

        if (CameraPerspective == PerspectiveModes.ThirdPersonPlayer)
        {
            // Third person
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(0.0f, 3.0f, -11.0f),
                cameraSwitchSpeed * Time.deltaTime);
            cameraObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (CameraPerspective == PerspectiveModes.FirstPersonIndoor)
        {
            // First person by Veljko
            cameraObject.transform.localPosition =
                Vector3.Lerp(cameraObject.transform.localPosition, new Vector3(-0.4f, 0.52f, -0.3f),
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
        inputAxisTimer += Time.deltaTime;
        if (inputAxisTimer > inputAxisCooldown)
        {
            inputAxisUnlocked = true;
            inputAxisTimer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.C) ||
            (CrossPlatformInputManager.GetAxisRaw("Camera") == 1.0f && inputAxisUnlocked))
        {
            switch (CameraPerspective)
            {
                case PerspectiveModes.ThirdPersonPlayer:
                    CameraPerspective = PerspectiveModes.FirstPersonIndoor;
                    break;
                case PerspectiveModes.FirstPersonIndoor:
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

            inputAxisUnlocked = false;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            targets[switchTargetIndex].Find("FrontCamera").gameObject.GetComponent<Camera>().targetTexture = null;
            switchTargetIndex++;
            if (switchTargetIndex > targets.Length - 1)
            {
                switchTargetIndex = 0;
            }
            targets[switchTargetIndex].Find("FrontCamera").gameObject.GetComponent<Camera>().targetTexture = frontCamView;
        }
    }
}
