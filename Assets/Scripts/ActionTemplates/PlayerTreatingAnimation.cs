using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTreatingAnimation : Action
{
    [Header("Data")]
    [SerializeField] private string _animationName;
    [SerializeField] private float _animationEndTime;

    [Header("Player Position")]
    [SerializeField] PlayerTreatingPosition _playerTreatingPos;

    [Header("Log")]
    [SerializeField] private string _logText;

    private Animator _playerAnimator;
    private string _playerName;

    private void Start()
    {
        PlayerTreatingPositions.Add(PatientHeadPosPlayerTransform);
        PlayerTreatingPositions.Add(PatientChestPosPlayerTransform);
        PlayerTreatingPositions.Add(PatientLegPosPlayerTrasform);

        EquipmentPositions.Add(PatientHeadPosEquipmentTransform);
        EquipmentPositions.Add(PatientChestPosEquipmentTransform);
    }

    public void PlayAnimation()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            _playerAnimator = LocalPlayerData.gameObject.transform.GetChild(5).GetComponent<Animator>();

            int playerTreatingPos = (int)_playerTreatingPos;
            LocalPlayerData.transform.SetPositionAndRotation(PlayerTreatingPositions[playerTreatingPos].position, PlayerTreatingPositions[playerTreatingPos].rotation);

            _playerAnimator.SetBool(_animationName, true);

            TextToLog = _logText;

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
