using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Log : Action
{
    [Header("Component's Data")]
    [SerializeField] private string _textToLog;

    public void ShowAlert()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
            LogText(_textToLog);
    }
}
