using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeartMassages : Action
{
    [Header("Component's Data")]
    [SerializeField] private GameObject _heartMassagesWindow;
    [SerializeField] private int _newHeartRate;

    private PlayerController _playerController;
    private Animator _playerAnimator;

    public void DoHeartMassage()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            _playerAnimator = LocalPlayerData.gameObject.transform.GetChild(5).GetComponent<Animator>();

            LocalPlayerData.transform.SetPositionAndRotation(PatientChestPosPlayerTransform.position, PatientChestPosPlayerTransform.rotation);

            _playerAnimator.SetBool("Administering Cpr", true);
            CurrentPatient.PhotonView.RPC("ChangeHeartRateRPC", RpcTarget.All, _newHeartRate);

            _playerController = LocalPlayerData.GetComponent<PlayerController>();
            _playerController.ChangeToTreatingState(true);

            _heartMassagesWindow.SetActive(true);

            TextToLog = $"התחיל במתן עיסוי לב";

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }

    public void StopHeartMassages()
    {
        TextToLog = $"הפסיק במתן עיסוי לב";
        _playerAnimator.SetBool("Administering Cpr", false);
        _heartMassagesWindow.SetActive(false);

        _playerController.ChangeToTreatingState(false);

        if (_shouldUpdateLog)
        {
            LogText(TextToLog);
        }
    }
}
