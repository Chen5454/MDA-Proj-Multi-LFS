using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlaceBandage : Action
{
    // need to turn off Action Panel from editor

    //[SerializeField] private GameObject _bagWindow;
    [SerializeField] private bool _useTourniquetInstead = false;
    private CameraController _camController;
    [SerializeField] private LayerMask _bandageLayer;
    private bool _useSelectableLayer;

    private void Update()
    {
        if (_useSelectableLayer && CurrentPatient != null)
        {
            ChooseBandage();
        }
    }

    public void PlaceBandageAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            if (_useTourniquetInstead == true)
            {
                TextToLog = "ביצע חוסם עורקים למטופל";
            }
            else
            {
                TextToLog = "חבש את המטופל";
            }

            CurrentPatient.PhotonView.RPC("PlaceBandageAction_RPC", RpcTarget.AllViaServer, _useTourniquetInstead);
            SwitchRayCastTarget(false);

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    base.OnPlayerEnteredRoom(newPlayer);

    //    if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
    //        CurrentPatient.PhotonView.RPC("PlaceBandageAction_RPC", newPlayer, _useTourniquetInstead);

    //}

    private void SwitchRayCastTarget(bool useInteractable)
    {
        _useSelectableLayer = !useInteractable;
        _camController = LocalPlayerData.GetComponent<CameraController>();
        _camController.ToggleInteractRaycast(useInteractable);
    }

    private void ChooseBandage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camController.PlayerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit bandageRaycastHit, 20f, _bandageLayer))
            {
                bandageRaycastHit.transform.GetComponent<MakeItAButton>().EventToCall.Invoke();
                SwitchRayCastTarget(true);
                Debug.Log($"Chose {bandageRaycastHit.transform.name}");
            }
        }
    }
    //private void CloseBagWindow()
    //{
    //    if (_bagWindow)
    //        _bagWindow.SetActive(false);
    //}
}
