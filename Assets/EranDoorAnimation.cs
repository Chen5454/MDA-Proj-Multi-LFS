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
    [SerializeField] private PhotonView _photonView;

    private OwnershipTransfer _transfer;
    private bool _isOpen;
    private bool isClosed;


    private void Start()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _photonView = GetComponent<PhotonView>();
        _eranDoorAnim = GetComponent<Animator>();
    }


    void Update()
    {
        //if (PhotonNetwork.IsMasterClient)
        //    _photonView.RPC("AnimateEranDoor", RpcTarget.AllBufferedViaServer);
        AnimateEranDoor();
    }


    public void ShowDoorUI()
    {
        _transfer.TvOwner();
        eranDoorUI.SetActive(true);
    }

    public void OpenDoorClick()
    {
        _isOpen = true;
    }

    [PunRPC]
    public void AnimateEranDoor()
    {
        if (_isOpen)
        {
            eranDoor.GetComponent<BoxCollider>().enabled = false;
            _eranDoorAnim.SetBool("OpenDoor",true);
            _eranDoorAnim.SetBool("CloseDoor",false);

        }
        //else
        //{
        //    eranDoor.SetBool("OpenDoor", false);
        //    eranDoor.SetBool("CloseDoor", true);

        //}
    }

  
}
