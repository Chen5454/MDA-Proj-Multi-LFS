using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using VivoxUnity;

public class VoipSystem : MonoBehaviour
{
    public PhotonView PhotonView => gameObject.GetPhotonView();

    private string channelName;
    private bool positionalChannelExists;
    private bool isMute;
    [SerializeField] private GameObject muteIcon;
    [SerializeField] private GameObject umuteIcon;

    
    void Start()
    {

//#if UNITY_EDITOR
        if (VivoxManager.Instance.Lobby.ConnectAsPikud10.isOn)
        {
            VivoxManager.Instance.VivoxJoin3DPositional(VivoxManager.Instance.vivox.Channel3DName, true, false, false,
                ChannelType.Positional, 10, 5, 5, AudioFadeModel.InverseByDistance);
            VivoxManager.Instance.Join2DChannel(VivoxManager.Instance.vivox.Channel2DName, true, false, false,
                ChannelType.NonPositional);
        }

//#else
        else
        {
            VivoxManager.Instance.VivoxJoin3DPositional(VivoxManager.Instance.vivox.Channel3DName, true, false, true, ChannelType.Positional, 10, 5, 5, AudioFadeModel.InverseByDistance);

            VivoxManager.Instance.Join2DChannel(VivoxManager.Instance.vivox.Channel2DName, true, false, false, ChannelType.NonPositional);
        }

        //#endif
        muteIcon = UIManager.Instance.muteIcon;
        umuteIcon = UIManager.Instance.umuteIcon;
        StartCoroutine(Handle3DVoipPositionUpdate(0.3f));
    }


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //    if (VivoxManager.Instance.vivox.channelSession.Channel.Type == ChannelType.Positional)
        //        LocalMuteSelf(VivoxManager.Instance.vivox.client);


        //if (Input.GetKeyDown(KeyCode.N))
        //    if (VivoxManager.Instance.vivox.channelSession.Channel.Type == ChannelType.Positional)
        //        LocalUnmuteSelf(VivoxManager.Instance.vivox.client);
        if (PhotonView.IsMine)
        {
            MuteUnmuteToggle();
        }
    }

    IEnumerator Handle3DVoipPositionUpdate(float nextUpdate)
    {
        yield return new WaitForSeconds(nextUpdate);

            if (VivoxManager.Instance.vivox.loginSession.State == LoginState.LoggedIn)
            {
                if (positionalChannelExists)
                {
                    VivoxManager.Instance.vivox.channelSession.Set3DPosition(transform.position, transform.position,
                        transform.forward, transform.up);

                }
                else
                {
                    positionalChannelExists = CheckIfChannelExists();
                }
            }
            StartCoroutine(Handle3DVoipPositionUpdate(nextUpdate));

        


    }

    
  

    public bool CheckIfChannelExists()
    {

        if (VivoxManager.Instance.vivox.channelSession.Channel.Type == ChannelType.Positional)
        {
            channelName = VivoxManager.Instance.vivox.channelSession.Channel.Name;
            if (VivoxManager.Instance.vivox.channelSession.ChannelState == ConnectionState.Connected)
            {
                Debug.Log($"Channel : {channelName} is connected");
                if (VivoxManager.Instance.vivox.channelSession.AudioState == ConnectionState.Connected)
                {
                    Debug.Log($"Audio is Connected in Channel : {channelName}");
                    return true;
                }

                Debug.Log($"Audio is Connected in Channel : {channelName}");
            }
            else
            {
                Debug.Log($"Channel : {channelName} is not Connected");
            }
        }

        return false;
    }



    public void MuteUnmuteToggle()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isMute)
            {
                LocalMuteSelf(VivoxManager.Instance.vivox.client);
                muteIcon.SetActive(true);
                umuteIcon.SetActive(false);
            }
            else
            {
                LocalUnmuteSelf(VivoxManager.Instance.vivox.client);
                muteIcon.SetActive(false);
                umuteIcon.SetActive(true);

            }
            isMute = !isMute;

        }

    }





    //Mute/Unmute Section

    public void LocalMuteSelf(VivoxUnity.Client client)
    {
        client.AudioInputDevices.Muted = true;
    }

    public void LocalUnmuteSelf(VivoxUnity.Client client)
    {
        client.AudioInputDevices.Muted = false;
    }

}