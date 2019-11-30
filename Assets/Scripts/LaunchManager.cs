using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LaunchManager : MonoBehaviour
{
    public InputField playerName;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    public void ConnectSingle()
    {
        SceneManager.LoadScene("Track1");
    }
    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit ();
        #endif
    }

    public void SetName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
