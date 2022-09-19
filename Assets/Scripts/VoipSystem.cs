using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VoipSystem : MonoBehaviour
{
    public VivoxManager _VivoxManager;


    void Start()
    {
        _VivoxManager = GameObject.FindObjectOfType<VivoxManager>();

        _VivoxManager.lobbyUi.JoinChannelClicked();
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
        _VivoxManager.vivox.channelSession.Set3DPosition(transform.position, transform.position, transform.forward, transform.up);

        StartCoroutine(Handle3DVoipPositionUpdate(nextUpdate));

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