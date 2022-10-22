using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationDoorAnimation : MonoBehaviour
{
    [SerializeField] private GameObject operationDoor;
    [SerializeField] private GameObject operationDoorUI;
     private Animator operationDoorAnim;
    [SerializeField] private PlayerData PlayerRefrence;

    private OwnershipTransfer _transfer;
    public bool _isOpen;

    private void Start()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        operationDoorAnim = GetComponent<Animator>();
        operationDoor.layer = (int)LayerMasks.Default;

    }


    void Update()
    {
        AnimateEranDoor();
    }


    public void ShowDoorUI()
    {
        _transfer.TvOwner();
        operationDoorUI.SetActive(true);
    }

    public void OpenDoorClick()
    {
        _isOpen = true;
    }

    public void AnimateEranDoor()
    {
        if (_isOpen)
        {
            Debug.Log("Opening door ");
            operationDoorAnim.SetBool("OpenDoor", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRefrence = other.gameObject.GetComponent<PlayerData>();
            if (PlayerRefrence.IsInstructor || PlayerRefrence.IsMokdan)
            {
                Debug.Log("Welcome Instructor Or Mokdan ");
                operationDoor.layer = (int)LayerMasks.Interactable;
                //  AllowedToOpen = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRefrence = null;
            operationDoor.layer = (int)LayerMasks.Default;

        }
    }
}
