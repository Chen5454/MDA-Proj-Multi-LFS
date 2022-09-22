using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VoipSystem : MonoBehaviour
{
    public VivoxManager _VivoxManager;

    private string channelName;
    private bool positionalChannelExists;
    void Start()
    {
        _VivoxManager = GameObject.FindObjectOfType<VivoxManager>();

        _VivoxManager.VivoxJoin3DPositional(_VivoxManager.vivox.Channel3DName , true, false, true, ChannelType.Positional, 10, 5, 5, AudioFadeModel.InverseByDistance);

        _VivoxManager.vivox.loginSession.SetTransmissionMode(TransmissionMode.All);

        StartCoroutine(Handle3DVoipPositionUpdate(0.3f));
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            LocalMuteSelf(_VivoxManager.vivox.client);
        if (Input.GetKeyDown(KeyCode.N))
            LocalUnmuteSelf(_VivoxManager.vivox.client);
    }

    IEnumerator Handle3DVoipPositionUpdate(float nextUpdate)
    {
        yield return new WaitForSeconds(nextUpdate);

        if (_VivoxManager.vivox.loginSession.State == LoginState.LoggedIn)
        {
            if (positionalChannelExists)
            {
                _VivoxManager.vivox.channelSession.Set3DPosition(transform.position, transform.position, transform.forward, transform.up);

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
       
            if (_VivoxManager.vivox.channelSession.Channel.Type == ChannelType.Positional)
            {
                channelName = _VivoxManager.vivox.channelSession.Channel.Name;
                if (_VivoxManager.vivox.channelSession.ChannelState == ConnectionState.Connected)
                {
                    Debug.Log($"Channel : {channelName} is connected");
                    if (_VivoxManager.vivox.channelSession.AudioState == ConnectionState.Connected)
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