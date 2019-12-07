using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPlatform : MonoBehaviour
{
    public static bool IsMobilePlatform()
    {
        bool isMobilePlatform = false;
        switch (Application.platform)
        {
            // Desktop platforms
            case RuntimePlatform.WindowsPlayer:
                isMobilePlatform = false;
                break;
            // Mobile platforms
            case RuntimePlatform.Android:
            case RuntimePlatform.WindowsEditor:
                isMobilePlatform = true;
                break;
            default:
                isMobilePlatform = false;
                break;

        }
        return isMobilePlatform;
    }
}
