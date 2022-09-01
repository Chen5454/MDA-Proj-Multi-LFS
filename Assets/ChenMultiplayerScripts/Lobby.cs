using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviourPunCallbacks
{

    public TMP_Text buttonText;
    public TMP_InputField usernameInput;

    private bool isConnecting;
    private byte maxPlayersPerRoom = 50;

    public System.Action OnPlayerListChange;


    private void Awake()
    {
        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }

    }

    public void Connect()
    {

        if (usernameInput.text.Length>=1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username",usernameInput.text);
            buttonText.text = "Connecting...";
            isConnecting =  PhotonNetwork.ConnectUsingSettings();
        }


    }


    // this function will be called automatically by photon if we successfully connected to photon.
    public override void OnConnectedToMaster()
    {

        // SceneManager.LoadScene("Lobby");
        // PhotonNetwork.JoinLobby(); ---- we gonna use other method to auto log us into the scene
        PhotonNetwork.AutomaticallySyncScene = true;

        if (isConnecting)
      {
          PhotonNetwork.JoinRandomRoom();
          isConnecting = false;
      }
    }

    //public override void OnJoinedRoom()
    //{

    //    if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
    //    {
    //        PhotonNetwork.LoadLevel(1);
    //    }
    //    else
    //    {
    //        OnPlayerListChange?.Invoke();
    //    }
    //}

    public override void OnLeftRoom()
    {
        OnPlayerListChange?.Invoke();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // if we failed to join a random room, maybe none exists or they are all full so we create a new room.

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

    }

    //When we joined the lobby we are opening our Scene. in this case Lobby
    //public override void OnJoinedLobby()
    //{
    //    SceneManager.LoadScene("Lobby");

    //}
}
