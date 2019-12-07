using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class HUDController : MonoBehaviour
{
    CanvasGroup canvasGroupHUD;
    float HUDSetting = 0.0f;

    float inputAxisTimer;
    float inputAxisCooldown = 0.5f;
    bool inputAxisUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroupHUD = GetComponentInParent<CanvasGroup>();
        canvasGroupHUD.alpha = 0.0f;

        if (PlayerPrefs.HasKey("HUDSetting"))
        {
            HUDSetting = PlayerPrefs.GetFloat("HUDSetting");
            canvasGroupHUD.alpha = HUDSetting;
        }

        inputAxisTimer = 0.0f;
        inputAxisUnlocked = true;
    }

    // Update is called once per frame
    private void Update()
    {
        inputAxisTimer += Time.deltaTime;
        if (inputAxisTimer > inputAxisCooldown)
        {
            inputAxisUnlocked = true;
            inputAxisTimer = 0.0f;
        }

        if (!RaceMonitor.racing)
        {
            canvasGroupHUD.alpha = 0.0f;
            return;
        }

        canvasGroupHUD.alpha = HUDSetting;

        if (Input.GetKeyDown(KeyCode.H) || (CrossPlatformInputManager.GetAxisRaw("Fire3") == 1.0f && inputAxisUnlocked))
        {
            canvasGroupHUD.alpha = (canvasGroupHUD.alpha == 1.0f) ? 0.0f : 1.0f;
            HUDSetting = canvasGroupHUD.alpha;
            PlayerPrefs.SetFloat("HUDSetting", HUDSetting);
            inputAxisUnlocked = false;
        }
    }
}
