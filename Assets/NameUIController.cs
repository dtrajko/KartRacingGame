using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NameUIController : MonoBehaviour
{
    public Text playerName;
    public Transform target;
    CanvasGroup canvasGroup;
    public Renderer carRend;

    int minFontSize = 20;
    int maxFontSize = 30;
    float maxDistanceToCamera = 120.0f;

    // Start is called before the first frame update
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = this.GetComponent<Text>();
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
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

                // Debug.Log("playerName: " + playerName.text + " maxDistanceToCamera: " + maxDistanceToCamera + " distanceToCamera: " + distanceToCamera);
                // Debug.Log("playerName: " + playerName.text + " distanceFactor: " + distanceFactor + " fontSize: " + GetComponent<Text>().fontSize);
            }
            else
            {
                canvasGroup.alpha = 0.0f;
            }
        }
    }
}
