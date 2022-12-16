using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CheckMeasurement : Action
{
    [SerializeField] private bool _showAlert = false, _writeMeasurements = false;
    
    [Header("Component's Data")]
    [SerializeField] private Measurements _measurement;
    [SerializeField] private string _measurementName, _measurementNameForAlert;
    [SerializeField] private TextMeshProUGUI _textInput;


    private int _currentMeasurement;

    public void CheckMeasurementAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            int measurementNum = (int)_measurement;
            //_currentMeasurement = CurrentPatientData.GetMeasurement(measurementNum);
            _currentMeasurement = int.Parse(CurrentPatientData.GetMeasurement(measurementNum));

            TextToLog = $"Checked Patient's {_measurementName}, it is {_currentMeasurement}";

            if (_writeMeasurements)
            {
                _textInput.text = _currentMeasurement.ToString();
            }

            if (_showAlert)
            {
                ShowNumAlert(_measurementNameForAlert, _currentMeasurement);
            }

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
