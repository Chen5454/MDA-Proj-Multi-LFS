﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RevealPropOnPatient : Action
{
    [Header("Prefab References")]
    [SerializeField] private Props _prop;

    [Header("Log Text")]
    [SerializeField] private string _logText;

    public void RevealOnPatient()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            CurrentPatient.PhotonView.RPC("RevealPropOnPatientRPC", RpcTarget.AllViaServer, (int)_prop);
            //CurrentPatient.PropList[(int)_prop].SetActive(true);

            TextToLog = $"{_logText}";

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
