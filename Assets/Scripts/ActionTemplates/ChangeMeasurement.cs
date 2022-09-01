using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeMeasurement : Action
{
    [SerializeField] private bool _useMedicineLog;

    [Header("Component's Data")]
    [SerializeField] private int _newMeasurement;
    [SerializeField] private string _treatmentName;
    [SerializeField] private Measurements _measurement;

    public void ChangeMeasurementAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            int measurementNum = (int)_measurement;
            CurrentPatient.PhotonView.RPC("SetMeasurementByIndexRPC", RpcTarget.All, measurementNum, _newMeasurement);

            if (_useMedicineLog)
            {
                TextToLog = $"Patient's took & applied {_treatmentName}";
            }
            else
            {
                TextToLog = $"did {_treatmentName} on Patient";
            }

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
