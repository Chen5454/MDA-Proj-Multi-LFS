using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class WorldCanvasInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PhotonView _playerView;
    private PlayerController _playerController;

    [SerializeField] private float _distanceFromPlayer = 2f;

    private void SetPlayerViewAndController()
    {
        if (!_playerView)
        {
            for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
            {
                if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
                {
                    _playerView = ActionsManager.Instance.AllPlayersPhotonViews[i];

                    if (!_playerController)
                        _playerController = _playerView.GetComponent<PlayerController>();

                    break;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetPlayerViewAndController();

        if (_playerView.IsMine)
        {
            Debug.Log("My Pointer Enter");

            if (Vector3.Distance(transform.position, _playerView.transform.position) < _distanceFromPlayer)
            {
                _playerController.ChangeToUseUIState(true);
            }
        }
        else
        {
            Debug.Log("Any Pointer Enter");
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SetPlayerViewAndController();

        if (_playerView.IsMine)
        {
            Debug.Log("My Pointer Exit");
            _playerController.ChangeToUseUIState(false);
        }
        else
        {
            Debug.Log("Any Pointer Exit");
        }
    }
    public void OnCloseWindow()
    {
        SetPlayerViewAndController();

        if (_playerView.IsMine)
        {
            Debug.Log("My Pointer Exit");
            _playerController.ChangeToUseUIState(false);
        }
    }
}
