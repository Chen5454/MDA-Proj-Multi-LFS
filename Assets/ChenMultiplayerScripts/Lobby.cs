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
    public VivoxManager _VivoxManager;

    public TMP_Text buttonText;
    public TMP_InputField usernameInput;

    private bool isConnecting;
    private byte maxPlayersPerRoom = 50;

    public System.Action OnPlayerListChange;



    public string _channelName;
    public string _channelName2;
    public TMP_InputField VivoxusernameInput;



    private void Awake()
    {
        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _VivoxManager = GameObject.FindObjectOfType<VivoxManager>();


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

    #region MyRegion
    public void LoginUser()
    {
        if (_VivoxManager.FilterChannelAndUserName(usernameInput.text))
        {
            _VivoxManager.Login(usernameInput.text);
        }
    }

    public void JoinChannelClicked()
    {
        _VivoxManager.VivoxJoin3DPositional(_channelName, true, false, true, ChannelType.Positional, 10, 5, 5, AudioFadeModel.InverseByDistance);
    }
    public void LeaveChannelClick()
    {
        _VivoxManager.LeaveChannel(_VivoxManager.vivox.channelSession, _channelName);
        // _VivoxManager.LeaveChannel(_VivoxManager.vivox.channelSession2, _channelName2);
    }

    public void Logout()
    {
        _VivoxManager.vivox.loginSession.Logout();
        _VivoxManager.BindLoginCallBack(false, _VivoxManager.vivox.loginSession);
    }

    #endregion


}
