using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingBedCollider : MonoBehaviour
{

    [SerializeField] public GameObject BedRefrence;
    public GameObject DoorLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EmergencyBed"))
        {
            Debug.Log("Bed Detected");
            BedRefrence = other.gameObject;

            if (BedRefrence)
            {
                if (BedRefrence.GetComponent<EmergencyBedController>() && BedRefrence.GetComponent<EmergencyBedController>()._player.GetComponent<PlayerController>())
                {
                    if (BedRefrence.GetComponent<EmergencyBedController>()._player.GetComponent<PlayerController>()._photonView.IsMine && BedRefrence.GetComponent<EmergencyBedController>()._isFollowingPlayer)
                    {
                        DoorLayer.layer = (int)LayerMasks.Interactable;
                    }
                }   
            }
       
            //else
            //{
            //    DoorLayer.layer = (int)LayerMasks.Default;
            //}
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EmergencyBed"))
        {
            BedRefrence = other.gameObject;
            if (BedRefrence != null)
            {
                if (BedRefrence.GetComponent<EmergencyBedController>() && BedRefrence
                        .GetComponent<EmergencyBedController>()._player.GetComponent<PlayerController>())
                {
                    if (BedRefrence)
                    {
                        if (BedRefrence.GetComponent<EmergencyBedController>()._player.GetComponent<PlayerController>()
                                ._photonView.IsMine &&
                            BedRefrence.GetComponent<EmergencyBedController>()._isFollowingPlayer)
                        {
                            DoorLayer.layer = (int) LayerMasks.Interactable;
                        }

                        if (!BedRefrence.GetComponent<EmergencyBedController>()._isFollowingPlayer)
                        {
                            DoorLayer.layer = (int) LayerMasks.Default;
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EmergencyBed"))
        {
            BedRefrence = null;
            DoorLayer.layer = (int)LayerMasks.Default;
        }
    }
}
