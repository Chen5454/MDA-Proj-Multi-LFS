using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CalmPatientDown : Action
{
    [Header("Component's Data")]
    //[SerializeField] private int _calmDownHeartRateBy = 0;
    //[SerializeField] private int _calmDownRespiratoryRateBy;
    [SerializeField] private int _newHeartRate, _newRespiratoryRate;

    private int _heartRateIndex = 0, _respiratoryRate = 2;

    public void CalmPatientDownAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            CurrentPatient.PhotonView.RPC("SetMeasurementByIndexRPC", RpcTarget.All, _heartRateIndex, _newHeartRate);
            CurrentPatient.PhotonView.RPC("SetMeasurementByIndexRPC", RpcTarget.All, _respiratoryRate, _newRespiratoryRate);

            TextToLog = $"Patient calm down";

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
