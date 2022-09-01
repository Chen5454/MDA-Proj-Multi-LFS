using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PatientTreatmentAnimation : Action
{
    [Header("Animation's Data")]
    [SerializeField] private string _animationName;
    [SerializeField] private float _animationEndTime;

    [Header("Alert")]
    [SerializeField] private string _alertTitle;
    [SerializeField] private string _alertText;

    [Header("Conditions")]
    [SerializeField] private bool _showAlert = false;
    [SerializeField] private bool _updateLog = false;

    private Animator _patientAnimator;
    private string _patientName;

    private IEnumerator WaitToFinishAnimation()
    {
        yield return new WaitForSeconds(_animationEndTime);

        _patientAnimator.SetBool(_animationName, false);
        //ActionTemplates.Instance.UpdatePatientLog(PhotonNetwork.NickName, $"{_patientName} has finished {_animationName}");
    }

    public void PlayAnimation()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            // need fixing
            _patientAnimator = CurrentPatient.GetComponent<Animator>();

            _patientAnimator.SetBool(_animationName, true);
            StartCoroutine(WaitToFinishAnimation());

            TextToLog = $" {CurrentPatientData.Name} is Reciving Heart Massages";

            if (_showAlert)
            {
                ShowTextAlert(_alertTitle, _alertText);
            }

            if (_updateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
