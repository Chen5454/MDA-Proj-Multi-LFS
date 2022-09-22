using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks,IInRoomCallbacks
{
    public static GameManager Instance;
    
    private PhotonView _photonView;
    [field: SerializeField] public bool IsAranActive { get; set; }

    [Header("General")]
    public List<Transform> CurrentIncidentsTransforms;
    public List<Patient> AllPatients;
    public List<Patient> AllTaggedPatients;
    //public List<int> OnGoingIncidents;
    public Transform[] IncidentPatientSpawns;
    public bool[] IsPatientSpawned;

    [Header("Aran")]
    public PhotonView Pikud10View; 
    public PhotonView Redua10View; 
    public PhotonView Pinuy10View; 
    public PhotonView Henyon10View;
    public List<int> usedValues = new List<int>();
    public List<string> usedNamesValues = new List<string>();
    public List<PhotonView> NatanCarList = new List<PhotonView>();
    public List<PhotonView> NatanInPinuyCarList = new List<PhotonView>();
    public List<PhotonView> NatanFreeCarList = new List<PhotonView>();

    [Header("Pikud10")]
    public Camera Pikud10Camera;
    public Material LineMaterial;
    public RenderTexture Pikud10TextureRenderer;

    [Header("Pinuy10")]
    public GameObject TaggedPatientListRow;
    public GameObject UrgentEvacuationCanvas;

    [Header("Metargel")]
    public List<string> metargelUsersList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            Instance = this;

        }
        DontDestroyOnLoad(this.gameObject);

    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        OnEscape(true);

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    OnGoingIncidents = new List<int>();
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (PhotonView car in NatanCarList)
            {
                Debug.Log(car.GetComponent<CarControllerSimple>().RandomNumber+" "+ car.GetComponent<CarControllerSimple>().RandomName);
            }
        }

        if (IsAranActive)
        {
            UpdatePinuyList();
        }
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    private void OnEscape(bool paused)
    {
        ChangeCursorMode(paused);
        //GameMenuMode(paused);
    }

    private void ChangeCursorMode(bool unlocked)
    {
        if (unlocked)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect(); // disconnected from the master client

         while (PhotonNetwork.IsConnected)
            yield return null;

        // SceneManager.LoadScene(0);
        //PhotonNetwork.LoadLevel(0);
        Application.Quit();
    }

    //Called from UI Quit
    public void OnPlayerDisconnect()
    {
        Debug.Log("attempting to Leave Room and Disconnecting");

        DisconnectPlayer();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (this.CanRecoverFromDisconnect(cause))
        {
            this.Recover();
        }
    }

    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
                
        }

        return false;
    }

    private void Recover()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.LogError("ReconnectAndRejoin failed. trying to Reconnect");
            if (!PhotonNetwork.Reconnect())
            {
                Debug.LogError("Reconnect failed. trying to ConnectUsingSettings");
                if (!PhotonNetwork.ConnectUsingSettings())
                {
                    Debug.LogError("ConnectUsingSettings failed ");

                }
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].OwnerActorNr == otherPlayer.ActorNumber)
            {
                if (ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PhotonView>().IsMine == true && PhotonNetwork.IsConnected == true)
                    PhotonNetwork.Destroy(ActionsManager.Instance.AllPlayersPhotonViews[i]);
            }
        }

        Debug.Log(otherPlayer.NickName + " has left the room");
        Debug.Log(ActionsManager.Instance.AllPlayersPhotonViews.Count);
    }

    //void OnFinshedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    currentScene = scene.buildIndex;
    //}



    public void UpdateTaggedPatientList()
    {
        
    }

    public void UpdatePinuyList()
    {
        _photonView.RPC("UpdatePinuyList_RPC", RpcTarget.AllBufferedViaServer);
    }

    public void ChangeAranState(bool isActive)
    {
        photonView.RPC("ChangeAranStateRPC", RpcTarget.All, isActive);
    }

    [PunRPC]
    public void UpdatePinuyList_RPC()
    {
        foreach (PhotonView car in NatanCarList)
        {
            if (car.GetComponent<CarControllerSimple>().IsInPinuy)
            {
                NatanFreeCarList.Remove(car);

                if (!NatanInPinuyCarList.Contains(car))
                    NatanInPinuyCarList.Add(car);
            }
            else
            {
                NatanInPinuyCarList.Remove(car);

                if (!NatanFreeCarList.Contains(car))
                    NatanFreeCarList.Add(car);
            }
        }
    }

    [PunRPC]
    private void ChangeAranStateRPC(bool isActive)
    {
        IsAranActive = isActive;
    }
}