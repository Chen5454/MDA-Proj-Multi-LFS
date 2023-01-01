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
    [SerializeField] private Measurements _measurement; //TBF ALON this needs to be a full list of all measurements - as measurementDelta (fields that need not change are left blank, and are to be ignored)

    public void ChangeMeasurementAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            int measurementNum = (int)_measurement;
            CurrentPatient.PhotonView.RPC("SetMeasurementByIndexRPC", RpcTarget.All, measurementNum, _newMeasurement);

            if (_useMedicineLog)
            {
                TextToLog = $" המטופל לקח וצרך:  {_treatmentName}";
            }
            else
            {
                TextToLog = $"ביצע {_treatmentName} על המטופל";
            }

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
