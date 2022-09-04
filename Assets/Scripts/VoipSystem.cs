using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VoipSystem : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Handle3DVoipPositionUpdate(0.3f));
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            LocalMuteSelf(LoginCredentials.Instance.client);
        if (Input.GetKeyDown(KeyCode.N))
            LocalUnmuteSelf(LoginCredentials.Instance.client);
    }

    IEnumerator Handle3DVoipPositionUpdate(float nextUpdate)
    {
        yield return new WaitForSeconds(nextUpdate);
        LoginCredentials.Instance.GetChannelSession.Set3DPosition(transform.position, transform.position, transform.forward, transform.up);

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
