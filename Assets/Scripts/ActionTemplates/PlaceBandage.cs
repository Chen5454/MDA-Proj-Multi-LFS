using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBandage : Action
{
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

        TextToLog = "Placed Bandage on Patient";

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            SwitchRayCastTarget(false);
            CurrentPatient.UseTourniquet = _useTourniquetInstead;
            CurrentPatient.SetUnusedBandages(true);

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }

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
}
