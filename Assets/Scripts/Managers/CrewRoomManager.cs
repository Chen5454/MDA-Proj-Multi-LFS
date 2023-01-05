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

public class CrewRoomManager : MonoBehaviour, IPunObservable
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
    [SerializeField] private Button SicknessButtonDefualt;
    [SerializeField] private Sprite PressedSprite;
    [SerializeField] private GameObject /*_patientMale, _patientFemale, */_chooseIncidentParent, _chooseIncidentMenu, _overlay, _chooseSimulationPanel;
    [SerializeField] private Button _startSimulationBtn;
    [SerializeField] private TextMeshProUGUI _currentIncidentNameTMP, _startSimulationTMP;
    [SerializeField] private TMP_InputField _apartmentNumber;
    [SerializeField] private string _currentIncidentName, _startSimulationText, _startAranSimulationText, _waitMemberText, _incidentStartTitle, _incidentStartText, _errorTitle, _errorFullString, _errorSomthingWentWrong, _errorAptBusy;
    [SerializeField] private bool isUsed, _isNatanRequired, _isRandomIncident;
    [SerializeField] public FilteredPatientsRoster _filterredRoaster;
    [SerializeField] private bool IsAranActive;
    [SerializeField] private bool changeCar;
    
    private OwnershipTransfer _transfer;
    //[SerializeField] private GameObject _crewRoomDoor;
    public int AptNumber;

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
        _tvScreen.layer = (int)LayerMasks.Default;
        _currentIncidentNameTMP.text = _currentIncidentName;
        //GameManager.Instance.AllCrewRooms[_crewRoomIndex - 1] = this;
        //GameManager.Instance.AllCrewRooms.Add(this);
        _crewRoomIndexStatic++;
        _crewRoomIndex = _crewRoomIndexStatic;
        SicknessButtonDefualt.image.sprite = PressedSprite;
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
            object[] data = (object[])obj.CustomData;
            var r = (float)data[0];
            var b = (float)data[1];
            var g = (float)data[2];

            for (int i = 0; i < _playersInRoomList.Count; i++)
            {
                PlayerData currentPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
                NameTagDisplay desiredPlayerName = _playersInRoomList[i].GetComponentInChildren<NameTagDisplay>();

                desiredPlayerName.text.color = new Color(r, b, g);
                currentPlayerData.CrewColor = new Color(r, b, g);

                if (!currentPlayerData.IsDataInitialized)
                {
                    desiredPlayerName.text.color =Color.white;
                    currentPlayerData.CrewColor = Color.white;
                }
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
        rolesList.Remove("None");
        foreach (var dropdown in CrewMemberRoleDropDownList)
        {
            dropdown.AddOptions(rolesList);
        }
    }

    public void CreateCrewSubmit()
    {
        _photonView.RPC("CrewCreateSubmit_RPC", RpcTarget.AllViaServer, GetCrewRolesByEnum(), GetCrewLeaderIndex(), _crewRoomIndex);

        _photonView.RPC("GivesLeaderButton", RpcTarget.AllViaServer, GetCrewLeaderIndex());

        ChangeCrewColors();
    }

    [PunRPC]
    public void GivesLeaderButton(int leaderIndex)
    {
        if (!GameManager.Instance.IsAranActive)
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
        // _photonView.RPC("ShowCrewUI_RPC", RpcTarget.AllBufferedViaServer);
        ShowCrewUI_RPC();

    }
    public void HideCrewRoomMenu()
    {
        //_photonView.RPC("CloseCrewUI_RPC", RpcTarget.AllBufferedViaServer);
        isUsed = false;
        _tvScreen.layer = (int)LayerMasks.Interactable;
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
            if (PatientCreationSpace.PatientCreator.newPatient == null)
            {
                Debug.LogError("no patient loaded!");
                return;
            }

            NewPatientData newPatientData = PatientCreationSpace.PatientCreator.newPatient;

            //1) Get the NewPatientData to be spawned

            //2) switchcase on which models/prefab to spawn from NewPatientData

            //3) Instantiate correct prefab

            string prefabToInstantiate = name;
            if (newPatientData.Gender == PatientGender.Male)
            {
                switch (newPatientData.PatientType)
                {
                    case PatientType.Old:
                        prefabToInstantiate = GameManager.Instance.MalePatients[0].name;
                        break;
                    case PatientType.Grown:
                        prefabToInstantiate = GameManager.Instance.MalePatients[1].name;
                        break;
                    case PatientType.Kid:
                        prefabToInstantiate = GameManager.Instance.KidPatient.name;
                        break;
                    default:
                        Debug.Log("This model don't exist, try another.");
                        break;
                }
            }
            else
            {
                switch (newPatientData.PatientType)
                {
                    case PatientType.Old:
                        prefabToInstantiate = GameManager.Instance.FemalePatients[0].name;
                        break;
                    case PatientType.Grown:
                        prefabToInstantiate = GameManager.Instance.FemalePatients[1].name;
                        break;
                    default:
                        Debug.Log("This model don't exist, try another.");
                        break;
                }
            }

            GameObject go = PhotonNetwork.InstantiateRoomObject(prefabToInstantiate, GameManager.Instance.IncidentPatientSpawns[apartmentNum].position, GameManager.Instance.IncidentPatientSpawns[apartmentNum].rotation);
            go.GetComponent<Patient>().InitializePatientData(newPatientData);
            _photonView.RPC("UpdateCurrentIncidents", RpcTarget.AllViaServer, apartmentNum);
            AptNumber = apartmentNum;
            AlertStartAll(_incidentStartTitle, $"{_incidentStartText} {apartmentNum + 1}");
        }
        else
        {
            AlertStartAll(_errorTitle, _errorSomthingWentWrong);
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
            }
        }
        catch
        {
            AlertStartAll(_errorTitle, _errorFullString);
        }

        if (!GameManager.Instance.IsPatientSpawned[apartmentNum])
        {
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

            _photonView.RPC("SpawnPatients_RPC", RpcTarget.MasterClient, apartmentNum, instantiationData, (int)PatientCreationSpace.PatientCreator.newPatient.Gender, (int)PatientCreationSpace.PatientCreator.newPatient.PatientType);
            //3.5) Grab the Patient component from the instantiated object.
            //4) Set this patients data to the NewPatientData to be spawned
            // go.GetComponent<Patient>().InitializePatientData(PatientCreationSpace.PatientCreator.newPatient);
            // Debug.Log(PatientCreationSpace.PatientCreator.newPatient);
            //go.GetComponent<Patient>().InitializePatientData(PatientCreationSpace.PatientCreator.newPatient);

            //go.GetComponent<Patient>().PhotonView.TransferOwnership(GetCrewLeader());

            _photonView.RPC("UpdateCurrentIncidents", RpcTarget.AllViaServer, apartmentNum);
            AptNumber = apartmentNum;

            AlertStartAll(_incidentStartTitle, $"{_incidentStartText} {apartmentNum + 1}");
        }
        else
        {
            AlertStartAll(_errorTitle, _errorSomthingWentWrong);
        }
    }

    [PunRPC]
    private void SpawnPatients_RPC(int apartmentNum, object[] instantiationData, int patientGender, int patientType)
    {
        string prefabToInstantiate = name;
        PatientGender enumPatientGender = (PatientGender)patientGender;
        PatientType enumPatientType = (PatientType)patientType;
        if (enumPatientGender == PatientGender.Male)
        {
            switch (enumPatientType)
            {
                case PatientType.Old:
                    prefabToInstantiate = GameManager.Instance.MalePatients[0].name;
                    break;
                case PatientType.Grown:
                    prefabToInstantiate = GameManager.Instance.MalePatients[1].name;
                    break;
                case PatientType.Kid:
                    prefabToInstantiate = GameManager.Instance.KidPatient.name;
                    break;
                default:
                    Debug.Log("This model don't exist, try another.");
                    break;
            }
        }
        else
        {
            switch (enumPatientType)
            {
                case PatientType.Old:
                    prefabToInstantiate = GameManager.Instance.FemalePatients[0].name;
                    break;
                case PatientType.Grown:
                    prefabToInstantiate = GameManager.Instance.FemalePatients[1].name;
                    break;
                default:
                    Debug.Log("This model don't exist, try another.");
                    break;
            }
        }

        PhotonNetwork.InstantiateRoomObject(prefabToInstantiate, GameManager.Instance.IncidentPatientSpawns[apartmentNum].position,
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

    public void SetStartIncidentBtn()
    {
        if (GameManager.Instance.IsAranActive)
        {
            _overlay.SetActive(false);
            _startSimulationBtn.interactable = true;
            _startSimulationTMP.text = _startAranSimulationText;

            _chooseSimulationPanel.SetActive(false);
        }
        else
        {
            _overlay.SetActive(false);
            _startSimulationBtn.interactable = true;
            _startSimulationTMP.text = _startSimulationText;

            _chooseSimulationPanel.SetActive(false);
        }

    }

    public void ChangeVehicleRequired(bool changeVehicle)
    {
        changeCar = changeVehicle;
       // _photonView.RPC("ChangeVehicleRequiredRPC", RpcTarget.AllBufferedViaServer, changeVehicle);
       ChangeVehicleRequiredRPC(changeCar);
    }

    // Collision Methods
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
      _tvScreen.layer=(int)LayerMasks.Interactable;
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("PlayerCollider") && !_playersInRoomList.Contains(playerView))
        {
            // _playersInRoomList.Add(playerView);
            _photonView.RPC("AddingToRoomList_RPC", RpcTarget.AllViaServer, playerView.Owner.NickName);
            _photonView.RPC("SetToUi_RPC", RpcTarget.AllViaServer);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        _tvScreen.layer = (int)LayerMasks.Default;
        PhotonView playerView = other.GetComponentInParent<PhotonView>();

        if (other.CompareTag("PlayerCollider") && _playersInRoomList.Contains(playerView))
        {
            _photonView.RPC("RemovingFromRoomList_RPC", RpcTarget.AllViaServer, playerView.Owner.NickName);

            _photonView.RPC("UpdateUiNameOnExit", RpcTarget.AllViaServer, playerView.Owner.NickName);
            //_playersInRoomList.Remove(playerView);
        }
    }

    public void ShowOverlayUI()
    {
        if (!GameManager.Instance.IsAranActive)
            ShowOverlayUI_RPC();
        //  _photonView.RPC("ShowOverlayUI_RPC", RpcTarget.AllBufferedViaServer);
    }
    public void RemoveOverlayUI()
    {
        if (!GameManager.Instance.IsAranActive)
            RemoveOverlayUI_RPC();
        //  _photonView.RPC("RemoveOverlayUI_RPC", RpcTarget.AllBufferedViaServer);
    }
    public void ShowSimulationPanelUI()
    {
        //_photonView.RPC("ShowSimulationPanelUI_RPC", RpcTarget.AllBufferedViaServer);
        ShowSimulationPanelUI_RPC();
    }
    public void RemoveShowSimulationPanelUI()
    {
        // _photonView.RPC("RemoveSimulationPanelUI_RPC", RpcTarget.AllBufferedViaServer);
        RemoveSimulationPanelUI_RPC();
    }
    public void ActivateAranBehaviour(bool isAranActive)
    {
        // _photonView.RPC("CheckAranBehaviourRPC", RpcTarget.AllBufferedViaServer, isAranActive);
        IsAranActive = isAranActive;
        CheckAranBehaviourRPC(IsAranActive);
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

            desiredPlayerName.text.color = new Color(r, b, g);
            currentPlayerData.CrewColor = new Color(r, b, g);
        }




        object[] data = new object[] { r, b, g };
        PhotonNetwork.RaiseEvent(COLOR_CHANGE_EVENT, data, new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCacheGlobal }, SendOptions.SendReliable);
    }

    [PunRPC]
    private void AlertStartAllRPC(string title, string content)
    {
        ActionTemplates.Instance.ShowAlertWindow(title, content);
    }

    [PunRPC]
    void CrewCreateSubmit_RPC(int[] roleIndex, int leaderIndex, int crewIndex)
    {
        
            for (int i = 0; i < roleIndex.Length; i++)
            {
                PlayerData desiredPlayerData = _playersInRoomList[i].GetComponent<PlayerData>();
                var desiredPlayerController = desiredPlayerData.GetComponent<PlayerController>();
               
                    desiredPlayerController.PlayerData.UserRole = Roles.None;
                    desiredPlayerData.IsDataInitialized = false; 
                    desiredPlayerData.CrewIndex = crewIndex;
                    desiredPlayerData.UserRole = (Roles)roleIndex[i];
                    desiredPlayerController.SetUserVestRPC((int)desiredPlayerData.UserRole);
                
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

    void SpawnVehicle_RPC(int crewroomIndex,int aptNumber)
    {
        VehicleChecker currentPosVehicleChecker = ActionsManager.Instance.VehiclePosTransforms[crewroomIndex - 1].GetComponent<VehicleChecker>();
        object[] crewRoom = new object[2];
        crewRoom[0] = crewroomIndex;
        crewRoom[1] = aptNumber;
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

    public void SpawnVehicle()
    {
        _photonView.RPC("SpawnVehicle_RPC", RpcTarget.MasterClient, _crewRoomIndex, AptNumber);

    }

    void ShowCrewUI_RPC()
    {
        if (GameManager.Instance.IsAranActive)
        {
            _startSimulationTMP.text = _startAranSimulationText;
            _startSimulationBtn.interactable = true;
            _chooseIncidentParent.SetActive(false);
        }
        else
        {
            _startSimulationTMP.text = _startSimulationText;
            _chooseIncidentParent.SetActive(true);
        }

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

    public void ChangeVehicleRequiredRPC(bool changeVehcile)
    {
        _isNatanRequired = changeVehcile;
    }

    [PunRPC]
    private void UpdateCurrentIncidents(int apartmentNum)
    {
        GameManager.Instance.IsPatientSpawned[apartmentNum] = true;
        //GameManager.Instance.CurrentIncidentsTransforms.Add(GameManager.Instance.IncidentPatientSpawns[apartmentNum]);
    }

    private void ShowOverlayUI_RPC()
    {
        _overlay.gameObject.SetActive(true);
        _filterredRoaster._photonView.TransferOwnership(_photonView.Owner);
    }
    public void RemoveOverlayUI_RPC()
    {
        _overlay.gameObject.SetActive(false);
    }
    private void ShowSimulationPanelUI_RPC()
    {
        _chooseSimulationPanel.gameObject.SetActive(true);
    }
    private void RemoveSimulationPanelUI_RPC()
    {
        _chooseSimulationPanel.gameObject.SetActive(false);
    }

    private void CheckAranBehaviourRPC(bool isAranActive)
    {
        
        if (!isAranActive)
            ShowOverlayUI();
        else
            SetStartIncidentBtn();
    }

    public void ExitButton()
    {
        foreach (var player in _playersInRoomList)
        {
            PlayerController wantedPlayer = player.GetComponent<PlayerController>();

            if (player.Owner == _photonView.Owner)
            {
                wantedPlayer.UnstuckFromUI();
            }
        }
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
            stream.SendNext(_isNatanRequired);
            stream.SendNext(AptNumber);
            stream.SendNext(RoomCrewMenuUI.gameObject.activeSelf);
            stream.SendNext(_startSimulationBtn.interactable);
            stream.SendNext(_tvScreen.layer);
            stream.SendNext(isUsed);
            stream.SendNext(_overlay.gameObject.activeSelf);
            stream.SendNext(_chooseSimulationPanel.gameObject.activeSelf);
            stream.SendNext(IsAranActive);
            stream.SendNext(changeCar);
            stream.SendNext(_crewRoomIndex);


        }
        else
        {
            CrewLeaderDropDown.value = (int)stream.ReceiveNext();

            foreach (var dropdown in CrewMemberRoleDropDownList)
            {
                dropdown.value = (int)stream.ReceiveNext();
            }
            _isNatanRequired = (bool)stream.ReceiveNext();
            AptNumber = (int)stream.ReceiveNext();
            RoomCrewMenuUI.gameObject.SetActive((bool)stream.ReceiveNext());
            _startSimulationBtn.interactable = (bool) stream.ReceiveNext();
            _tvScreen.layer = (int) stream.ReceiveNext();
            isUsed = (bool) stream.ReceiveNext();
            _overlay.gameObject.SetActive((bool)stream.ReceiveNext());
            _chooseSimulationPanel.gameObject.SetActive((bool)stream.ReceiveNext());
            IsAranActive = (bool) stream.ReceiveNext();
            changeCar = (bool) stream.ReceiveNext();
            _crewRoomIndex = (int) stream.ReceiveNext();
        }
    }
}