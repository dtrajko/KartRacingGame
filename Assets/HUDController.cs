using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    CanvasGroup canvasGroupHUD;
    float HUDSetting = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroupHUD = GetComponentInParent<CanvasGroup>();
        canvasGroupHUD.alpha = 0.0f;

        if (PlayerPrefs.HasKey("HUDSetting"))
        {
            HUDSetting = PlayerPrefs.GetFloat("HUDSetting");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!RaceMonitor.racing)
        {
            canvasGroupHUD.alpha = 0.0f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            canvasGroupHUD.alpha = (canvasGroupHUD.alpha == 1.0f) ? 0.0f : 1.0f;
            HUDSetting = canvasGroupHUD.alpha;
            PlayerPrefs.SetFloat("HUDSetting", HUDSetting);
        }
    }
}
