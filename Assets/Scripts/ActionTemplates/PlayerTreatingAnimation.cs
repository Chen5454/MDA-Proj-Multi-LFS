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

    public void PlayAnimation()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            _playerAnimator = LocalPlayerData.gameObject.transform.GetChild(5).GetComponent<Animator>();

            int playerTreatingPos = (int)_playerTreatingPos;
            LocalPlayerData.transform.SetPositionAndRotation(PlayerTreatingPositions[playerTreatingPos].position, new Quaternion(LocalPlayerPhotonView.transform.rotation.x, PlayerTreatingPositions[playerTreatingPos].rotation.y, LocalPlayerPhotonView.transform.rotation.z, LocalPlayerPhotonView.transform.rotation.w));

            _playerAnimator.SetBool(_animationName, true);

            TextToLog = _logText;

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
