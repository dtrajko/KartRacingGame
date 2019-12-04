﻿using System.Collections;
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

    private void Awake()
    {
        Time.timeScale = 1.0f;

        isConnecting = false;

        PhotonNetwork.AutomaticallySyncScene = true;

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName.text = PlayerPrefs.GetString("PlayerName");
        }

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

    public void ConnectNetwork()
    {
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
        isConnecting = false;
        PhotonNetwork.Disconnect();
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
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text += "On Disconnected [cause: " + cause + "]" + "\n";
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        feedbackText.text += "Joined Room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players." + "\n";
        PhotonNetwork.LoadLevel("Track1");
    }
}
