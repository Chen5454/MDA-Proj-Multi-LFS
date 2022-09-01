using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OwnershipTransfer : MonoBehaviourPun,IPunOwnershipCallbacks
{
    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void ClickToJoinPatient()
    {
        base.photonView.RequestOwnership();
    }

    public void PatientJoinBed()
    {
        base.photonView.RequestOwnership();
    }
 
    public void CarDriver()
    {
        base.photonView.RequestOwnership();

    }
    public void CarDoorAnimation()
    {
        base.photonView.RequestOwnership();

    }
    public void BedPickUp()
    {
        base.photonView.RequestOwnership();
    }

    public void CrewUI()
    {
        base.photonView.RequestOwnership();
    }

    public void TvOwner()
    {
        base.photonView.RequestOwnership();
    }

    public void LogCheack()
    {
        base.photonView.RequestOwnership();
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log("Changing Ownership " + requestingPlayer.NickName);

        if (targetView != base.photonView)
            return;

        base.photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView)
            return;
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
}
