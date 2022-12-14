using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

/*
 * for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                PhotonView photonView = ActionsManager.Instance.AllPlayersPhotonViews[i];
            }
        }
 */

public class ActionsManager : MonoBehaviour
{
    public static ActionsManager Instance;

    [Header("Photon")]
    public List<PhotonView> AllPlayersPhotonViews;
    public List<PlayerData> AllPlayerData;

    [Header("VehiclesPrefabs")]
    public GameObject AmbulancePrefab;
    public GameObject NatanPrefab;

    #region Prefab References
    [Header("Equipment")]
    public GameObject Clipboard;
    public GameObject HeadVice, Megaphone, NeckBrace, Hat, BloodPressureSleeve, OxyMask, RespirationBalloon;
    public GameObject ArmBandage, ArmTourniquet, BicepsBandage, BicepsTourniquet, KneeBandage, KneeTourniquet, ShinBandage, ShinTourniquet;

    [Header("Attachments")]
    public GameObject Asherman;
    public GameObject EcgSticker, ThroatTube, InTube, Venflon;

    [Header("Aids")]
    public GameObject OxyTank;
    public GameObject IVPole;

    [Header("Devices")]
    public GameObject BloodPressureDevice;
    public GameObject Monitor, Respirator;

    [Header("Vests")]
    public Mesh[] Vests;
    #endregion

    [Header("Crews")]
    public int NextCrewIndex = 0;
    public List<Transform> /*AmbulancePosTransforms,*/ VehiclePosTransforms;
    public List<bool> /*AmbulancePosTransforms,*/ VehiclePosOccupiedList;

    private Patient _lastClickedPatient;
    public Patient LastClickedPatient => _lastClickedPatient;
    private NewPatientData _lastClickedPatientData;

    #region MonoBehaviour Callbacks
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
    #endregion

    #region UnityEvents
    public void OnPatientClicked()
    {
        Debug.Log($"Attempting to Click On Patient");

        // loops through all players photonViews
        for (int i = 0; i < AllPlayersPhotonViews.Count; i++)
        {
            if (AllPlayersPhotonViews[i].IsMine)
            {
                PhotonView photonView = AllPlayersPhotonViews[i];
                PlayerData myPlayerData = photonView.gameObject.GetComponent<PlayerData>();
                _lastClickedPatient = myPlayerData.CurrentPatientNearby;

                //NewPatientData currentPatientData = myPlayerData.CurrentPatientNearby != null ? myPlayerData.CurrentPatientNearby.NewPatientData : null;
                //_lastClickedPatientData = currentPatientData;

                Debug.Log($"{myPlayerData.UserName} Clicked on: {myPlayerData.CurrentPatientNearby}");

                if (_lastClickedPatient == null)
                    return;

                if (!myPlayerData.CurrentPatientNearby.IsPlayerJoined(myPlayerData))
                {
                    Debug.Log($"Attempting Join Patient");
                    _lastClickedPatient.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllViaServer);
                    Debug.Log($"Joined Patient");
                    UIManager.Instance.JoinPatientPopUp.SetActive(true);
                    Debug.Log($"Attempting Open Player Info");
                }
                else
                {
                    Debug.Log($"Attempting Open Player Info");
                    _lastClickedPatient.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllViaServer);
                    UIManager.Instance.PatientInfoParent.SetActive(true);
                    Debug.Log($"Attempting Open Player Info");
                }

                break;
            }
        }
    }


    public PhotonView GetPlayerPhotonViewByNickName(string nickName)
    {
        for (int i = 0; i < AllPlayersPhotonViews.Count; i++)
        {
            if (AllPlayersPhotonViews[i].Owner.NickName == nickName)
            {
                return AllPlayersPhotonViews[i];
            }
        }

        return null;
    }

    public void OnPlayerJoinPatientRPC(bool isJoined)
    {
        Debug.Log("attempting to Join Patient");

        for (int i = 0; i < AllPlayersPhotonViews.Count; i++)
        {
            if (!AllPlayersPhotonViews[i].IsMine)
                continue;

            PlayerData myPlayerData = AllPlayersPhotonViews[i].gameObject.GetComponent<PlayerData>();

            if (isJoined)
            {
                myPlayerData.CurrentPatientNearby.PhotonView.RPC("AddUserToTreatingLists", RpcTarget.AllViaServer, myPlayerData.UserName);
              //  Debug.LogError("Added to AddUserToTreatingLists");
                UIManager.Instance.JoinPatientPopUp.SetActive(false);
                UIManager.Instance.PatientInfoParent.SetActive(true);
            }
            else
            {
                UIManager.Instance.JoinPatientPopUp.SetActive(false);
            }
        }
    }

    public void OnPlayerLeavePatientRPC()
    {
        Debug.Log("attempting to Leave Patient");

        for (int i = 0; i < AllPlayersPhotonViews.Count; i++)
        {
            if (!AllPlayersPhotonViews[i].IsMine)
                continue;

            PlayerData myPlayerData = AllPlayersPhotonViews[i].gameObject.GetComponent<PlayerData>();

            if (myPlayerData.CurrentPatientNearby)
                myPlayerData.PhotonView.RPC("OnLeavePatient", RpcTarget.AllViaServer, myPlayerData.CurrentPatientNearby.PhotonView.ViewID);
            else
                return;
            UIManager.Instance.CloseAllPatientWindows();
        }
    }
    #endregion
}