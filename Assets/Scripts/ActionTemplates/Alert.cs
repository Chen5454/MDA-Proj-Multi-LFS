using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Alert : Action
{
    [SerializeField] private bool _isAlertNum = false;

    [Header("Alert's Content")]
    [SerializeField] private string _alertTitle;
    [SerializeField] private string _alertText;

    [Header("Log's Content")]
    [SerializeField] private string _textToLog;

    public void ShowAlert()
    {
        GetActionData();

        TextToLog = _textToLog;

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            if (!_isAlertNum)
            {
                ShowTextAlert(_alertTitle, _alertText);
            }
            else
            {
                ShowNumAlert(_alertTitle, int.Parse(_alertText));
            }

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
