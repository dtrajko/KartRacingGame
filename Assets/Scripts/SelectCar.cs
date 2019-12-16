using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SelectCar : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] levels;
    float heightLevel1;
    float heightLevel2;
    float heightLevel3;
    float cameraHeight = 3.0f;
    Quaternion lookDir;
    int currentCar = 0;
    float cameraSpeed = 4.0f;

    float inputAxisTimer;
    float inputAxisCooldown = 0.6f;
    bool inputAxisUnlocked;

    public AudioSource buttonSound;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1.0f;

        heightLevel1 = levels[0].transform.position.y;
        heightLevel2 = levels[1].transform.position.y;
        heightLevel3 = levels[2].transform.position.y;

        if (PlayerPrefs.HasKey("PlayerCar"))
        {
            currentCar = PlayerPrefs.GetInt("PlayerCar");
        }

        if (currentCar > cars.Length - 1)
        {
            currentCar = 0;
        }

        this.transform.position = new Vector3(0.0f, cameraHeight, 0.0f);
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

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || 
            (CrossPlatformInputManager.GetAxisRaw("Horizontal") == 1.0f && inputAxisUnlocked))
        {
            SelectCarNext();
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            (CrossPlatformInputManager.GetAxisRaw("Horizontal") == -1.0f && inputAxisUnlocked))
        {
            SelectCarPrevious();
        }

        AdjustCamera();
    }

    public void SelectCarNext()
    {
        currentCar++;
        if (currentCar > cars.Length - 1)
        {
            currentCar = 0;
        }

        inputAxisUnlocked = false;
        buttonSound.Play();
        PlayerPrefs.SetInt("PlayerCar", currentCar);
    }

    public void SelectCarPrevious()
    {
        currentCar--;
        if (currentCar < 0)
        {
            currentCar = cars.Length - 1;
        }

        inputAxisUnlocked = false;
        buttonSound.Play();
        PlayerPrefs.SetInt("PlayerCar", currentCar);
    }

    private void AdjustCamera()
    {
        Vector3 newPosition = this.transform.position;
        if (currentCar >= 0 && currentCar <= 3)
        {
            newPosition = new Vector3(
                this.transform.position.x,
                heightLevel1 + cameraHeight,
                this.transform.position.z);
        }
        else if(currentCar >= 4 && currentCar <= 7)
        {
            newPosition = new Vector3(
                this.transform.position.x,
                heightLevel2 + cameraHeight,
                this.transform.position.z);
        }
        else if (currentCar >= 8 && currentCar <= 11)
        {
            newPosition = new Vector3(
                this.transform.position.x,
                heightLevel3 + cameraHeight,
                this.transform.position.z);
        }

        // this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.deltaTime * cameraSpeed * 2.0f);
        this.transform.position = newPosition;

        lookDir = Quaternion.LookRotation(cars[currentCar].transform.position - new Vector3(0, -1.0f, 0) - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookDir, Time.deltaTime * cameraSpeed);
        // Debug.Log("Rotation: " + this.transform.rotation + " lookDir: " + lookDir + " time offset: " + Time.deltaTime * cameraSpeed);

    }
}
