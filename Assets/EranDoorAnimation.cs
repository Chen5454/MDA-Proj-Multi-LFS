using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EranDoorAnimation : MonoBehaviour
{
    [SerializeField] private GameObject eranDoor;
    [SerializeField] private GameObject eranDoorUI;
    private Animator _eranDoorAnim;
   // [SerializeField] private PhotonView _photonView;
    [SerializeField] private PlayerData PlayerRefrence;

    //private OwnershipTransfer _transfer;
    public bool _isOpen;
    private bool isClosed;
  //  private bool AllowedToOpen;


    private void Start()
    {
       // _transfer = GetComponent<OwnershipTransfer>();
      //  _photonView = GetComponent<PhotonView>();
        _eranDoorAnim = GetComponent<Animator>();
        eranDoor.layer = (int)LayerMasks.Default;

    }


    void Update()
    {
        //    _photonView.RPC("AnimateEranDoor", RpcTarget.AllBufferedViaServer);
        AnimateEranDoor();
    }


    public void ShowDoorUI()
    {
       //_transfer.TvOwner();
        eranDoorUI.SetActive(true);
    }

    public void OpenDoorClick()
    {
        _isOpen = true;
    }

   // [PunRPC]
    public void AnimateEranDoor()
    {
        if (_isOpen)
        {
           // eranDoor.GetComponent<BoxCollider>().enabled = false;
            _eranDoorAnim.SetBool("OpenDoor",true);
            //_eranDoorAnim.SetBool("CloseDoor",false);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRefrence = other.gameObject.GetComponent<PlayerData>();
            if (PlayerRefrence.IsInstructor)
            {
                Debug.Log("Welcome Instructor ");
                eranDoor.layer = (int)LayerMasks.Interactable;
                //  AllowedToOpen = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRefrence =null;
            eranDoor.layer = (int)LayerMasks.Default;

        }
    }
}
