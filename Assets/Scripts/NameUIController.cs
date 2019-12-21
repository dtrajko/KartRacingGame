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
    public MeshRenderer carRenderer;
    CheckpointManager cpManager;

    int minFontSize = 24;
    int maxFontSize = 34;
    float maxDistanceToCamera = 150.0f;
    int carRego = -1;
    int fontSizeDiff;

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

        fontSizeDiff = GetComponent<Text>().fontSize - lapDisplay.fontSize;
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

        if (carRenderer == null)
        {
            Debug.Log("The 'carRenderer' variable is undefined [playerName: " + playerName.text + "]");
            return;
        }

        if (carRego == -1 && playerName.text != "PLAYER NAME")
        {
            carRego = Leaderboard.RegisterCar(playerName.text);
            return;
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRenderer.bounds);
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
                lapDisplay.fontSize = Mathf.Clamp(fontSizeNew - fontSizeDiff, minFontSize, maxFontSize - fontSizeDiff);
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

            if (carRego > -1)
            { 
                Leaderboard.SetPosition(carRego, cpManager.lap, cpManager.checkPoint, cpManager.timeEntered);
                string position = Leaderboard.GetPosition(carRego);

                int progressPercentage = 0;
                if (cpManager.checkPointCount > 0)
                {
                    progressPercentage = Mathf.Clamp((int)(((float)cpManager.checkPoint / (float)cpManager.checkPointCount) * 100.0f), 0, 100);
                }
                lapDisplay.text = position + " - Lap " + cpManager.lap + "/" + FindObjectOfType<RaceMonitor>().totalLaps + " [" + progressPercentage + "%]";

            }
        }
    }
}
