using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceMonitor : MonoBehaviour
{
    public static int totalLaps = 1;
    public GameObject[] countdownItems;
    public static bool racing = false;
    public GameObject gameOverPanel;
    public GameObject HUD;

    CheckpointManager[] carsCPManagers;
    List<CheckpointManager> playerCPManagers;

    public GameObject[] carPrefabs;
    public Transform[] spawnPos;
    public GameObject[] arrowTags;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject countdownItem in countdownItems)
        {
            countdownItem.SetActive(false);
        }

        StartCoroutine(PlayCountDown());
        gameOverPanel.SetActive(false);

        int playerIndex = Random.Range(0, spawnPos.Length);

        int spawnPosIndex = 0;
        foreach (Transform t in spawnPos)
        {
            GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
            car.transform.position = t.position;
            car.transform.rotation = t.rotation;

            GameObject carBody = null;
            foreach (Transform child in car.transform)
            {
                if (child.tag == "car")
                {
                    carBody = child.gameObject;
                }
            }
            arrowTags[spawnPosIndex].GetComponent<TagFollowVehicle>().targetVehicleBody = carBody;

            Camera[] cameras = car.GetComponentsInChildren<Camera>();
            Camera frontCamera = cameras[0];
            Camera rearCamera = cameras[1];

            if (spawnPosIndex == playerIndex)
            {
                car.GetComponent<AIController>().enabled = false;
                car.GetComponent<PlayerController>().enabled = true;
                car.GetComponent<CameraFollow>().enabled = true;

                frontCamera.tag = "MainCamera";
                frontCamera.targetDisplay = 0; // set targetDisplay to Display1
                frontCamera.targetTexture = null;
                AudioListener audioListener = frontCamera.GetComponent<AudioListener>();
                audioListener.enabled = true;

                rearCamera.enabled = true;
            }
            else
            {
                rearCamera.enabled = false; ;
            }
            spawnPosIndex++;
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
        // Debug.Log("playerCPManagers.Count: " + playerCPManagers.Count);
    }

    IEnumerator PlayCountDown()
    {
        yield return new WaitForSeconds(2);
        foreach (GameObject countdownItem in countdownItems)
        {
            countdownItem.SetActive(true);
            yield return new WaitForSeconds(1);
            countdownItem.SetActive(false);
        }
        racing = true;
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

        // Debug.Log("finishedCount: " + finishedCount + "playerCPManagers.Count: " + playerCPManagers.Count);

    }
}
