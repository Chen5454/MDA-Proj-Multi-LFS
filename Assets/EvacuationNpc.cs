using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EvacuationNpc : MonoBehaviour
{
    private Evacuation evacuation;

    private TestingBedCollider bedRef;

    public GameObject _evacuationUI;

    public GameObject DoorLayer;

    private PhotonView _photonView;
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        evacuation = GetComponentInParent<Evacuation>();

        bedRef = GetComponentInParent<TestingBedCollider>();

        DoorLayer.layer = (int)LayerMasks.Default;

    }

    public void OnInteracted()
    {
        OnEvacuateNPCClicked();
    }

    //public void Update()
    //{
    //    CheckIfInteractable();

    //}

    public void CheckIfInteractable()
    {
        
            if (bedRef.BedRefrence.GetComponent<EmergencyBedController>()._player.GetComponent<PlayerController>()._photonView.IsMine && bedRef.BedRefrence.GetComponent<EmergencyBedController>()._isFollowingPlayer)
            {
                DoorLayer.layer = (int)LayerMasks.Interactable;
            }
            else
            {
                DoorLayer.layer = (int)LayerMasks.Default;
            }
        
    }

    public void OnEvacuateNPCClicked()
    {
        Debug.Log($"Attempting to Click On Npc");
        EmergencyBedController bed = bedRef.BedRefrence.GetComponent<EmergencyBedController>();

        if (bed.IsPatientOnBed)
            _evacuationUI.SetActive(true);
        else
            ActionTemplates.Instance.ShowAlertWindow("Evac", "No Patient");

    }

    public void EvacPatient()
    {
        _photonView.RPC("DestoryPatient", FindPatientOwner());
        _photonView.RPC("EvacPatient_RPC", RpcTarget.AllViaServer);

    }

    private Player FindPatientOwner()
    {
        var patient = evacuation.NearbyPatient[0].GetComponent<Patient>();
        var patientOwner = patient.GetComponent<PhotonView>().Owner;
        return patientOwner;
        //for (int i = 0; i < GameManager.Instance.AllPatients.Count; i++)
        //{
        //    Patient desiredPatient = GameManager.Instance.AllPatients[i].GetComponent<Patient>();

        //    if (desiredPatient._ownedCrewNumber == evacuation.NearbyPatient[0]._ownedCrewNumber)
        //    {
        //        Player playerOwner = desiredPatient.GetComponent<PhotonView>().Owner;
        //        return playerOwner;
        //    }


        //}

        //return null;
    }

    [PunRPC]
    public void EvacPatient_RPC()
    {
        EmergencyBedController bed = bedRef.BedRefrence.GetComponent<EmergencyBedController>();

        for (int i = 0; i < evacuation.NearbyPatient[0].NearbyUsers.Count; i++)
        {
            PlayerData playerData = evacuation.NearbyPatient[0].NearbyUsers[i];
            if (playerData.LastCarController || playerData.LastVehicleController)
            {
                playerData.LastVehicleController.IsBusy = false;
                playerData.LastCarController.IsInPinuy = false;
                break;
            }
        }
        EvacuationManager.Instance.AddPatientToRooms(evacuation.NearbyPatient[0].PhotonView, evacuation.RoomEnum);
        evacuation.NearbyPatient.Clear();

        EvacuationManager.Instance.ResetEmergencyBed(bed);

        bed.EmergencyBedUI.SetActive(false);

        if (bed.ParentVehicle.IsNatan)
        {
            UIManager.Instance.NatanBar.SetActive(true);
        }
        else
        {
            UIManager.Instance.AmbulanceBar.SetActive(true);
        }

        _evacuationUI.SetActive(false);
        
    }

    [PunRPC]
    private void DestoryPatient()
    {
       // EvacuationManager.Instance.DestroyPatient(evacuation.NearbyPatient[0].PhotonView);
        PhotonNetwork.Destroy(evacuation.NearbyPatient[0].gameObject);
    }


    public void CancelEvac()
   {
        _evacuationUI.SetActive(false);

    }
}
