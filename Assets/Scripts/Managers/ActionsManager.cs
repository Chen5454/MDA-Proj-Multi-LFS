using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ActionsManager : MonoBehaviour
{
    public static ActionsManager Instance;

    [Header("Photon")]
    public List<PhotonView> AllPlayersPhotonViews;
    public List<PlayerData> AllPlayerData;

    [Header("VehiclesPrefabs")]
    //[SerializeField] private GameObject _ambulancePrefab;
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
    public MeshFilter[] Vests;
    #endregion

    [Header("Crews")]
    public int NextCrewIndex = 0;
    public List<Transform> /*AmbulancePosTransforms,*/ VehiclePosTransforms;
    public List<bool> /*AmbulancePosTransforms,*/ VehiclePosOccupiedList;

    private Patient _lastClickedPatient;
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

                NewPatientData currentPatientData = myPlayerData.CurrentPatientNearby != null ? myPlayerData.CurrentPatientNearby.NewPatientData : null;
                _lastClickedPatientData = currentPatientData;

                Debug.Log($"{myPlayerData.UserName} Clicked on: {myPlayerData.CurrentPatientNearby}");

                if (!myPlayerData.CurrentPatientNearby.IsPlayerJoined(myPlayerData))
                {
                    Debug.Log($"Attempting Join Patient");
                    _lastClickedPatient.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllBufferedViaServer);
                    Debug.Log($"Joined Patient");
                    UIManager.Instance.JoinPatientPopUp.SetActive(true);
                    Debug.Log($"Attempting Open Player Info");
                }
                else
                {
                    Debug.Log($"Attempting Open Player Info");
                    _lastClickedPatient.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllBufferedViaServer);
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
            PlayerData myPlayerData = AllPlayersPhotonViews[i].gameObject.GetComponent<PlayerData>();

            if (isJoined)
            {
                myPlayerData.PhotonView.RPC("OnJoinPatient", RpcTarget.AllBufferedViaServer);
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
            PlayerData myPlayerData = AllPlayersPhotonViews[i].gameObject.GetComponent<PlayerData>();
            myPlayerData.PhotonView.RPC("OnLeavePatient", RpcTarget.AllBufferedViaServer);
            UIManager.Instance.CloseAllPatientWindows();
        }
    }
    #endregion
}