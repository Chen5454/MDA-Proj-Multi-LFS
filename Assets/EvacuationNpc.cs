using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class EvacuationNpc : MonoBehaviour
{
    private Evacuation evacuation;

    private TestingBedCollider bedRef;

    public GameObject _evacuationUI;

    private PhotonView _photonView;
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        evacuation = GetComponentInParent<Evacuation>();

        bedRef = GetComponentInParent<TestingBedCollider>();
    }

    public void OnInteracted()
    {
        OnEvacuateNPCClicked();
    }

    public void OnEvacuateNPCClicked()
    {
        Debug.Log($"Attempting to Click On Npc");
        _evacuationUI.SetActive(true);
    }

    public void EvacPatient()
    {
        _photonView.RPC("EvacPatient_RPC", RpcTarget.AllBufferedViaServer);
        // AllPatientsPhotonViews.PhotonView.RPC("EvacPatient_RPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void EvacPatient_RPC()
    {
        for (int i = 0; i < evacuation.NearbyPatient[0].NearbyUsers.Count; i++)
        {
            PlayerData playerData = evacuation.NearbyPatient[0].NearbyUsers[i];
            if (playerData.LastCarController)
            {
                playerData.LastCarController.IsInPinuy = false;
                break;
            }
        }
        EvacuationManager.Instance.AddPatientToRooms(evacuation.NearbyPatient[0].PhotonView, evacuation.RoomEnum);
        EvacuationManager.Instance.DestroyPatient(evacuation.NearbyPatient[0].PhotonView);
        evacuation.NearbyPatient.Clear();

        EmergencyBedController bed = bedRef.BedRefrence.GetComponent<EmergencyBedController>();
        EvacuationManager.Instance.ResetEmergencyBed(bed);

        _evacuationUI.SetActive(false);
    }

    public void CancelEvac()
   {
        _evacuationUI.SetActive(false);

    }
}
