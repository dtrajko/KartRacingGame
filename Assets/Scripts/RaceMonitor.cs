using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;


public struct CarPrefabInfo
{
    public int id;
    public string type;
    public string color;

    public CarPrefabInfo(int inId, string inType, string inColor)
    {
        id = inId;
        type = inType;
        color = inColor;
    }
}

public class RaceMonitor : MonoBehaviourPunCallbacks
{
    public static float soundVolume = 0.2f;
    public static int totalLaps = 1;
    public GameObject[] countdownItems;
    public static bool racing = false;
    public static bool pause = false;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject HUD;
    public GameObject startGameButton;
    public GameObject startGameWaitingText;

    CheckpointManager[] carsCPManagers;
    List<CheckpointManager> playerCPManagers;

    public GameObject[] carPrefabs;
    public Transform[] spawnPositions;
    public GameObject[] arrowTags;

    int playerPrefsCarIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        racing = false;

        AudioListener.volume = soundVolume;

        foreach (GameObject countdownItem in countdownItems)
        {
            countdownItem.SetActive(false);
        }

        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        startGameButton.SetActive(false);
        startGameWaitingText.SetActive(false);

        playerCPManagers = new List<CheckpointManager>();

        if (PlayerPrefs.HasKey("PlayerCar"))
        {
            playerPrefsCarIndex = PlayerPrefs.GetInt("PlayerCar");
        }

        int playerSpawnPositionIndex = Random.Range(0, spawnPositions.Length);
        Vector3 playerStartPosition = spawnPositions[playerSpawnPositionIndex].position;
        Quaternion playerStartRotation = spawnPositions[playerSpawnPositionIndex].rotation;

        GameObject playerCar = null;

