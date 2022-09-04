using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using TMPro;


public class LoginCredentials : MonoBehaviour
{
    public static LoginCredentials Instance;

    public Client client;
    private Uri server = new Uri("https://mt1s.www.vivox.com/api2");
    private string issuer = "mda0741-md03-dev";
    private string domain = "mt1s.vivox.com";
    private string tokeKey = "java050";
   // private TimeSpan timeSpan = new TimeSpan(90);
    private TimeSpan timeSpan = TimeSpan.FromSeconds(90);

    private ILoginSession loginSession;
    private IChannelSession channelSession;
    public IChannelSession GetChannelSession => channelSession;

    [SerializeField] private string _channelName;
    public TMP_InputField usernameInput;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        client = new Client();
        client.Uninitialize();
        client.Initialize();
       
    }

    private void OnApplicationQuit()
    {
        LeaveChannelClick();
        client.Uninitialize(); // closes all the net to the servers
    }

    public void BindLoginCallBack(bool bind,ILoginSession loginSesh)
    {
        if (bind)
        {
            loginSesh.PropertyChanged += LoginStatus;
        }
        else
        {
            loginSesh.PropertyChanged -= LoginStatus;
        }
    }

    

    #region Login Methods
    public void Login(string userName)
    {
        AccountId accountId = new AccountId(issuer, userName, domain);
        loginSession = client.GetLoginSession(accountId);
        BindLoginCallBack(true,loginSession);
        loginSession.BeginLogin(server, loginSession.GetLoginToken(tokeKey, timeSpan), ar=>
        {
            try
            {
                loginSession.EndLogin(ar);
            }

            catch (Exception e)
            {
                BindLoginCallBack(false,loginSession);
                Debug.Log(e.Message);
            }
            // 
        });

    }
    public void LoginUser()
    {
        Login(usernameInput.text);
    }

    public void Logout()
    {
        loginSession.Logout();
        BindLoginCallBack(false,loginSession);
    }

    public void LoginStatus(object sender, PropertyChangedEventArgs loginArgs)
    {
        ILoginSession source = (ILoginSession)sender;

        switch (source.State)
        {
            case LoginState.LoggingIn:
                Debug.Log("Logging....");
                break;

            case LoginState.LoggedIn:
                Debug.Log($"Logged In{loginSession.LoginSessionId.Name}");
                JoinChannelClicked();
                break;



        }
    }

    #endregion

    #region Join Channle Methods

    public void JoinChannel(string channelName,bool IsAudio,bool IsText,bool switchTransmission,ChannelType channelType)
    {
        ChannelId channelId = new ChannelId(issuer, channelName,domain, channelType);
        channelSession = loginSession.GetChannelSession(channelId);
        BindChannelCallBackListener(true,channelSession);

        if (IsAudio)
        {
            channelSession.PropertyChanged += OnAudioStateChanged;
        }
        
        channelSession.BeginConnect(IsAudio, IsText, switchTransmission, channelSession.GetConnectToken(tokeKey, timeSpan), ar =>
        {
            try
            {
                channelSession.EndConnect(ar);
            }
            catch (Exception e)
            {
                BindChannelCallBackListener(false,channelSession);
                if (IsAudio)
                {
                    channelSession.PropertyChanged -= OnAudioStateChanged;
                }
                Debug.Log(e);
            }
        });
    }


    public void JoinChannelClicked()
    {
        JoinChannel(_channelName,true,false,true,ChannelType.Positional);
    }

    public void LeaveChannel(IChannelSession channelToDisconnect,string channelName)
    {
        channelToDisconnect.Disconnect();
        loginSession.DeleteChannelSession(new ChannelId(issuer, channelName,domain));

    }

    public void LeaveChannelClick()
    {
        LeaveChannel(channelSession, _channelName);
    }

    public void BindChannelCallBackListener(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.PropertyChanged += OnChannelStatusChanged;
        }
        else
        {
            channelSesh.PropertyChanged -= OnChannelStatusChanged;

        }
    }


    public void OnChannelStatusChanged(object sender,PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.ChannelState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Channel Connecting...");
                break;
            case ConnectionState.Connected:
                Debug.Log($"{source.Channel.Name}Channel Connected");
                _channelName = source.Channel.Name;

                break;
            case ConnectionState.Disconnecting:
                Debug.Log($"{source.Channel.Name}Channel Disconnecting...");

                break;
            case ConnectionState.Disconnected:
                Debug.Log($"{source.Channel.Name}Channel Disconnected");

                break;
        }

    }

    public void OnAudioStateChanged(object sender, PropertyChangedEventArgs AudioArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.AudioState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Audio Channel Connecting...");
                break;
            case ConnectionState.Connected:
                Debug.Log("Audio Channel Connected");

                break;
            case ConnectionState.Disconnecting:
                Debug.Log("Audio Channel Disconnecting...");

                break;
            case ConnectionState.Disconnected:
                Debug.Log("Audio Channel Disconnected");

                break;
        }
    }


    #endregion

}
