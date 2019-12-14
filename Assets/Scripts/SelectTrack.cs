using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SelectTrack : MonoBehaviour
{
    public GameObject[] tracks;
    Quaternion lookDir;
    int currentTrack = 0;
    float cameraSpeed = 4.0f;
    float trackWidth = 12.0f;

    float inputAxisTimer;
    float inputAxisCooldown = 0.6f;
    bool inputAxisUnlocked;

    public AudioSource buttonSound;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1.0f;

        if (PlayerPrefs.HasKey("PlayerTrack"))
        {
            currentTrack = PlayerPrefs.GetInt("PlayerTrack");
        }

        this.transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        AdjustCamera();

        inputAxisTimer = 0.0f;
        inputAxisUnlocked = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        inputAxisTimer += Time.deltaTime;
        if (inputAxisTimer > inputAxisCooldown)
        {
            inputAxisUnlocked = true;
            inputAxisTimer = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SelectTrackNext();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SelectTrackPrevious();
        }
    }

    private void AdjustCamera()
    {
        Vector3 newPosition = this.transform.position;
        newPosition = new Vector3(
                currentTrack * trackWidth,
                this.transform.position.y,
                this.transform.position.z);

        this.transform.position = newPosition;
    }

    public void SelectTrackNext()
    {
        currentTrack++;
        if (currentTrack > tracks.Length - 1)
        {
            currentTrack = 0;
        }

        inputAxisUnlocked = false;
        buttonSound.Play();
        PlayerPrefs.SetInt("PlayerTrack", currentTrack);
        AdjustCamera();
    }

    public void SelectTrackPrevious()
    {
        currentTrack--;
        if (currentTrack < 0)
        {
            currentTrack = tracks.Length - 1;
        }

        inputAxisUnlocked = false;
        buttonSound.Play();
        PlayerPrefs.SetInt("PlayerTrack", currentTrack);
        AdjustCamera();
    }
}
