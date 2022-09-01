using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using TMPro;


public class EvacuationManager : MonoBehaviour
{
    public static EvacuationManager Instance;

    [SerializeField] private TMP_Text _destinationName;

    public List<PhotonView> CtRoomList;
    public List<PhotonView> ShockRoomList;
    public List<PhotonView> ChildrenRoomList;
    public List<PhotonView> EmergencyRoomList;

    //public GameObject SingleIncidentFeedbackWindow;
    //public GameObject AranFeedbackWindow;
    //public bool IsAranOngoing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ShockRoomList = new List<PhotonView>();
        CtRoomList = new List<PhotonView>();
        ChildrenRoomList = new List<PhotonView>();
        EmergencyRoomList = new List<PhotonView>();
    }

    public void AddPatientToRooms(PhotonView patient, EvacRoom enumRoom)
    {
        switch (enumRoom)
        {
            case EvacRoom.Children_Room:
                ChildrenRoomList.Add(patient);
                break;

            case EvacRoom.CT_Room:
                CtRoomList.Add(patient);
                break;

            case EvacRoom.Emergency_Room:
                EmergencyRoomList.Add(patient);
                break;

            case EvacRoom.Shock_Room:
                ShockRoomList.Add(patient);
                break;
        }

        //if (!IsAranOngoing)
        //{
        //    SingleIncidentFeedbackWindow.SetActive(true);
        //}
        //else
        //{
        //    AranFeedbackWindow.SetActive(true);
        //}
    }

    public void DestroyPatient(PhotonView patient)
    {
        patient.gameObject.SetActive(false);
        
    }

    public void ResetEmergencyBed(EmergencyBedController ebc)
    {
        ebc.TakeOutReturnBedToggle();
    }

    //public void ReturnEmergencyBed(EmergencyBedController ebc)
    //{
    //    ebc.ReturnBackBack();
    //}

    public void DropDown_IndexChanged(int index)
    {
        EvacRoom name = (EvacRoom)index;
        _destinationName.text = name.ToString();
    }
}
