using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class VehicleInteraction : MonoBehaviour
{
    [SerializeField] private VehicleController _vehicleController;
    [SerializeField] private string _vehicleAlertTitle, _vehicleSitTaken, _vehicleFullContent;
    [SerializeField] private int _barType;
    public PhotonView _PhotonView;

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
        _PhotonView.RPC("EnterVehicle_RPC", RpcTarget.AllViaServer, sitNum);
    }
    public void ExitVehicle()
    {
        _PhotonView.RPC("ExitVehicle_RPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void EnterVehicle_RPC(int sitNum)
    {

        int localPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PhotonView playerView = null;

        foreach (PhotonView pv in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            if (pv.OwnerActorNr == localPlayerActorNumber)
            {
                playerView = pv;
                break;
            }
        }

        if (playerView!=null)
        {
            _vehicleSit = (VehicleSit)sitNum;

            PlayerController playerController = playerView.GetComponent<PlayerController>();
            PlayerData playerData = playerView.GetComponent<PlayerData>();

            if (UIManager.Instance.CurrentActionBarParent.activeInHierarchy)
            {
                if (UIManager.Instance.CurrentActionBarParent != UIManager.Instance.NatanBar)
                    CloseAllCurrentBarPanels(false);
                else
                    CloseAllCurrentBarPanels(true);

                UIManager.Instance.CurrentActionBarParent.SetActive(false);
            }

            if (_barType == 0)
            {
                UIManager.Instance.CurrentActionBarParent = UIManager.Instance.AmbulanceBar;
                AmbulancePermissions ambulancePermissions = UIManager.Instance.AmbulanceBar.GetComponent<AmbulancePermissions>();
                ambulancePermissions.RemovePermissions();
                ambulancePermissions.InitializePermissions((Roles)playerData.UserRole);
                ambulancePermissions.SetActions();
            }
            else if (_barType == 1)
            {
                UIManager.Instance.CurrentActionBarParent = UIManager.Instance.NatanBar;
                NatanPermissions natanPermissions = UIManager.Instance.NatanBar.GetComponent<NatanPermissions>();
                natanPermissions.RemovePermissions();
                natanPermissions.InitializePermissions((Roles)playerData.UserRole);
                natanPermissions.SetActions();
            }

            UIManager.Instance.CurrentActionBarParent.SetActive(true);

            if (UIManager.Instance.VehicleDriverUI.activeInHierarchy)
                UIManager.Instance.VehicleDriverUI.SetActive(false);

            if (UIManager.Instance.VehiclePassangerUI.activeInHierarchy)
                UIManager.Instance.VehiclePassangerUI.SetActive(false);

            if (_vehicleController.CollidingPlayers.Contains(playerView.gameObject))
            {
                //playerController.transform.GetChild(5).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
                SliderButton sliderHeadlightBtn = UIManager.Instance.HeadlightBtn.GetComponent<SliderButton>();
                SliderButton sliderSirenBtn = UIManager.Instance.SirenBtn.GetComponent<SliderButton>();
                playerController.CurrentVehicleController = _vehicleController;

                if (_vehicleSit == VehicleSit.Driver)
                {
                    if (!_vehicleController.IsDriverIn)
                    {
                        UIManager.Instance.VehicleDriverUI.SetActive(true);
                        _vehicleController.Transfer.CarDriver();

                        //playerController.CurrentVehicleController = _vehicleController;
                        playerController.IsInVehicle = true;
                        playerController.IsDriving = true;

                        UIManager.Instance.DriverExitBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.HeadlightBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.SirenBtn.onClick.RemoveAllListeners();

                        UIManager.Instance.DriverExitBtn.onClick.AddListener(delegate { ExitVehicle(); });
                        UIManager.Instance.HeadlightBtn.onClick.AddListener(delegate { ToggleHeadlights(); });
                        UIManager.Instance.HeadlightBtn.onClick.AddListener(delegate { sliderHeadlightBtn.SliderBtnOnClick(); });
                        UIManager.Instance.SirenBtn.onClick.AddListener(delegate { ToggleSiren(); });
                        UIManager.Instance.SirenBtn.onClick.AddListener(delegate { sliderSirenBtn.SliderBtnOnClick(); });

                        _PhotonView.RPC("ChangeSit", RpcTarget.All, playerView.ViewID,(int)VehicleSit.Driver, true);
                        _vehicleController.CurrentDriverController = playerController;
                        StartCoroutine(ChangeKinematicStateCorooutine());
                        _vehicleController.IsDriverIn = true;
                    }
                    else
                    {
                        ActionTemplates.Instance.ShowAlertWindow(_vehicleAlertTitle, _vehicleSitTaken);
                    }
                }
                else if (_vehicleSit == VehicleSit.Passanger)
                {
                    if (!_vehicleController.IsPassangerIn)
                    {
                        UIManager.Instance.VehiclePassangerUI.SetActive(true);
                        _PhotonView.RPC("ChangeSit", RpcTarget.All, playerView.ViewID, (int)VehicleSit.Passanger, true);
                        playerController.IsInVehicle = true;
                        playerController.IsPassanger = true;
                        _vehicleController.IsPassangerIn = true;

                        UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();
                        UIManager.Instance.PassangerExitBtn.onClick.AddListener(delegate { ExitVehicle(); });
                    }
                    else
                    {
                        ActionTemplates.Instance.ShowAlertWindow(_vehicleAlertTitle, _vehicleFullContent);
                    }
                }
                else
                {
                    if (_vehicleSit == VehicleSit.Middle)
                    {
                        if (!_vehicleController.IsMiddleIn)
                        {
                            UIManager.Instance.VehiclePassangerUI.SetActive(true);
                            _PhotonView.RPC("ChangeSit", RpcTarget.All, playerView.ViewID, (int)VehicleSit.Middle, true);
                            playerController.IsInVehicle = true;
                            playerController.IsMiddleSit = true;
                            _vehicleController.IsMiddleIn = true;

                            UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();
                            UIManager.Instance.PassangerExitBtn.onClick.AddListener(delegate { ExitVehicle(); });
                        }
                    }
                    else if (_vehicleSit == VehicleSit.LeftBack)
                    {
                        if (!_vehicleController.IsLeftBackIn)
                        {
                            UIManager.Instance.VehiclePassangerUI.SetActive(true);
                            _PhotonView.RPC("ChangeSit", RpcTarget.All, playerView.ViewID, (int)VehicleSit.LeftBack, true);
                            playerController.IsInVehicle = true;
                            playerController.IsLeftBackSit = true;
                            _vehicleController.IsLeftBackIn = true;

                            UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();
                            UIManager.Instance.PassangerExitBtn.onClick.AddListener(delegate { ExitVehicle(); });
                        }
                    }
                    else if (_vehicleSit == VehicleSit.RightBack)
                    {
                        if (!_vehicleController.IsRightBackIn)
                        {
                            UIManager.Instance.VehiclePassangerUI.SetActive(true);
                            _PhotonView.RPC("ChangeSit", RpcTarget.All, playerView.ViewID, (int)VehicleSit.RightBack, true);
                            playerController.IsInVehicle = true;
                            playerController.IsRightBackSit = true;
                            _vehicleController.IsRightBackIn = true;

                            UIManager.Instance.PassangerExitBtn.onClick.RemoveAllListeners();
                            UIManager.Instance.PassangerExitBtn.onClick.AddListener(delegate { ExitVehicle(); });
                        }
                    }
                    else
                    {
                        ActionTemplates.Instance.ShowAlertWindow(_vehicleAlertTitle, _vehicleFullContent);
                    }
                }
            }
        }




        foreach (GameObject door in _vehicleController.AllDoors)
        {
            door.layer = (int)LayerMasks.Default;
        }
    }

    [PunRPC]
    public void ExitVehicle_RPC()
    {
        int localPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PhotonView playerView = null;

        foreach (PhotonView pv in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            if (pv.OwnerActorNr == localPlayerActorNumber)
            {
                playerView = pv;
                break;
            }
        }
        if (playerView != null)
        {

            PlayerController playerController = playerView.GetComponent<PlayerController>();
            PlayerData playerData = playerView.GetComponent<PlayerData>();

            PhotonView photonView = playerData.GetComponent<PhotonView>();

            if (UIManager.Instance.VehicleDriverUI.activeInHierarchy)
                UIManager.Instance.VehicleDriverUI.SetActive(false);

            if (UIManager.Instance.VehiclePassangerUI.activeInHierarchy)
                UIManager.Instance.VehiclePassangerUI.SetActive(false);

            if (playerController.IsDriving)
            {
                //  playerController._characterController.enabled = false;

                _vehicleController.IsDriverIn = false;
                _vehicleController.IsInMovement = false;
                playerController.IsDriving = false;
                UIManager.Instance.HeadlightBtn.onClick.RemoveListener(delegate { ToggleHeadlights(); });
                UIManager.Instance.DriverExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });
                UIManager.Instance.SirenBtn.onClick.RemoveListener(delegate { ToggleSiren(); });
                _vehicleController.CurrentDriverController = null;
                photonView.RPC("SetUserVehicleController", RpcTarget.AllViaServer);
                playerController.PlayerData.LastVehicleController = _vehicleController;
                StartCoroutine(ChangeKinematicStateCorooutine());
                photonView.transform.position = _vehicleController.DriverExit.position;
                //photonView.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            }
            else if (playerController.IsPassanger)
            {
                _vehicleController.IsPassangerIn = false;
                playerController.IsPassanger = false;
                photonView.transform.position = _vehicleController.PassangerExit.position;
                //photonView.transform.localRotation = _vehicleController.PassangerExit.rotation;
                UIManager.Instance.PassangerExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });

            }
            else if (playerController.IsMiddleSit)
            {
                _vehicleController.IsMiddleIn = false;
                playerController.IsMiddleSit = false;
                photonView.transform.position = _vehicleController.MiddleExit.position;
                //photonView.transform.localRotation = _vehicleController.MiddleExit.rotation;
                UIManager.Instance.PassangerExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });
            }
            else if (playerController.IsLeftBackSit)
            {
                _vehicleController.IsLeftBackIn = false;
                playerController.IsLeftBackSit = false;
                photonView.transform.position = _vehicleController.MiddleExit.position;
                //photonView.transform.localRotation = _vehicleController.MiddleExit.rotation;
                UIManager.Instance.PassangerExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });
            }
            else if (playerController.IsRightBackSit)
            {
                _vehicleController.IsRightBackIn = false;
                playerController.IsRightBackSit = false;
                photonView.transform.position = _vehicleController.MiddleExit.position;
                //photonView.transform.localRotation = _vehicleController.MiddleExit.rotation;
                UIManager.Instance.PassangerExitBtn.onClick.RemoveListener(delegate { ExitVehicle(); });
            }
            Debug.Log("Exit Function Activated");
            playerController.IsInVehicle = false;
            playerController.CurrentVehicleController = null;

            _vehicleController.PhotonView.RPC("ChangeSit", RpcTarget.All, playerView.ViewID, 0, false);
            // playerController.transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true; // what for?
            // DontDestroyOnLoad(photonView.gameObject);
        }

        foreach (GameObject door in _vehicleController.AllDoors)
        {
            door.layer = (int)LayerMasks.Interactable;
        }
    }
    #endregion

    private void CloseAllCurrentBarPanels(bool isNatan)
    {
        if (!isNatan)
        {
            UIManager.Instance.AmbulanceNoBagPanel.SetActive(false);
            UIManager.Instance.AmbulanceAmbuPanel.SetActive(false);
            UIManager.Instance.AmbulanceKidsAmbuPanel.SetActive(false);
            UIManager.Instance.AmbulanceMedicPanel.SetActive(false);
            UIManager.Instance.AmbulanceDefibrilationPanel.SetActive(false);
            UIManager.Instance.AmbulanceOxygenPanel.SetActive(false);
            UIManager.Instance.AmbulanceMonitorPanel.SetActive(false);
        }
        else
        {
            UIManager.Instance.NatanNoBagPanel.SetActive(false);
            UIManager.Instance.NatanAmbuPanel.SetActive(false);
            UIManager.Instance.NatanKidsAmbuPanel.SetActive(false);
            UIManager.Instance.NatanMedicPanel.SetActive(false);
            UIManager.Instance.NatanQuickDrugsPanel.SetActive(false);
            UIManager.Instance.NatanDrugsPanel.SetActive(false);
            UIManager.Instance.NatanOxygenPanel.SetActive(false);
            UIManager.Instance.NatanMonitorPanel.SetActive(false);
        }
    }


    #region OnClick Events
    public void OpenCloseBackDoor()
    {
        _vehicleController.PhotonView.RPC("OpenCloseBackDoorRPC", RpcTarget.AllViaServer);
    }
    public void ToggleHeadlights()
    {
        _vehicleController.PhotonView.RPC("ToggleHeadlightsRPC", RpcTarget.AllViaServer);
    }
    public void ToggleSiren()
    {
        _vehicleController.PhotonView.RPC("ToggleSirenRPC", RpcTarget.AllViaServer);
    }
    #endregion

    #region Coroutines
    private IEnumerator ChangeKinematicStateCorooutine()
    {
        yield return new WaitForFixedUpdate();

        _vehicleController.ChangeKinematicState();
    }
    #endregion
}