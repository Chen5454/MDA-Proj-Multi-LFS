using System;
using VivoxUnity;

public interface IVivoxMute 
{
    void LocalToggleMuteRemoteUser(string userName, IChannelSession channelSession);
    void MuteAllUsers(IChannelSession channelSession);
    void UnmuteAllUsers(IChannelSession channelSession);
    void LocalMuteSelf(VivoxUnity.Client client);
    void LocalUnmuteSelf(VivoxUnity.Client client);
}
