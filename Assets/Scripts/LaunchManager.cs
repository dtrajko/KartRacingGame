using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;


public class LaunchManager : MonoBehaviourPunCallbacks
{
    byte maxPlayersPerRoom = 4;
    bool isConnecting;
    public InputField playerName;
    public Text feedbackText;
    string gameVersion = "1";
    public AudioSource buttonSound;

    int currentLevel = 0;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        Time.timeScale = 1.0f;
        isConnecting = false;

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName.text = PlayerPrefs.GetString("PlayerName");
        }

        UpdateLevel();

        feedbackText.text = "";

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InRoom)
            {
                // feedbackText.text += "LM.Awake: InRoom? YES. LeaveRoom." + "\n";
                PhotonNetwork.LeaveRoom();
            }
        
            if (!PhotonNetwork.InLobby)
            {
                // feedbackText.text += "LM.Awake: InLobby? NO. JoinLobby." + "\n";
                PhotonNetwork.JoinLobby();
            }
        }
    }

    private void UpdateLevel()
    {
        if (PlayerPrefs.HasKey("PlayerTrack"))
        {
            currentLevel = PlayerPrefs.GetInt("PlayerTrack");
        }
    }

    public void ConnectNetwork()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        buttonSound.Play();

        feedbackText.text = "";
        isConnecting = true;

        PhotonNetwork.NickName = playerName.text;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            feedbackText.text += "Joining Room..." + "\n";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            feedbackText.text += "Connecting..." + "\n";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ConnectSingle()
    {
        buttonSound.Play();

        isConnecting = false;
        PhotonNetwork.Disconnect();

        UpdateLevel();
        SceneManager.LoadScene(RaceMonitor.GetLevelNameById(currentLevel));
        // Debug.Log("LaunchManager.ConnectSingle LoadScene: Track1");

    }

    public void Exit()
    {
        buttonSound.Play();

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

    // Network Callbacks
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            feedbackText.text += "On Connected To Master..." + "\n";
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedLobby()
    {
        feedbackText.text += "On Joined Lobby..." + "\n";
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        feedbackText.text += "Failed to join random room [code: " + returnCode + ", message: " + message + "]" + "\n";
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = this.maxPlayersPerRoom };
        // roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        // roomOptions.CustomRoomProperties.Add("level", currentLevel);
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (feedbackText)
        { 
            feedbackText.text += "On Disconnected [cause: " + cause + "]" + "\n";
        }

        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        if (feedbackText)
        {
            feedbackText.text += "Joined Room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players." + "\n";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            UpdateLevel();
            // Debug.Log("LaunchManager.OnJoinedRoom LoadLevel: " + RaceMonitor.GetLevelNameById(currentLevel));
            PhotonNetwork.LoadLevel(RaceMonitor.GetLevelNameById(currentLevel));
        }
    }
}