        if (PhotonNetwork.IsConnected)
        {
            // Multiplayer (NetworkPlayer)
            playerSpawnPositionIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            playerStartPosition = spawnPositions[playerSpawnPositionIndex].position;
            playerStartRotation = spawnPositions[playerSpawnPositionIndex].rotation;

            if (NetworkedPlayer.LocalPlayerInstance == null)
            {
                playerCar = PhotonNetwork.Instantiate(carPrefabs[playerPrefsCarIndex].name, playerStartPosition, playerStartRotation, 0);
                SetupScripts(playerCar, true);
                SetupCameras(playerCar, true);
                assignArrowTag(playerCar, playerSpawnPositionIndex);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                startGameButton.SetActive(true);
            }
            else
            { 
                startGameWaitingText.SetActive(true);
            }
        }
        else
        {
            // Single Player (PlayerController)
            for (int spawnPosIndex = 0; spawnPosIndex < spawnPositions.Length; spawnPosIndex++)
            {
                if (spawnPosIndex == playerSpawnPositionIndex)
                {
                    // PlayerController
                    playerCar = InstantiateCar(playerStartPosition, playerStartRotation, playerPrefsCarIndex, playerSpawnPositionIndex, true);
                }
                else
                {
                    // AIController (NPC)
                    int carPrefabIndex = Random.Range(0, carPrefabs.Length);
                    InstantiateCar(spawnPositions[spawnPosIndex].position, spawnPositions[spawnPosIndex].rotation, carPrefabIndex, spawnPosIndex, false);
                }
            }

            RPC_StartGame(); // this RPC also used in single player
        }
    }

    public void BeginGame()
    {
        NetworkSpawnNPCs();
        photonView.RPC("RPC_StartGame", RpcTarget.All);
    }

    public void NetworkSpawnNPCs()
    {
        byte playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        byte maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        for (int spawnPositionIndex = playerCount; spawnPositionIndex < maxPlayers; spawnPositionIndex++)
        {
            Vector3 startPosition = spawnPositions[spawnPositionIndex].position;
            Quaternion startRotation = spawnPositions[spawnPositionIndex].rotation;
            int carPrefabIndex = Random.Range(0, carPrefabs.Length);

            object[] instanceData = new object[1];
            instanceData[0] = carPrefabs[carPrefabIndex].GetComponent<Drive>().playerName;
            GameObject AICar = PhotonNetwork.Instantiate(carPrefabs[carPrefabIndex].name, startPosition, startRotation, 0, instanceData);
            // AICar.GetComponent<Drive>().networkName = (string)instanceData[0];
            AICar.GetComponent<Drive>().networkName = carPrefabs[carPrefabIndex].GetComponent<Drive>().playerName;

            SetupScripts(AICar, false);
            SetupCameras(AICar, false);
            assignArrowTag(AICar, spawnPositionIndex);

        }
    }

    [PunRPC]
    public void RPC_StartGame()
    {
        float initDelay = 1.0f;
        StartCoroutine(PlayCountDown(initDelay));
        startGameButton.SetActive(false);
        startGameWaitingText.SetActive(false);
        SetupCheckpointManagers();
    }

    private GameObject InstantiateCar(Vector3 position, Quaternion rotation, int carPrefabIndex, int spawnPositionIndex, bool isPlayer)
    {
        GameObject car = Instantiate(carPrefabs[carPrefabIndex]);
        car.transform.position = position;
        car.transform.rotation = rotation;

        SetupScripts(car, isPlayer);
        SetupCameras(car, isPlayer);
        assignArrowTag(car, spawnPositionIndex);

        return car;
    }

    private void SetupScripts(GameObject car, bool isPlayer)
    {
        car.GetComponent<CameraFollow>().enabled = false;

        if (isPlayer)
        {
            // PlayerController
            car.GetComponent<Drive>().enabled = true;
            car.GetComponent<AIController>().enabled = false;
            car.GetComponent<PlayerController>().enabled = true;

            if (photonView.IsMine)
            {
                car.GetComponent<CameraFollow>().enabled = true;
            }
        }
        else
        {
            // AIController
            car.GetComponent<Drive>().enabled = true;
            car.GetComponent<AIController>().enabled = true;
            car.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void SetupCameras(GameObject car, bool isPlayer)
    {
        Camera[] cameras = car.GetComponentsInChildren<Camera>();
        Camera frontCamera = cameras[0];
        Camera rearCamera = cameras[1];

        rearCamera.enabled = false;

        if (isPlayer)
        {
            frontCamera.tag = "MainCamera";
            frontCamera.targetDisplay = 0; // set targetDisplay to Display1
            frontCamera.targetTexture = null;
            AudioListener audioListener = frontCamera.GetComponent<AudioListener>();
            audioListener.enabled = true;

            rearCamera.enabled = true;
        }
    }

    private void SetupCheckpointManagers()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
        carsCPManagers = new CheckpointManager[cars.Length];

        for (int i = 0; i < cars.Length; i++)
        {
            carsCPManagers[i] = cars[i].GetComponent<CheckpointManager>();
            if (cars[i].GetComponentInParent<PlayerController>().enabled)
            {
                playerCPManagers.Add(cars[i].GetComponent<CheckpointManager>());
            }
        }
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
        if (!racing)
        {
            // return;
        }

        int finishedCount = 0;
        foreach (CheckpointManager playerCPManager in playerCPManagers)
        {
            if (playerCPManager.lap == totalLaps + 1)
            {
                finishedCount++;
            }
        }

        if (playerCPManagers.Count > 0 && finishedCount == playerCPManagers.Count && !gameOverPanel.activeSelf)
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

    [PunRPC]
    public void RPC_RestartGame()
    {
        PhotonNetwork.LoadLevel("Track1");
    }

    public void RestartLevel()
    {
        racing = false;
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_RestartGame", RpcTarget.All);
        }
        else
        { 
            SceneManager.LoadScene("Track1");
        }
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
                carPrefabInfo = new CarPrefabInfo(0, "Car", "Red");
                break;
            case 1:
                carPrefabInfo = new CarPrefabInfo(1, "Car", "Magenta");
                break;
            case 2:
                carPrefabInfo = new CarPrefabInfo(2, "Car", "Green");
                break;
            case 3:
                carPrefabInfo = new CarPrefabInfo(3, "Car", "Yellow");
                break;
            case 4:
                carPrefabInfo = new CarPrefabInfo(4, "Jeep", "Red");
                break;
            case 5:
                carPrefabInfo = new CarPrefabInfo(5, "Jeep", "Magenta");
                break;
            case 6:
                carPrefabInfo = new CarPrefabInfo(6, "Jeep", "Green");
                break;
            case 7:
                carPrefabInfo = new CarPrefabInfo(7, "Jeep", "Yellow");
                break;
            default:
                carPrefabInfo = new CarPrefabInfo(-1, "N/A", "N/A");
                break;
        }
        return carPrefabInfo;
    }
}
