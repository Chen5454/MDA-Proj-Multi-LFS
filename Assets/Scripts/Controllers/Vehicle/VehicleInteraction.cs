using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VehicleInteraction : MonoBehaviour
{
    [SerializeField] private VehicleController _vehicleController;
    
    private VehicleSit _vehicleSit;

    public void EnterVehicle(int sitNum)
    {
        _vehicleSit = (VehicleSit)sitNum;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                PhotonView photonView = ActionsManager.Instance.AllPlayersPhotonViews[i];
                PlayerController playerController = photonView.GetComponent<PlayerController>();

                if (_vehicleController.CollidingPlayers.Contains(photonView.gameObject))
                {
                    if (_vehicleSit == VehicleSit.Driver)
                    {
                        playerController.IsInVehicle = true;
                        playerController.IsDriving = true;
                        photonView.transform.SetParent(_vehicleController.DriverSit);
                        photonView.transform.localPosition = Vector3.zero;
                        photonView.transform.localRotation = Quaternion.identity;
                        _vehicleController.CurrentDriverController = playerController;
                        _vehicleController.IsDriverIn = true;
                        break;
                    }
                    else if (_vehicleSit == VehicleSit.Passanger)
                    {
                        photonView.transform.position = _vehicleController.PassangerSit.position;
                        playerController.IsInVehicle = true;
                        _vehicleController.IsPassangerIn = true;
                        break;
                    }
                    else
                    {
                        if (_vehicleController.IsMiddleIn)
                        {
                            photonView.transform.position = _vehicleController.MiddleSit.position;
                            playerController.IsInVehicle = true;
                            _vehicleController.IsMiddleIn = true;
                            break;
                        }
                        else if (_vehicleController.IsLeftBackIn)
                        {
                            photonView.transform.position = _vehicleController.LeftBackSit.position;
                            playerController.IsInVehicle = true;
                            _vehicleController.IsLeftBackIn = true;
                            break;
                        }
                        else if (_vehicleController.IsRightBackIn)
                        {
                            photonView.transform.position = _vehicleController.RightBackSit.position;
                            playerController.IsInVehicle = true;
                            _vehicleController.IsRightBackIn = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
