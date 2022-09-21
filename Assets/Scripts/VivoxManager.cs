using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using System.Reflection;
using TMPro;


public class VivoxManager : MonoBehaviour
{
    //public static VivoxManager Instance;

    public VivoxBaseData vivox = new VivoxBaseData();
    public Lobby lobbyUi;

    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Instance = this;

        vivox.client = new Client();
        vivox.client.Uninitialize();
        vivox.client.Initialize();
        DontDestroyOnLoad(this);


    }

    private void Start()
    {
        lobbyUi = GameObject.FindObjectOfType<Lobby>();
    }

    private void OnApplicationQuit()
    {
        // LeaveChannelClick();
        LeaveChannel(vivox.channelSession, lobbyUi._channelName);
        lobbyUi.Logout();
        vivox.client.Uninitialize(); // closes all the net to the servers
    }

    #region BindCallBacks

    public void BindLoginCallBack(bool bind, ILoginSession loginSesh)
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
    public void BindChannelCallBackListener2D(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.PropertyChanged += OnChannelStatusChanged2D;
        }
        else
        {
            channelSesh.PropertyChanged -= OnChannelStatusChanged2D;

        }
    }
    public void BindUserCallBacks(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.Participants.AfterKeyAdded += OnPraticipantAdded;
            channelSesh.Participants.BeforeKeyRemoved += OnPraticipantRemoved;
            channelSesh.Participants.AfterValueUpdated += OnPraticipantUpdated;

        }
        else
        {
            channelSesh.Participants.AfterKeyAdded -= OnPraticipantAdded;
            channelSesh.Participants.BeforeKeyRemoved -= OnPraticipantRemoved;
            channelSesh.Participants.AfterValueUpdated -= OnPraticipantUpdated;
        }
    }

    #endregion

    #region Login Methods
    public void Login(string userName)
    {
        AccountId accountId = new AccountId(vivox.issuer, userName, vivox.domain);
        vivox.loginSession = vivox.client.GetLoginSession(accountId);
        BindLoginCallBack(true, vivox.loginSession);
        vivox.loginSession.BeginLogin(vivox.server, vivox.loginSession.GetLoginToken(vivox.tokeKey, vivox.timeSpan), ar =>
        {
            try
            {
                vivox.loginSession.EndLogin(ar);
            }

            catch (Exception e)
            {
                BindLoginCallBack(false, vivox.loginSession);
                Debug.Log(e.Message);
            }
            // 
        });

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
                Debug.Log($"Logged In{vivox.loginSession.LoginSessionId.Name}");
                //lobbyUi.VivoxusernameInput.text = vivox.loginSession.LoginSessionId.Name;
                // VivoxJoin3DPositional(lobbyUi._channelName, true, false, true, ChannelType.Positional, 10, 5, 5, AudioFadeModel.InverseByDistance);
                //vivox.loginSession.SetTransmissionMode(TransmissionMode.All);

                break;
        }
    }

    #endregion
    #region Join Channle Methods

    public void Join3DChannel(string channelName, bool IsAudio, bool IsText, bool switchTransmission,
        ChannelType channelType, Channel3DProperties channel3DProperties)
    {
        ChannelId channelId = new ChannelId(vivox.issuer, channelName, vivox.domain, channelType, channel3DProperties);
        vivox.channelSession = vivox.loginSession.GetChannelSession(channelId);
        BindChannelCallBackListener(true, vivox.channelSession);
        BindUserCallBacks(true, vivox.channelSession);

        if (IsAudio)
        {
            vivox.channelSession.PropertyChanged += OnAudioStateChanged;
        }

        if (IsText)
        {
            vivox.channelSession.PropertyChanged += OnTextStateChanged;
        }

        vivox.channelSession.BeginConnect(IsAudio, IsText, switchTransmission, vivox.channelSession.GetConnectToken(vivox.tokeKey, vivox.timeSpan), ar =>
        {
            try
            {
                vivox.channelSession.EndConnect(ar);
            }
            catch (Exception e)
            {
                BindChannelCallBackListener(false, vivox.channelSession);
                BindUserCallBacks(false, vivox.channelSession);

                if (IsAudio)
                {
                    vivox.channelSession.PropertyChanged -= OnAudioStateChanged;
                }

                if (IsText)
                {
                    vivox.channelSession.PropertyChanged -= OnTextStateChanged;
                }

                Debug.Log(e);
            }
        });
    }


    public void Join2DChannel(string channelName, bool IsAudio, bool IsText, bool switchTransmission)
    {
        ChannelId channelId = new ChannelId(vivox.issuer, channelName, vivox.domain);
        vivox.channelSession2 = vivox.loginSession.GetChannelSession(channelId);
        BindChannelCallBackListener2D(true, vivox.channelSession2);
        BindUserCallBacks(true, vivox.channelSession2);

        if (IsAudio)
        {
            vivox.channelSession2.PropertyChanged += OnAudioStateChanged2D;
        }
        vivox.channelSession2.BeginConnect(IsAudio, IsText, switchTransmission, vivox.channelSession2.GetConnectToken(vivox.tokeKey, vivox.timeSpan), ar =>
        {
            try
            {
                vivox.channelSession2.EndConnect(ar);
            }
            catch (Exception e)
            {
                BindChannelCallBackListener2D(false, vivox.channelSession2);
                BindUserCallBacks(false, vivox.channelSession2);

                if (IsAudio)
                {
                    vivox.channelSession2.PropertyChanged -= OnAudioStateChanged2D;
                }
                Debug.Log(e);
            }
        });
    }



    public void LeaveChannel(IChannelSession channelToDisconnect, string channelName)
    {
        channelToDisconnect.Disconnect();
        vivox.loginSession.DeleteChannelSession(new ChannelId(vivox.issuer, channelName, vivox.domain));

    }


    #endregion

    #region UserCallbacks

    public void OnPraticipantAdded(object sender, KeyEventArg<string> participantArg)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        var user = source[participantArg.Key];

        Debug.Log($"{user.Account.Name} has join the channel");
    }
    public void OnPraticipantRemoved(object sender, KeyEventArg<string> participantArg)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        var user = source[participantArg.Key];

        Debug.Log($"{user.Account.Name} has left the channel");

    }
    public void OnPraticipantUpdated(object sender, ValueEventArg<string, IParticipant> participantArg)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        var user = source[participantArg.Key];
    }
    #endregion

    #region StatesChanges

    public void OnChannelStatusChanged(object sender, PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        if (channelArgs.PropertyName == "ChannelState")
        {
            switch (source.ChannelState)
            {
                case ConnectionState.Connecting:
                    Debug.Log("Channel Connecting...");
                    break;
                case ConnectionState.Connected:
                    Debug.Log($"{source.Channel.Name}Channel Connected");

                    break;
                case ConnectionState.Disconnecting:
                    Debug.Log($"{source.Channel.Name}Channel Disconnecting...");

                    break;
                case ConnectionState.Disconnected:
                    Debug.Log($"{source.Channel.Name}Channel Disconnected");
                    BindChannelCallBackListener(false, vivox.channelSession);
                    BindUserCallBacks(false, vivox.channelSession);

                    break;
            }
        }

    }

    public void OnChannelStatusChanged2D(object sender, PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.ChannelState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Channel2D Connecting...");
                break;
            case ConnectionState.Connected:
                Debug.Log($"{source.Channel.Name}Channel2D Connected");

                break;
            case ConnectionState.Disconnecting:
                Debug.Log($"{source.Channel.Name}Channel2D Disconnecting...");

                break;
            case ConnectionState.Disconnected:
                Debug.Log($"{source.Channel.Name}Channel2D Disconnected");

                break;
        }

    }

    public void OnAudioStateChanged(object sender, PropertyChangedEventArgs AudioArgs)
    {
        IChannelSession source = (IChannelSession)sender;
        if (AudioArgs.PropertyName == "AudioState")
        {
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
                    vivox.channelSession.PropertyChanged -= OnAudioStateChanged;

                    break;
            }
        }

    }

    public void OnTextStateChanged(object sender, PropertyChangedEventArgs TextArgs)
    {
        IChannelSession source = (IChannelSession)sender;
        if (TextArgs.PropertyName == "TextState")
        {
            switch (source.TextState)
            {
                case ConnectionState.Connecting:
                    Debug.Log("Text Channel Connecting...");
                    break;
                case ConnectionState.Connected:
                    Debug.Log("Text Channel Connected");

                    break;
                case ConnectionState.Disconnecting:
                    Debug.Log("Text Channel Disconnecting...");

                    break;
                case ConnectionState.Disconnected:
                    Debug.Log("Text Channel Disconnected");
                    vivox.channelSession.PropertyChanged -= OnTextStateChanged;
                    break;
            }
        }

    }

    public void OnAudioStateChanged2D(object sender, PropertyChangedEventArgs AudioArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.AudioState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Audio Channel2D Connecting...");
                break;
            case ConnectionState.Connected:
                Debug.Log("Audio Channel2D Connected");

                break;
            case ConnectionState.Disconnecting:
                Debug.Log("Audio Channel2D Disconnecting...");

                break;
            case ConnectionState.Disconnected:
                Debug.Log("Audio Channel2D Disconnected");
                //if (isPikud10)
                //{
                //    LoginCredentials.Instance.VivoxJoinChannel(LoginCredentials.Instance._channelName2, true, false,
                //        true, ChannelType.NonPositional);
                //    LoginCredentials.Instance.loginSession.SetTransmissionMode(TransmissionMode.All);
                //}

                break;
        }
    }
    #endregion


    public bool FilterChannelAndUserName(string nameToFilter)
    {
        char[] allowedChars = new char[] { '0','1','2','3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n','o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I','J', 'K', 'L', 'M', 'N', 'O', 'P','Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '!', '(', ')', '+','-', '.', '=', '_', '~'};
        List<char> allowed = new List<char>(allowedChars);
        foreach (char c in nameToFilter)
        {
            if (!allowed.Contains(c))
            {
                OnSendLog(MethodBase.GetCurrentMethod(), $"Can't join channel, Channel name has invalid character '{c}'");
                return false;
            }
        }

        return true;
    }

    private void OnSendLog(MethodBase methodBase, string v)
    {
        Debug.Log("OnSendLog");
    }

    public void VivoxJoinChannel(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel)
    {
        if (FilterChannelAndUserName(channelName))
        {
            Join2DChannel(channelName, includeVoice, includeText, switchToThisChannel);
        }
        else
        {
            Debug.Log("error");
        }
    }

    public void VivoxJoin3DPositional(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel, ChannelType channelType,
        int maxHearingDistance, int minHearingDistance, float voiceFadeOutOverDistance, AudioFadeModel audioFadeModel)
    {
        //this.DebugLog($"{loginSession.Presence.Status} {loginSession.Presence.Message}");
        Channel3DProperties channel3DProperties = new Channel3DProperties(maxHearingDistance, minHearingDistance, voiceFadeOutOverDistance, audioFadeModel);
        Join3DChannel(channelName, includeVoice, includeText, switchToThisChannel, channelType, channel3DProperties);
    }




}