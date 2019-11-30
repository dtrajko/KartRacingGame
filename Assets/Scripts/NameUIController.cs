using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NameUIController : MonoBehaviour
{
    public Text playerName;
    public Text lapDisplay;
    public Transform target;
    CanvasGroup canvasGroup;
    public Renderer carRend;
    CheckpointManager cpManager;

    int carRego = -1;

    int minFontSize = 20;
    int maxFontSize = 28;
    float maxDistanceToCamera = 150.0f;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.Find("Canvas") == null)
        {
            Debug.Log("The 'Canvas' object is disabled?");
        }

        this.transform.SetParent(GameObject.Find("PlayerNames").GetComponent<Transform>(), false);
        playerName = this.GetComponent<Text>();
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!RaceMonitor.racing)
        {
            canvasGroup.alpha = 0.0f;
            return;
        }

        if (Camera.main == null)
        {
            Debug.Log("Camera.main undefined. The 'Main camera' tag is not assigned?");
            return;
        }

        if (carRend == null)
        {
            Debug.Log("The 'carRend' variable is undefined.");
            return;
        }

        if (carRego == -1 && playerName.text != "PLAYER NAME")
        {
            carRego = Leaderboard.RegisterCar(playerName.text); // before: 
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRend.bounds);
        canvasGroup.alpha = carInView ? 1.0f : 0.0f;
        Vector3 worldPosition = target.position + Vector3.up * 1.4f;
        this.transform.position = Camera.main.WorldToScreenPoint(worldPosition);

        if (carInView)
        { 
            float distanceToCamera = Mathf.Abs(Vector3.Distance(Camera.main.transform.position, worldPosition));
            if (distanceToCamera < maxDistanceToCamera)
            {
                canvasGroup.alpha = 1.0f;
                float distanceFactor = Mathf.Clamp(maxDistanceToCamera / (distanceToCamera * 3.0f), 0, 1);
                int fontSizeNew = (int)(distanceFactor * maxFontSize);
                GetComponent<Text>().fontSize = Mathf.Clamp(fontSizeNew, minFontSize, maxFontSize);
                lapDisplay.fontSize = Mathf.Clamp(fontSizeNew - 6, minFontSize, maxFontSize - 6);

                // Debug.Log("playerName: " + playerName.text + " maxDistanceToCamera: " + maxDistanceToCamera + " distanceToCamera: " + distanceToCamera);
                // Debug.Log("playerName: " + playerName.text + " distanceFactor: " + distanceFactor + " fontSize: " + GetComponent<Text>().fontSize);
            }
            else
            {
                canvasGroup.alpha = 0.0f;
            }

            // lap manager
            if (cpManager == null)
            {
                cpManager = target.GetComponentInParent<CheckpointManager>();
            }

            Leaderboard.SetPosition(carRego, cpManager.lap, cpManager.checkPoint, cpManager.timeEntered);
            string position = Leaderboard.GetPosition(carRego);

            int progressPercentage = 0;
            if (cpManager.checkPointCount > 0)
            {
                progressPercentage = Mathf.Clamp((int)(((float)cpManager.checkPoint / (float)cpManager.checkPointCount) * 100.0f), 0, 100);
            }
            lapDisplay.text = position + " - Lap " + cpManager.lap + " / " + RaceMonitor.totalLaps + " (" + progressPercentage + "%)";
        }
    }
}
