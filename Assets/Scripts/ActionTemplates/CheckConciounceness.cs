using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckConciounceness : Action
{
    [Header("Component's Data")]
    [SerializeField] private string _caseConscious;
    [SerializeField] private string _caseNotConscious;

    [Header("Alert")]
    [SerializeField] private string _alertTitle;

    [Header("Conditions")]
    [SerializeField] private bool _showAlert = false;
    [SerializeField] private bool _updateLog = true;

    private string _consciousnessState;

    public void CheckConciouncenessAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            _consciousnessState = CurrentPatientData.IsConscious ? _caseConscious : _caseNotConscious;

            TextToLog = $"is {_consciousnessState}";

            if (_showAlert)
            {
                ShowTextAlert(_alertTitle, _consciousnessState);
            }

            if (_updateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
