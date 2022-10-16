using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class EasySIP : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Gets valid Vivox SIP address
    public static string GetUserSIP(string issuer, string userName, string domain)
    {
        string result = $"sip:.{issuer}.{userName}.@{domain}";
        return result;
    }

    // Gets valid Vivox Channel SIP address
    public static string GetChannelSip(IChannelSession channelSession,
        Channel3DProperties channel3DProperties = default)
    {
        string channelToken = "Channel Token Invalid";
        string issuer = channelSession.Channel.Issuer;
        string channelName = channelSession.Channel.Name;
        string domain = channelSession.Channel.Domain;
        ChannelType channelType = channelSession.Channel.Type;
        switch (channelType)
        {
            case ChannelType.Echo:
                channelToken = $"sip:confctl-e-{issuer}.{channelName}@{domain}";
                break;
            case ChannelType.NonPositional:
                channelToken = $"sip:confctl-g-{issuer}.{channelName}@{domain}";
                break;
            case ChannelType.Positional:
                if (channel3DProperties == null)
                {
                    channel3DProperties = new Channel3DProperties();
                }

                channelToken = $"sip:confctl-d-{issuer}.{channelName}{channel3DProperties}@{domain}";
                break;
        }

        return channelToken;
    }

  
    public static string GetChannelSIP(ChannelType channelType, string issuer, string channelName, string domain,
        string region, string hash,
        Channel3DProperties channel3DProperties = default)
    {
        string channelToken = "Channel Token Invalid";
        switch (channelType)
        {
            case ChannelType.Echo:
                channelToken = $"sip:confctl-e-{issuer}.{region}.{hash}-{channelName}@{domain}";
                break;
            case ChannelType.NonPositional:
                channelToken = $"sip:confctl-g-{issuer}.{region}.{hash}-{channelName}@{domain}";
                break;
            case ChannelType.Positional:
                if (channel3DProperties == null)
                {
                    channel3DProperties = new Channel3DProperties();
                }

                channelToken = $"sip:confctl-d-{issuer}.{region}.{hash}-{channelName}{channel3DProperties}@{domain}";
                break;
        }

        return channelToken;
    }

  
    public static string GetChannelSIP(ChannelType channelType, string issuer, string channelName, string domain,
        Channel3DProperties channel3DProperties = default)
    {
        string channelToken = "Channel Token Invalid";
        switch (channelType)
        {
            case ChannelType.Echo:
                channelToken = $"sip:confctl-e-{issuer}.{channelName}@{domain}";
                break;
            case ChannelType.NonPositional:
                channelToken = $"sip:confctl-g-{issuer}.{channelName}@{domain}";
                break;
            case ChannelType.Positional:
                if (channel3DProperties == null)
                {
                    channel3DProperties = new Channel3DProperties();
                }

                channelToken = $"sip:confctl-d-{issuer}.{channelName}{channel3DProperties}@{domain}";
                break;
        }

        return channelToken;
    }
}

