using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCar : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] levels;
    float heightLevel1;
    float heightLevel2;
    float cameraHeight = 3.0f;
    Quaternion lookDir;
    int currentCar = 0;
    float cameraSpeed = 4.0f;

    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1.0f;

        heightLevel1 = levels[0].transform.position.y;
        heightLevel2 = levels[1].transform.position.y;

        if (PlayerPrefs.HasKey("PlayerCar"))
        {
            currentCar = PlayerPrefs.GetInt("PlayerCar");
        }

        this.transform.position = new Vector3(0.0f, cameraHeight, 0.0f);
        this.transform.rotation = Quaternion.Euler(18.0f, 0.0f, 0.0f);

        // this.transform.LookAt(cars[currentCar].transform.position);

        AdjustCamera();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCar++;
            if (currentCar > cars.Length - 1)
            {
                currentCar = 0;
            }
            // Debug.Log("CurrentCar: " + currentCar + " Total Cars: " + cars.Length);
            // Debug.Log("lookDir: " + lookDir + " cameraSpeed: " + cameraSpeed);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCar--;
            if (currentCar < 0)
            {
                currentCar = cars.Length - 1;
            }
            // Debug.Log("CurrentCar: " + currentCar + " Total Cars: " + cars.Length);
            // Debug.Log("lookDir: " + lookDir + " cameraSpeed: " + cameraSpeed);
        }

        PlayerPrefs.SetInt("PlayerCar", currentCar);

        AdjustCamera();
    }

    private void AdjustCamera()
    {
        Vector3 newPosition = this.transform.position;
        if (currentCar < cars.Length / 2)
        {
            newPosition = new Vector3(
                this.transform.position.x,
                heightLevel1 + cameraHeight,
                this.transform.position.z);
        }
        else
        {
            newPosition = new Vector3(
                this.transform.position.x,
                heightLevel2 + cameraHeight,
                this.transform.position.z);
        }

        // this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.deltaTime * cameraSpeed * 2.0f);
        this.transform.position = newPosition;

        lookDir = Quaternion.LookRotation(cars[currentCar].transform.position - new Vector3(0, -0.5f, 0) - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookDir, Time.deltaTime * cameraSpeed);
        // Debug.Log("Rotation: " + this.transform.rotation + " lookDir: " + lookDir + " time offset: " + Time.deltaTime * cameraSpeed);

    }
}
