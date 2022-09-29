using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VehicleInteraction : MonoBehaviour
{
    [SerializeField] private VehicleController _vehicleController;
    [SerializeField] private string _vehicleAlertTitle, _vehicleFullContent;
    [SerializeField] private int _barType;
    



    private VehicleSit _vehicleSit;

    #region Monobehaviour Callbacks
    private void Start()
    {
       
            UIManager.Instance.DriverExitBtn.onClick.RemoveAllListeners();
            UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();


            UIManager.Instance.HeadlightBtn.onClick.RemoveAllListeners();
            UIManager.Instance.SirenBtn.onClick.RemoveAllListeners();



    }
    #endregion

    
    #region Enter & Exit Vehicle
    public void EnterVehicle(int sitNum)
    {
        _vehicleSit = (VehicleSit)sitNum;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                PhotonView photonView = ActionsManager.Instance.AllPlayersPhotonViews[i];
                PlayerController playerController = photonView.GetComponent<PlayerController>();

                UIManager.Instance.CurrentActionBarParent = _barType switch
                {
                    0 => UIManager.Instance.AmbulanceBar,
                    1 => UIManager.Instance.NatanBar,
                    _ => UIManager.Instance.AmbulanceBar,
                };

                UIManager.Instance.CurrentActionBarParent.SetActive(true);

                if (UIManager.Instance.VehicleDriverUI.activeInHierarchy)
                    UIManager.Instance.VehicleDriverUI.SetActive(false);

                if (UIManager.Instance.VehiclePassangerUI.activeInHierarchy)
                    UIManager.Instance.VehiclePassangerUI.SetActive(false);

                if (_vehicleController.CollidingPlayers.Contains(photonView.gameObject))
                {
                    playerController.transform.GetChild(5).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;

                    if (_vehicleSit == VehicleSit.Driver)
                    {
                        UIManager.Instance.VehicleDriverUI.SetActive(true);
                        _vehicleController.Transfer.CarDriver();
                        playerController.CurrentVehicleController = _vehicleController;
                        playerController.IsInVehicle = true;
                        playerController.IsDriving = true;
                        UIManager.Instance.DriverExitBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.HeadlightBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.SirenBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.DriverExitBtn.onClick.AddListener(delegate { ExitVehicle(); });
                        UIManager.Instance.HeadlightBtn.onClick.AddListener(delegate { ToggleHeadlights(); }); 
                        UIManager.Instance.SirenBtn.onClick.AddListener(delegate { ToggleSiren(); });

                        photonView.transform.SetParent(_vehicleController.DriverSit);
                        photonView.transform.localPosition = Vector3.zero;
                        photonView.transform.localRotation = Quaternion.identity;
                        _vehicleController.CurrentDriverController = playerController;
                        _vehicleController.IsDriverIn = true;
                        StartCoroutine(ChangeKinematicStateCorooutine());
                        break;
                    }
                    else if (_vehicleSit == VehicleSit.Passanger)
                    {
                        UIManager.Instance.VehiclePassangerUI.SetActive(true);
                        photonView.transform.position = _vehicleController.PassangerSit.position;
                        photonView.transform.SetParent(_vehicleController.PassangerSit);
                        playerController.IsInVehicle = true;
                        playerController.IsPassanger = true;
                        _vehicleController.IsPassangerIn = true;

                        UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.PassangerExitBtn.onClick.AddListener(delegate { ExitVehicle(); });

                        break;
                    }
                    else
                    {
                        if (!_vehicleController.IsMiddleIn)
                        {
                            UIManager.Instance.VehiclePassangerUI.SetActive(true);
                            photonView.transform.position = _vehicleController.MiddleSit.position;
                            photonView.transform.SetParent(_vehicleController.MiddleSit);
                            playerController.IsInVehicle = true;
                            playerController.IsMiddleSit = true;
                            _vehicleController.IsMiddleIn = true;
                            break;
                        }
                        else if (!_vehicleController.IsLeftBackIn)
                        {
                            UIManager.Instance.VehiclePassangerUI.SetActive(true);
                            photonView.transform.position = _vehicleController.LeftBackSit.position;
                            photonView.transform.SetParent(_vehicleController.LeftBackSit);
                            playerController.IsInVehicle = true;
                            playerController.IsLeftBackSit = true;
                            _vehicleController.IsLeftBackIn = true;
                            break;
                        }
                        else if (!_vehicleController.IsRightBackIn)
                        {
                            UIManager.Instance.VehiclePassangerUI.SetActive(true);
                            photonView.transform.position = _vehicleController.RightBackSit.position;
                            photonView.transform.SetParent(_vehicleController.RightBackSit);
                            playerController.IsInVehicle = true;
                            playerController.IsRightBackSit = true;
                            _vehicleController.IsRightBackIn = true;
                            break;
                        }
                        else
                        {
                            ActionTemplates.Instance.ShowAlertWindow(_vehicleAlertTitle, _vehicleFullContent);
                        }
                    }
                }
            }
        }
    }
    public void ExitVehicle()
    {
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                PhotonView photonView = ActionsManager.Instance.AllPlayersPhotonViews[i];
                PlayerController playerController = photonView.GetComponent<PlayerController>();

                if (UIManager.Instance.VehicleDriverUI.activeInHierarchy)
                    UIManager.Instance.VehicleDriverUI.SetActive(false);

                if (UIManager.Instance.VehiclePassangerUI.activeInHierarchy)
                    UIManager.Instance.VehiclePassangerUI.SetActive(false);

                if (playerController.IsDriving)
                {
                    _vehicleController.IsDriverIn = false;
                    playerController.IsDriving = false;
                    UIManager.Instance.HeadlightBtn.onClick.RemoveListener(delegate { ToggleHeadlights(); });
                    UIManager.Instance.DriverExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });
                    UIManager.Instance.SirenBtn.onClick.RemoveListener(delegate { ToggleSiren(); });
                    _vehicleController.CurrentDriverController = null;
                    playerController.PlayerData.LastVehicleController = _vehicleController;
                    StartCoroutine(ChangeKinematicStateCorooutine());
                    photonView.transform.position = _vehicleController.DriverExit.position;
                    photonView.transform.localRotation = _vehicleController.DriverExit.rotation;
                }
            

                else if (playerController.IsPassanger)
                {
                    _vehicleController.IsPassangerIn = false;
                    playerController.IsPassanger = false;
                    photonView.transform.position = _vehicleController.PassangerExit.position;
                    photonView.transform.localRotation = _vehicleController.PassangerExit.rotation;
                    UIManager.Instance.PassangerExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });

                }
                else if (playerController.IsMiddleSit)
                {
                    _vehicleController.IsMiddleIn = false;
                    playerController.IsMiddleSit = false;
                    photonView.transform.position = _vehicleController.MiddleExit.position;
                    photonView.transform.localRotation = _vehicleController.MiddleExit.rotation;
                }
                else if (playerController.IsLeftBackSit)
                {
                    _vehicleController.IsLeftBackIn = false;
                    playerController.IsLeftBackSit = false;
                    photonView.transform.position = _vehicleController.MiddleExit.position;
                    photonView.transform.localRotation = _vehicleController.MiddleExit.rotation;
                }
                else if (playerController.IsRightBackSit)
                {
                    _vehicleController.IsRightBackIn = false;
                    playerController.IsRightBackSit = false;
                    photonView.transform.position = _vehicleController.MiddleExit.position;
                    photonView.transform.localRotation = _vehicleController.MiddleExit.rotation;
                }

                playerController.IsInVehicle = false;
                playerController.CurrentVehicleController = null;
                
                photonView.transform.SetParent(transform.root.parent);
                playerController.transform.GetChild(5).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true;
                DontDestroyOnLoad(photonView.gameObject);
            }
        }
    }
    #endregion

    #region OnClick Events
    public void OpenCloseBackDoor()
    {
        if (_vehicleController.IsBackDoorsOpen)
        {
            _vehicleController.LeftBackDoorAnimator.SetBool("IsDoorOpen", false);
            _vehicleController.RightBackDoorAnimator.SetBool("IsDoorOpen", false);
            _vehicleController.IsBackDoorsOpen = false;
            //_vehicleController.RightBackDoorAnimator.Play("Close Back Doors");
        }
        else
        {
            _vehicleController.LeftBackDoorAnimator.SetBool("IsDoorOpen", true);
            _vehicleController.RightBackDoorAnimator.SetBool("IsDoorOpen", true);
            _vehicleController.IsBackDoorsOpen = true;
            //_vehicleController.RightBackDoorAnimator.Play("Open Back Doors");
        }
    }
    public void ToggleHeadlights()
    {
        _vehicleController.PhotonView.RPC("ToggleHeadlightsRPC", RpcTarget.AllBufferedViaServer);
    }
    public void ToggleSiren()
    {
        _vehicleController.PhotonView.RPC("ToggleSirenRPC", RpcTarget.AllBufferedViaServer);
    }
    #endregion

    #region Coroutines
    private IEnumerator ChangeKinematicStateCorooutine()
    {
        yield return null;

        _vehicleController.ChangeKinematicState();
    }
    #endregion
}
