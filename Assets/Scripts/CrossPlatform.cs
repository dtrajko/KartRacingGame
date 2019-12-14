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
            case RuntimePlatform.WindowsEditor:
                isMobilePlatform = false;
                break;
            // Mobile platforms
            case RuntimePlatform.Android:
                isMobilePlatform = true;
                break;
            default:
                isMobilePlatform = false;
                break;

        }
        return isMobilePlatform;
    }
}
