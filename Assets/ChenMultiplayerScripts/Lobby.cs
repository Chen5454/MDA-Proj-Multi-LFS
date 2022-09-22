using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using VivoxUnity;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private VivoxManager _VivoxManager;

    public TMP_Text buttonText;
    public TMP_InputField usernameInput;

    private bool isConnecting;
    private byte maxPlayersPerRoom = 50;

    public System.Action OnPlayerListChange;

    public TMP_InputField VivoxusernameInput;



    private void Start()
    {
        //_VivoxManager = GameObject.FindObjectOfType<VivoxManager>();


        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }

    }

    public void Connect()
    {

        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username", usernameInput.text);
            buttonText.text = "Connecting...";
            isConnecting = PhotonNetwork.ConnectUsingSettings();
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

    public override void OnLeftRoom()
    {
        OnPlayerListChange?.Invoke();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // if we failed to join a random room, maybe none exists or they are all full so we create a new room.

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

    }



    #region MyRegion
    public void LoginUser()
    {
        if (_VivoxManager.FilterChannelAndUserName(usernameInput.text))
        {
            _VivoxManager.Login(usernameInput.text);
        }
    }



    #endregion


}
