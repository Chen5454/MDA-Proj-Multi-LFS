using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VivoxBaseData 
{

    public Client client;
    public Uri server = new Uri("https://mt1s.www.vivox.com/api2");
    public string issuer = "mda0741-md03-dev";
    public string domain = "mt1s.vivox.com";
    public string tokeKey = "java050";
    // private TimeSpan timeSpan = new TimeSpan(90);
    public TimeSpan timeSpan = TimeSpan.FromSeconds(90);

    // public bool isPikud10;

    public ILoginSession loginSession;
    public IChannelSession channelSession;
    public IChannelSession channelSession2;
   
}
