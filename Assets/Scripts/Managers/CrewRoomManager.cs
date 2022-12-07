using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class CrewRoomManager : MonoBehaviour,IPunObservable
{
    //public GameObject RoomDoorBlocker;
    //public TextMeshProUGUI CrewMemberNameText1, CrewMemberNameText2, CrewMemberNameText3, CrewMemberNameText4;

    private PhotonView _photonView;

    public int _crewRoomIndex;
    public static int _crewRoomIndexStatic;
    public int _playersMaxCount = 4;

    public Canvas RoomCrewMenuUI;
    public List<TextMeshProUGUI> listOfUiNamesTMP;
    public List<TMP_Dropdown> CrewMemberRoleDropDownList;
    public TMP_Dropdown CrewLeaderDropDown;

    public List<PhotonView> _playersInRoomList;

    private const byte COLOR_CHANGE_EVENT = 0;
    //public int _crewRoomIndex;

    private Color crewColor;
    private Vector3 _vestPos = new Vector3(0f, 0.295f, -0.015f);

    [SerializeField] private GameObject _tvScreen;
    [SerializeField] private GameObject _patientMale, _patientFemale, _chooseIncidentMenu, _overlay, _chooseSimulationPanel;
    [SerializeField] private Button _startSimulationBtn;
    [SerializeField] private TextMeshProUGUI _currentIncidentNameTMP, _startSimulationTMP;
    [SerializeField] private TMP_InputField _apartmentNumber;
    [SerializeField] private string _currentIncidentName, _startSimulationText, _startAranSimulationText, _waitMemberText, _incidentStartTitle, _incidentStartText, _errorTitle, _errorFullString, _errorSomthingWentWrong, _errorAptBusy;
    [SerializeField] private bool isUsed, _isNatanRequired, _isRandomIncident;
    [SerializeField] public FilteredPatientsRoster _filterredRoaster;


    private OwnershipTransfer _transfer;
    //[SerializeField] private GameObject _crewRoomDoor;

    private void Awake()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _crewRoomIndexStatic = 0;
        _photonView = GetComponent<PhotonView>();
        PopulateDropdownRoles();
        RoomCrewMenuUI.gameObject.SetActive(false);
        GameManager.Instance.CrewRoomsList.Add(_photonView);
    }

    private void Start()
    {
        _currentIncidentNameTMP.text = _currentIncidentName;
        //GameManager.Instance.AllCrewRooms[_crewRoomIndex - 1] = this;
        //GameManager.Instance.AllCrewRooms.Add(this);
        _crewRoomIndexStatic++;
        _crewRoomIndex = _crewRoomIndexStatic;
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            InteractUICrew();
        }
        else
        {
            RoomCrewMenuUI.GetComponent<CanvasGroup>().interactable = false;
        }
    }

    private void OnEnable()
    {
        _filterredRoaster.CrewRoomManager = this;
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == COLOR_CHANGE_EVENT)
        {
            object[] data = (object[]) obj.CustomData;
            var r = (float)data[0];
            var b = (float)data[1];
            var g = (float)data[2];

            for (int i = 0; i < _playersInRoomList.Count; i++)
            {
                PlayerData currentPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
                NameTagDisplay desiredPlayerName = _playersInRoomList[i].GetComponentInChildren<NameTagDisplay>();

                desiredPlayerName.text.color = new Color(r, b, g);
                currentPlayerData.CrewColor = new Color(r, b, g);
            }


        }
    
    }

    private bool CheckIfAlreadyInList(GameObject player)
    {
        bool playerFound = false;

        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (_playersInRoomList[i].ViewID == player.GetPhotonView().ViewID)
            {
                playerFound = true;
            }
        }

        return playerFound;
    }

    private void InteractUICrew()
    {
        if (isUsed)
        {
            _tvScreen.layer = (int)LayerMasks.Default;
            RoomCrewMenuUI.GetComponent<CanvasGroup>().interactable = true;
        }
    }


    private void SetCrewUITexts()
    {
        // Crew Roles UI
        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            listOfUiNamesTMP[i].text = _playersInRoomList[i].Owner.NickName;
        }

        // Crew Leader UI
        List<string> nicknamesList = new List<string>();
        foreach (var player in _playersInRoomList)
        {
            nicknamesList.Add(player.Owner.NickName);
        }

        CrewLeaderDropDown.ClearOptions();
        CrewLeaderDropDown.AddOptions(nicknamesList);
    }

    private void PopulateDropdownRoles()
    {
        string[] roles = Enum.GetNames(typeof(Roles));
        List<string> rolesList = new List<string>(roles);

        foreach (var dropdown in CrewMemberRoleDropDownList)
        {
            dropdown.AddOptions(rolesList);
        }
    }

    public void CreateCrewSubmit()
    {
         _photonView.RPC("CrewCreateSubmit_RPC", RpcTarget.AllBufferedViaServer, GetCrewRolesByEnum(),GetCrewLeaderIndex(), _crewRoomIndex);
         _photonView.RPC("GivesLeaderButton",RpcTarget.AllBufferedViaServer, GetCrewLeaderIndex()); 
      //  var color = Random.ColorHSV();
        //_photonView.RPC("ChangeCrewColors", RpcTarget.AllBufferedViaServer, new Vector3(color.r, color.g, color.b));
        ChangeCrewColors();
        //_photonView.RPC("CrewLeaderIsChosen", RpcTarget.AllBufferedViaServer, GetCrewLeader());
    }

    [PunRPC]
    public void GivesLeaderButton( int leaderIndex)
    {
        foreach (PhotonView player in _playersInRoomList)
        {
            if (player.IsMine)
            {
                UIManager.Instance.ResetCrewRoom.gameObject.SetActive(false);

            }
        }



        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            PlayerData leaderToBe = _playersInRoomList[leaderIndex].GetComponent<PlayerData>();
            leaderToBe.IsCrewLeader = true;
            if (leaderToBe.IsCrewLeader)
            {
                if (leaderToBe.photonView.IsMine)
                {
                    UIManager.Instance.ResetCrewRoom.gameObject.SetActive(true);

                }
            }
        }
 
    }

    public int[] GetCrewRolesByEnum()
    {
        int[] roles = new int[_playersInRoomList.Count];

        for (int i = 0; i < roles.Length; i++)
        {
            roles[i] = CrewMemberRoleDropDownList[i].value;
        }
        return roles;
    }

    public int GetCrewLeaderIndex()
    {
        int leaderIndex = 0;

        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (CrewLeaderDropDown.GetComponentInChildren<TextMeshProUGUI>().text == _playersInRoomList[i].Owner.NickName)
            {
                leaderIndex = i;
            }

        }
        return leaderIndex;
    }

    public Player GetCrewLeader()
    {
        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (CrewLeaderDropDown.GetComponentInChildren<TextMeshProUGUI>().text == _playersInRoomList[i].Owner.NickName)
            {
                Debug.Log("Returned Leader Owner : " + _playersInRoomList[i].Owner);
                return _playersInRoomList[i].GetComponent<PlayerData>().PhotonView.Owner;
            }
        }
        Debug.Log("No Crew Leader Owner Returned");

        return null;
    }

    // Show Hide MenuUI
    // --------------------
    public void ShowCrewRoomMenu()
    {
        _transfer.CrewUI();
        _photonView.RPC("ShowCrewUI_RPC", RpcTarget.AllBufferedViaServer);

        // RoomCrewMenuUI.gameObject.SetActive(true);
        //RefreshCrewUITexts();
    }

    public void HideCrewRoomMenu()
    {
        _photonView.RPC("CloseCrewUI_RPC", RpcTarget.AllBufferedViaServer);

        // RoomCrewMenuUI.gameObject.SetActive(false);
    }

    private void AlertStartAll(string title, string content)
    {
        _photonView.RPC("AlertStartAllRPC", RpcTarget.All, title, content);
    }

    private void StartRandomIncident()
    {
        List<int> unavailableList = new List<int>();

        for (int i = 0; i < GameManager.Instance.IsPatientSpawned.Length - 1; i++)
        {
            if (GameManager.Instance.IsPatientSpawned[i])
                unavailableList.Add(i);
        }

        int apartmentNum = Random.Range(0, 5);
        Debug.Log("Starting aptNum" + apartmentNum);
        try
        {
            while (!(unavailableList.Count >= 5) && GameManager.Instance.IsPatientSpawned[apartmentNum])
            {
                apartmentNum = Random.Range(0, 6);
                Debug.Log("New aptNum" + apartmentNum);

                //if (!GameManager.Instance.IsPatientSpawned[apartmentNum])
                //    break;
            }
        }
        catch
        {
            AlertStartAll(_errorTitle, _errorFullString);
        }
        if (!GameManager.Instance.IsPatientSpawned[apartmentNum])
        {
            //PhotonNetwork.Instantiate(_patientMale.name, GameManager.Instance.IncidentPatientSpawns[apartmentNum].position, GameManager.Instance.IncidentPatientSpawns[apartmentNum].rotation);
            if (PatientCreationSpace.PatientCreator.newPatient == null)
            {
                Debug.LogError("no patient loaded!");
                return;
            }
            //1) Get the NewPatientData to be spawned

            //2) switchcase on which models/prefab to spawn from NewPatientData

            //3) Instantiate correct prefab

            GameObject go = PhotonNetwork.InstantiateRoomObject(_patientMale.name, GameManager.Instance.IncidentPatientSpawns[apartmentNum].position, GameManager.Instance.IncidentPatientSpawns[apartmentNum].rotation);
            //3.5) Grab the Patient component from the instantiated object.
            //4) Set this patients data to the NewPatientData to be spawned
            go.GetComponent<Patient>().InitializePatientData(PatientCreationSpace.PatientCreator.newPatient);
            //go.GetComponent<Patient>().PhotonView.TransferOwnership(GetCrewLeader());
            _photonView.RPC("UpdateCurrentIncidents", RpcTarget.AllBufferedViaServer, apartmentNum);
            AlertStartAll(_incidentStartTitle, $"{_incidentStartText} {apartmentNum + 1}");

        }
        else
        {
            AlertStartAll(_errorTitle, _errorSomthingWentWrong);
        }
    }
    private void StartSpecificIncident()
    {
        int apartmentNum = int.Parse(_apartmentNumber.text);
        List<int> unavailableList = new List<int>();

        for (int i = 0; i < GameManager.Instance.IsPatientSpawned.Length - 1; i++)
        {
            if (GameManager.Instance.IsPatientSpawned[i])
                unavailableList.Add(i);
        }

        if (unavailableList.Count >= 5)
        {
            AlertStartAll(_errorTitle, _errorFullString);
        }
        else if (GameManager.Instance.IsPatientSpawned[apartmentNum - 1])
        {
            AlertStartAll(_errorTitle, _errorAptBusy);
        }
        else
        {
            PhotonNetwork.InstantiateRoomObject(_patientMale.name, GameManager.Instance.IncidentPatientSpawns[apartmentNum - 1].position, GameManager.Instance.IncidentPatientSpawns[apartmentNum - 1].rotation);

            _photonView.RPC("UpdateCurrentIncidents", RpcTarget.AllBufferedViaServer, apartmentNum - 1);

            AlertStartAll(_errorTitle, $"{_incidentStartText} {apartmentNum + 0}");
        }
    }
    private void StartIncidentInRandomLocation()
    {
        List<int> unavailableList = new List<int>();

        for (int i = 0; i < GameManager.Instance.IsPatientSpawned.Length - 1; i++)
        {
            if (GameManager.Instance.IsPatientSpawned[i])
                unavailableList.Add(i);
        }

        int apartmentNum = Random.Range(0, 5);
        Debug.Log("Starting aptNum" + apartmentNum);
        try
        {
            while (!(unavailableList.Count >= 5) && GameManager.Instance.IsPatientSpawned[apartmentNum])
            {
                apartmentNum = Random.Range(0, 6);
                Debug.Log("New aptNum" + apartmentNum);

                //if (!GameManager.Instance.IsPatientSpawned[apartmentNum])
                //    break;
            }
        }
        catch
        {
            AlertStartAll(_errorTitle, _errorFullString);
        }

        if (!GameManager.Instance.IsPatientSpawned[apartmentNum])
        {
            //PhotonNetwork.Instantiate(_patientMale.name, GameManager.Instance.IncidentPatientSpawns[apartmentNum].position, GameManager.Instance.IncidentPatientSpawns[apartmentNum].rotation);
            if (PatientCreationSpace.PatientCreator.newPatient == null)
            {
                Debug.LogError("no patient loaded!");
                return;
            }
            //1) Get the NewPatientData to be spawned

            //2) switchcase on which models/prefab to spawn from NewPatientData

            //3) Instantiate correct prefab

            object[] instantiationData = new object[2];
            instantiationData[0] = PatientCreationSpace.PatientCreator.newPatient.Name + "_" +
                                   PatientCreationSpace.PatientCreator.newPatient.SureName;

            instantiationData[1] = _crewRoomIndex;

            _photonView.RPC("SpawnPatients_RPC",RpcTarget.MasterClient, apartmentNum, instantiationData);
            //3.5) Grab the Patient component from the instantiated object.
            //4) Set this patients data to the NewPatientData to be spawned
            // go.GetComponent<Patient>().InitializePatientData(PatientCreationSpace.PatientCreator.newPatient);
            // Debug.Log(PatientCreationSpace.PatientCreator.newPatient);
            //go.GetComponent<Patient>().InitializePatientData(PatientCreationSpace.PatientCreator.newPatient);

            //go.GetComponent<Patient>().PhotonView.TransferOwnership(GetCrewLeader());

            _photonView.RPC("UpdateCurrentIncidents", RpcTarget.AllBufferedViaServer, apartmentNum);
            AlertStartAll(_incidentStartTitle, $"{_incidentStartText} {apartmentNum + 1}");
        }
        else
        {
            AlertStartAll(_errorTitle, _errorSomthingWentWrong);
        }
    }

    [PunRPC]
    private void SpawnPatients_RPC(int apartmentNum,object[] instantiationData)
    {
        PhotonNetwork.InstantiateRoomObject(_patientMale.name, GameManager.Instance.IncidentPatientSpawns[apartmentNum].position,
            GameManager.Instance.IncidentPatientSpawns[apartmentNum].rotation, 0, instantiationData);
    }

    public void StartIncident()
    {
        if (!GameManager.Instance.IsAranActive)
        {
            if (_isRandomIncident)
            {
                StartRandomIncident();
                _isRandomIncident = false;
                _startSimulationBtn.interactable = false;
                _currentIncidentNameTMP.text = _currentIncidentName;
            }
            else
            {
                StartIncidentInRandomLocation();
                _startSimulationBtn.interactable = false;
                _currentIncidentNameTMP.text = _currentIncidentName;
            }
        }
        else
        {
            _startSimulationBtn.interactable = false;
            _currentIncidentNameTMP.text = _currentIncidentName;
        }

    }

    public void SetRandomIncident()
    {
        _isRandomIncident = true;
    }

    [PunRPC]
    public void SetIncidentName(string incidentName)
    {
        _currentIncidentName = incidentName;
        _currentIncidentNameTMP.text = _currentIncidentName;
    }

    //public void ChooseIncidents()
    //{
    //    if (_isRandomIncident)
    //    {
    //        _overlay.SetActive(false);
    //        _startSimulationBtn.interactable = true;
    //        _startSimulationTMP.text = _startSimulationText;
    //        return;
    //    }
    //
    //    _chooseSimulationPanel.SetActive(true);
    //}
    public void SetStartIncidentBtn()
    {
        _overlay.SetActive(false);
        _startSimulationBtn.interactable = true;
        _startSimulationTMP.text = _startSimulationText;

        _chooseSimulationPanel.SetActive(false);
    }

    public void ChangeVehicleRequired(bool changeVehicle)
    {
        _photonView.RPC("ChangeVehicleRequiredRPC", RpcTarget.AllBufferedViaServer, changeVehicle);
    }

    //public void SpawnVehicle()
    //{
    //    _photonView.RPC("SpawnVehicle_RPC", RpcTarget.AllBufferedViaServer);
    //}


    // Collision Methods
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("PlayerCollider") && !_playersInRoomList.Contains(playerView))
        {
            // _playersInRoomList.Add(playerView);
            _photonView.RPC("AddingToRoomList_RPC", RpcTarget.AllBufferedViaServer, playerView.Owner.NickName);
            _photonView.RPC("SetToUi_RPC", RpcTarget.AllBufferedViaServer);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    PhotonView playerView = other.GetComponentInParent<PhotonView>();

    //    if (other.CompareTag("PlayerCollider") && !_playersInRoomList.Contains(playerView))
    //    {

    //        _photonView.RPC("AddingToRoomList_RPC", RpcTarget.AllBufferedViaServer, playerView.Owner.NickName);
    //        _playersInRoomList.Add(playerView);
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("PlayerCollider") && _playersInRoomList.Contains(playerView))
        {
            _photonView.RPC("RemovingFromRoomList_RPC", RpcTarget.AllBufferedViaServer, playerView.Owner.NickName);

            _photonView.RPC("UpdateUiNameOnExit", RpcTarget.AllBufferedViaServer, playerView.Owner.NickName);
            //_playersInRoomList.Remove(playerView);
        }
    }

    public void ShowOverlayUI()
    {

        _photonView.RPC("ShowOverlayUI_RPC", RpcTarget.AllBufferedViaServer);
    }
    public void RemoveOverlayUI()
    {
        _photonView.RPC("RemoveOverlayUI_RPC", RpcTarget.AllBufferedViaServer);

    }
    public void ShowSimulationPanelUI()
    {
        _photonView.RPC("ShowSimulationPanelUI_RPC", RpcTarget.AllBufferedViaServer);

    }
    public void RemoveShowSimulationPanelUI()
    {
        _photonView.RPC("RemoveSimulationPanelUI_RPC", RpcTarget.AllBufferedViaServer);

    }
    public void ActivateAranBehaviour(bool isAranActive)
    {
        _photonView.RPC("CheckAranBehaviourRPC", RpcTarget.AllBufferedViaServer, isAranActive);
    }

    // PUN RPC Methods
    // --------------------

    [PunRPC]
    void AddingToRoomList_RPC(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);


        if (currentPlayerView == null)
        {
            Debug.LogError("CurrentPlayer is Null");

            return;
        }

        for (int i = 0; i < 1; i++)
        {
            if (_playersInRoomList.Contains(currentPlayerView))
            {
                continue;
            }
            else
            {
                _playersInRoomList.Add(currentPlayerView);
            }
        }
        //BlockRoomAccess();
        // Debug.LogError("Added to room");

    }

    [PunRPC]
    void RemovingFromRoomList_RPC(string currentPlayer)
    {
        PhotonView currentPlayerView = ActionsManager.Instance.GetPlayerPhotonViewByNickName(currentPlayer);

        if (_playersInRoomList.Contains(currentPlayerView))
        {
            _playersInRoomList.Remove(currentPlayerView);
        }

        Debug.Log("Remove from room");

    }

    void ChangeCrewColors()
    {
       // var color = Random.ColorHSV();

        //crewColor = new Color(color.r, color.b, color.g);
        float r = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);

        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            PlayerData currentPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
            NameTagDisplay desiredPlayerName = _playersInRoomList[i].GetComponentInChildren<NameTagDisplay>();

            desiredPlayerName.text.color = new Color(r,b,g) ;
            currentPlayerData.CrewColor = new Color(r, b, g);
        }




        object[] data = new object[] { r,b,g };
        PhotonNetwork.RaiseEvent(COLOR_CHANGE_EVENT, data, new RaiseEventOptions{CachingOption = EventCaching.AddToRoomCacheGlobal}, SendOptions.SendReliable);
    }

    [PunRPC]
    private void AlertStartAllRPC(string title, string content)
    {
        ActionTemplates.Instance.ShowAlertWindow(title, content);
    }

    [PunRPC]
    void CrewCreateSubmit_RPC(int[] roleIndex,int leaderIndex,int crewIndex)
    {
       // int indexInCrewCounter = 0;
        for (int i = 0; i < roleIndex.Length; i++)
        {
            
            PlayerData desiredPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
            desiredPlayerData.CrewIndex = crewIndex;
           // desiredPlayerData.CrewIndex = indexInCrewCounter;
            desiredPlayerData.UserRole = (Roles)roleIndex[i];
            desiredPlayerData.PhotonView.RPC("SetUserVestRPC", RpcTarget.AllBufferedViaServer, desiredPlayerData.UserRole);
            //indexInCrewCounter++;
        }
        foreach (PhotonView player in _playersInRoomList)
        {
            player.GetComponent<PlayerData>().IsCrewLeader = false;
        }

        PlayerData leaderToBe = _playersInRoomList[leaderIndex].GetComponent<PlayerData>();
        leaderToBe.IsCrewLeader = true;
        ActionsManager.Instance.NextCrewIndex++;
    }

   [PunRPC]

   void SpawnVehicle_RPC(int crewroomIndex)
   {
       VehicleChecker currentPosVehicleChecker = ActionsManager.Instance.VehiclePosTransforms[crewroomIndex - 1].GetComponent<VehicleChecker>();
       object[] crewRoom = new object[1];
       crewRoom[0] = crewroomIndex;
       if (!currentPosVehicleChecker.IsPosOccupied)
       {
           if (_isNatanRequired)
           {
             PhotonNetwork.InstantiateRoomObject(ActionsManager.Instance.NatanPrefab.name, ActionsManager.Instance.VehiclePosTransforms[crewroomIndex - 1].position, ActionsManager.Instance.NatanPrefab.transform.rotation, 0, crewRoom);
           }
           else
           { 
             PhotonNetwork.InstantiateRoomObject(ActionsManager.Instance.AmbulancePrefab.name, ActionsManager.Instance.VehiclePosTransforms[crewroomIndex - 1].position, ActionsManager.Instance.NatanPrefab.transform.rotation, 0, crewRoom);
           }
       }
    }

    void SpawnVehicle()
    {
        _photonView.RPC("SpawnVehicle_RPC", RpcTarget.MasterClient, _crewRoomIndex);
     
    }

    [PunRPC]
    void ShowCrewUI_RPC()
    {
        RoomCrewMenuUI.gameObject.SetActive(true);
        _tvScreen.layer = (int)LayerMasks.Default;
        isUsed = true;

    }

    [PunRPC]
    void SetToUi_RPC()
    {
        SetCrewUITexts();
    }

    [PunRPC]
    void UpdateUiNameOnExit(string playerNickName)
    {
        foreach (TextMeshProUGUI memberName in listOfUiNamesTMP)
        {
            if (playerNickName == memberName.text)
            {
                memberName.text = _waitMemberText;
                // break;
            }
        }
    }

    [PunRPC]
    void RemoveFromUi_RPC(string currentPlayer)
    {
        for (int i = 0; i < _playersInRoomList.Count; i++)
        {
            if (currentPlayer == PhotonNetwork.NickName && listOfUiNamesTMP[i].text.Contains(currentPlayer))
            {
                listOfUiNamesTMP[i].text.Remove(currentPlayer[i]);
            }
        }
    }

    [PunRPC]
    void CloseCrewUI_RPC()
    {
        isUsed = false;
        _tvScreen.layer = (int)LayerMasks.Interactable;
        RoomCrewMenuUI.gameObject.SetActive(false);
    }

    [PunRPC]
    public void ChangeVehicleRequiredRPC(bool changeVehcile)
    {
        _isNatanRequired = changeVehcile;
    }

    [PunRPC]
    private void UpdateCurrentIncidents(int apartmentNum)
    {
        GameManager.Instance.IsPatientSpawned[apartmentNum] = true;
        GameManager.Instance.CurrentIncidentsTransforms.Add(GameManager.Instance.IncidentPatientSpawns[apartmentNum]);
    }

    [PunRPC]
    private void ShowOverlayUI_RPC()
    {
        _overlay.gameObject.SetActive(true);
        _filterredRoaster._photonView.TransferOwnership(_photonView.Owner);
    }
    [PunRPC]
    public void RemoveOverlayUI_RPC()
    {
        _overlay.gameObject.SetActive(false);
    }
    [PunRPC]
    private void ShowSimulationPanelUI_RPC()
    {
        _chooseSimulationPanel.gameObject.SetActive(true);
    }
    [PunRPC]
    private void RemoveSimulationPanelUI_RPC()
    {
        _chooseSimulationPanel.gameObject.SetActive(false);
    }
    [PunRPC]
    private void CheckAranBehaviourRPC(bool isAranActive)
    {
        if (!isAranActive)
            ShowOverlayUI();
        else
            SetStartIncidentBtn();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(CrewLeaderDropDown.value);

            foreach (var dropdown in CrewMemberRoleDropDownList)
            {
                stream.SendNext(dropdown.value);
            }
            //stream.SendNext(_crewRoomIndex);
            stream.SendNext(_isNatanRequired);

            

        }
        else
        {
            CrewLeaderDropDown.value = (int)stream.ReceiveNext();

            foreach (var dropdown in CrewMemberRoleDropDownList)
            {
                dropdown.value = (int)stream.ReceiveNext();
            }
           // _crewRoomIndex = (int)stream.ReceiveNext();
            _isNatanRequired = (bool)stream.ReceiveNext();


        }
    }
}