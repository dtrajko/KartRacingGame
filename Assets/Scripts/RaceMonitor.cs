using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;


public struct CarPrefabInfo
{
    public string type;
    public string color;

    public CarPrefabInfo(string inType, string inColor)
    {
        type = inType;
        color = inColor;
    }
}

public class RaceMonitor : MonoBehaviourPunCallbacks
{
    public static float soundVolume = 0.2f;
    public static int totalLaps = 2;
    public GameObject[] countdownItems;
    public static bool racing = false;
    public static bool pause = false;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject HUD;
    public GameObject startRace;

    CheckpointManager[] carsCPManagers;
    List<CheckpointManager> playerCPManagers;

    public GameObject[] carPrefabs;
    public Transform[] spawnPositions;
    public GameObject[] arrowTags;

    int playerPrefsCarIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = soundVolume;

        foreach (GameObject countdownItem in countdownItems)
        {
            countdownItem.SetActive(false);
        }

        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);

        startRace.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startRace.SetActive(true);
            }
        }
        else
        {
            StartGame(1.0f);
        }

        if (PlayerPrefs.HasKey("PlayerCar"))
        {
            playerPrefsCarIndex = PlayerPrefs.GetInt("PlayerCar");
        }

        int randomPlayerStartPosition = Random.Range(0, spawnPositions.Length);

        // Debug.Log("[Player] Car prefab " + playerPrefsCarIndex + " to spawn position " + randomPlayerStartPosition);

        GameObject playerCar = Instantiate(carPrefabs[playerPrefsCarIndex]);
        playerCar.transform.position = spawnPositions[randomPlayerStartPosition].position;
        playerCar.transform.rotation = spawnPositions[randomPlayerStartPosition].rotation;

        playerCar.GetComponent<AIController>().enabled = false;
        playerCar.GetComponent<PlayerController>().enabled = true;
        playerCar.GetComponent<CameraFollow>().enabled = true;

        Camera[] cameras = playerCar.GetComponentsInChildren<Camera>();
        Camera frontCamera = cameras[0];
        Camera rearCamera = cameras[1];

        frontCamera.tag = "MainCamera";
        frontCamera.targetDisplay = 0; // set targetDisplay to Display1
        frontCamera.targetTexture = null;
        AudioListener audioListener = frontCamera.GetComponent<AudioListener>();
        audioListener.enabled = true;
        rearCamera.enabled = true;

        assignArrowTag(playerCar, randomPlayerStartPosition);

        int spawnPositionIndex = -1;
        foreach (Transform spawnPosition in spawnPositions)
        {
            spawnPositionIndex++;

            if (spawnPositionIndex == randomPlayerStartPosition)
            {
                // PlayerController
                // Debug.Log("[Player in loop] Spawn position " + spawnPositionIndex + " ignored.");
                continue;
            }

            // NPC - AIController
            int carPrefabRandomIndex = Random.Range(0, carPrefabs.Length);

            GameObject car = Instantiate(carPrefabs[carPrefabRandomIndex]);

            // Debug.Log("[AI] Car prefab " + carPrefabRandomIndex + " to spawn position " + spawnPositionIndex);

            car.transform.position = spawnPosition.position;
            car.transform.rotation = spawnPosition.rotation;

            cameras = car.GetComponentsInChildren<Camera>();
            frontCamera = cameras[0];
            rearCamera = cameras[1];
            rearCamera.enabled = false;

            assignArrowTag(car, spawnPositionIndex);
        }

        GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
        carsCPManagers = new CheckpointManager[cars.Length];
        playerCPManagers = new List<CheckpointManager>();

        for (int i = 0; i < cars.Length; i++)
        {
            carsCPManagers[i] = cars[i].GetComponent<CheckpointManager>();
            if (cars[i].GetComponentInParent<PlayerController>().enabled) {
                playerCPManagers.Add(cars[i].GetComponent<CheckpointManager>());
            }
        }
    }

    public void StartGame(float initDelay)
    {
        StartCoroutine(PlayCountDown(initDelay));
        startRace.SetActive(false);
    }

    IEnumerator PlayCountDown(float initDelay)
    {
        yield return new WaitForSeconds(initDelay);
        foreach (GameObject countdownItem in countdownItems)
        {
            countdownItem.SetActive(true);
            yield return new WaitForSeconds(1);
            countdownItem.SetActive(false);
        }
        racing = true;
    }

    private void LateUpdate()
    {
        int finishedCount = 0;
        foreach (CheckpointManager playerCPManager in playerCPManagers)
        {
            if (playerCPManager.lap == totalLaps + 1)
            {
                finishedCount++;
            }
        }

        if (finishedCount == playerCPManagers.Count && !gameOverPanel.activeSelf)
        {
            HUD.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Debug.Log("finishedCount: " + finishedCount + "playerCPManagers.Count: " + playerCPManagers.Count);
    }

    public void TogglePause()
    {
        pause = !pause;
        HUD.SetActive(pause == false);
        pausePanel.SetActive(pause == true);
        Time.timeScale = pause ? 0.0f : 1.0f;
        AudioListener.volume = pause ? 0.0f : soundVolume;
    }

    private void assignArrowTag(GameObject car, int spawnPositionIndex)
    {
        // Arrow tags
        GameObject carBody = null;
        foreach (Transform child in car.transform)
        {
            if (child.tag == "car")
            {
                carBody = child.gameObject;
            }
        }
        arrowTags[spawnPositionIndex].GetComponent<TagFollowVehicle>().targetVehicleBody = carBody;
    }

    public void RestartLevel()
    {
        racing = false;
        SceneManager.LoadScene("Track1");
    }

    public void MainMenu()
    {
        racing = false;
        SceneManager.LoadScene("MainMenu");
    }

    public static CarPrefabInfo GetCarPrefabInfo(int playerPrefsIndex)
    {
        CarPrefabInfo carPrefabInfo;
        switch (playerPrefsIndex)
        {
            case 0:
                carPrefabInfo = new CarPrefabInfo("Car", "Red");
                break;
            case 1:
                carPrefabInfo = new CarPrefabInfo("Car", "Magenta");
                break;
            case 2:
                carPrefabInfo = new CarPrefabInfo("Car", "Green");
                break;
            case 3:
                carPrefabInfo = new CarPrefabInfo("Car", "Yellow");
                break;
            case 4:
                carPrefabInfo = new CarPrefabInfo("Jeep", "Red");
                break;
            case 5:
                carPrefabInfo = new CarPrefabInfo("Jeep", "Magenta");
                break;
            case 6:
                carPrefabInfo = new CarPrefabInfo("Jeep", "Green");
                break;
            case 7:
                carPrefabInfo = new CarPrefabInfo("Jeep", "Yellow");
                break;
            default:
                carPrefabInfo = new CarPrefabInfo("N/A", "N/A");
                break;
        }
        return carPrefabInfo;
    }
}
