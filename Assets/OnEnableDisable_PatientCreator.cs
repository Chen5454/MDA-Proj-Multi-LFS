using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableDisable_PatientCreator : MonoBehaviour
{
    private PhotonView _playerView;
    private PlayerController _playerController;
    bool inited =false;
    void OnEnable()
    {
        if (!inited)
            return;
        if (!_playerView)
        {
            for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
            {
                if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
                {
                    _playerView = ActionsManager.Instance.AllPlayersPhotonViews[i];
                    _playerController = _playerView.GetComponent<PlayerController>();
                    break;
                }
            }
        }
        if (!_playerController)
        {
            Debug.LogError("NO PLAYER CONTROLLER FOUND!");
            return;
        }
        _playerController.ChangeToUseUIState(true);
    }

    private void Start()
    {
        UIManager.Instance.PatientCreationWindow = gameObject;
        inited = true;
        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        if(inited)
        _playerController.ChangeToUseUIState(false);
    }
}
