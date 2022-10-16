using System;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VivoxMute : IVivoxMute
{
    public VivoxBaseData vivox = new VivoxBaseData();

    public void LocalToggleMuteRemoteUser(string userName, IChannelSession channelSession)
    {
        var participants = channelSession.Participants;
        string userToMute = EasySIP.GetUserSIP(vivox.issuer, userName, vivox.domain);
        Debug.Log($"Sip address of User to mute - {userToMute}");
        if (participants[userToMute].InAudio && !participants[userToMute].IsSelf)
        {
            if (participants[userToMute].LocalMute)
            {
                participants[userToMute].LocalMute = false;
            }
            else
            {
                participants[userToMute].LocalMute = true;
            }
        }
        else
        {
            Debug.Log($"Failed to mute {participants[userToMute].Account.DisplayName}");
        }
    }



    public void MuteAllUsers(IChannelSession channelSession)
    {
        foreach (var player in channelSession.Participants)
        {
            if (player.InAudio && !player.IsSelf)
            {
                if (!player.LocalMute)
                {
                    player.LocalMute = true;
                    Debug.Log($"Muted {player.Account.DisplayName}");
                }
            }
            else
            {
                Debug.Log($"Failed to mute {player.Account.DisplayName}, Might be local player");
            }
        }
    }

    public void UnmuteAllUsers(IChannelSession channelSession)
    {
        foreach (var player in channelSession.Participants)
        {
            if (player.InAudio && !player.IsSelf)
            {
                if (player.LocalMute)
                {
                    player.LocalMute = false;
                    Debug.Log($"Unmuted {player.Account.DisplayName}");
                }
            }
            else
            {
                Debug.Log($"Failed to mute {player.Account.DisplayName}, Might be local player");
            }
        }
    }


    public async void LocalMuteSelf(VivoxUnity.Client client)
    {
        client.AudioInputDevices.Muted = true;
       
    }

    public async void LocalUnmuteSelf(VivoxUnity.Client client)
    {
        client.AudioInputDevices.Muted = false;
    }

  
}
