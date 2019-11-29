using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceMonitor : MonoBehaviour
{
    public GameObject[] countdownItems;
    public static bool racing = false;
    public static int totalLaps = 2;
    public GameObject gameOverPanel;
    public GameObject HUD;

    CheckpointManager[] carsCPManagers;
    List<CheckpointManager> playerCPManagers;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject countdownItem in countdownItems)
        {
            countdownItem.SetActive(false);
        }

        StartCoroutine(PlayCountDown());
        gameOverPanel.SetActive(false);

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
