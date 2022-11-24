using System;
using System.IO;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private ToggleButton _femaleAvatar, _maleAvatar;
    [SerializeField] private string _pickAvatarText, _idleConnectText, _connectingText;
    [SerializeField] private bool _isAvatarPicked;

    public TMP_Text buttonText;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    private bool isConnecting;
    private byte maxPlayersPerRoom = 50;

    public System.Action OnPlayerListChange;

    public TMP_InputField VivoxusernameInput;

    public string PhotonRoomName = "MDA";
    public Button ConnectButton;
    public Toggle ConnectAsPikud10;
    public Toggle ConnectAsInstructor;

    public GameObject PasswordInput;
    public GameObject PasswordTitle;
    public GameObject WrongInput;
    private string filepath;
    private void Start()
    {
         filepath = Application.streamingAssetsPath + "/UsersAndPasswords/" + "UsersAndPasswords" + ".txt";

        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }

        ConnectButton.interactable = false;
        DontDestroyOnLoad(this.gameObject);

    }

    private void Update()
    {
        if (!_maleAvatar.IsBtnSelected && !_femaleAvatar.IsBtnSelected)
        {
            Debug.Log("No Avatar Selected");
            return;
        }
        else if (!_isAvatarPicked)
        {
            ConnectButton.interactable = true;
            buttonText.text = _idleConnectText;
            _isAvatarPicked = true;
        }
    }

    public void Connect()
    {
        if (usernameInput.text.Length < 1)
        {
            Debug.Log("Need name to proceed");
            return;
        }

        if (usernameInput.text.Length >= 1 && !ConnectAsInstructor.isOn)
        {
            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username", usernameInput.text);
            buttonText.text = _connectingText;
            isConnecting = PhotonNetwork.ConnectUsingSettings();


            Debug.Log("Login Into Vivox now....");
            LoginUser();
        }



        if (usernameInput.text.Length >= 1 && ConnectAsInstructor.isOn)
        {

            if (VerifyLogin(usernameInput.text, passwordInput.text, filepath))
            {
                PhotonNetwork.NickName = usernameInput.text;
                PlayerPrefs.SetString("username", usernameInput.text);
                buttonText.text = _connectingText;
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                Debug.Log("Login Into Vivox now....");
                WrongInput.SetActive(false);
                LoginUser();
            }
            else
            {
                WrongInput.SetActive(true);
                ConnectButton.interactable = true;
                Debug.Log("Username or Password is wrong");
            }
        }
    }


    private void CreateRoom()
    {
        Debug.Log(" we are creating a new room......");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.EmptyRoomTtl = 1;
        roomOptions.PlayerTtl = 1;
      //  roomOptions.CleanupCacheOnLeave = false; // I dont like this at all~
        PhotonNetwork.CreateRoom(PhotonRoomName, roomOptions, TypedLobby.Default);
    }

    // this function will be called automatically by photon if we successfully connected to photon.
    public override void OnConnectedToMaster()
    {
        // SceneManager.LoadScene("Lobby");
        // PhotonNetwork.JoinLobby(); ---- we gonna use other method to auto log us into the scene
        Debug.Log("OnConnectedToMaster");

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
        Debug.Log("Joining Room Failed. we are creating a new room......");
        CreateRoom();

        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Name Is :"+""+PhotonNetwork.CurrentRoom.Name);
      

    }


    public void InstructorButtonClick()
    {
        if (PasswordInput.activeInHierarchy&& PasswordTitle.activeInHierarchy)
        {
            PasswordInput.SetActive(false);
            PasswordTitle.SetActive(false);
        }
        else
        {
            PasswordInput.SetActive(true);
            PasswordTitle.SetActive(true);
        }
    }

    #region VivoxLogin
    public void LoginUser()
    {

        if (VivoxManager.Instance.FilterChannelAndUserName(usernameInput.text))
        {
            VivoxManager.Instance.Login(usernameInput.text);
        }
    }



    #endregion

    #region Authentication system

    public static bool VerifyLogin(string username, string password, string filepath)
    {
        string[] lines = File.ReadAllLines(@filepath);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] field = lines[i].Split(',');
            if (field[0].Equals(username)&&field[1].Equals(password))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

}
