using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCar : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] levels;
    int currentCar = 0;
    float heightLevel1;
    float heightLevel2;
    float cameraHeight = 3.0f;
    float cameraSpeed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerCar"))
        {
            currentCar = PlayerPrefs.GetInt("PlayerCar");
        }

        this.transform.LookAt(cars[currentCar].transform.position);
        heightLevel1 = levels[0].transform.position.y;
        heightLevel2 = levels[1].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCar++;
            if (currentCar > cars.Length - 1)
            {
                currentCar = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCar--;
            if (currentCar < 0)
            {
                currentCar = cars.Length - 1;
            }
        }

        PlayerPrefs.SetInt("PlayerCar", currentCar);

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

        Quaternion lookDir = Quaternion.LookRotation(cars[currentCar].transform.position - new Vector3(0, -0.5f, 0) - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime * cameraSpeed);
    }
}
