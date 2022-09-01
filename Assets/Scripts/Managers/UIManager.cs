using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using System.Xml;

public enum EvacRoom { CT_Room, Emergency_Room, Children_Room, Shock_Room, }

public class UIManager : MonoBehaviour
{
    //#region EventSystem
    //[Header("EventSystem")]
    //[SerializeField] private EventSystem _eventSystem;
    //private GameObject? _lastSelectedGameObject;
    //private GameObject _currentSelectedGameObject;
    //#endregion

    public static UIManager Instance;
    [SerializeField] private Vector3 _leaderMenuOffset;
    [SerializeField] private bool _isLeaderMenuOpen, _isPikud10MenuOpen, _isHenyon10MenuOpen, _isPinuy10MenuOpen;

    #region Player UI
    [Header("Player UI Parents")]
    public GameObject CurrentActionBarParent;
    public GameObject MapWindow, ContentPanel;
    public GameObject AmbulanceBar, NatanBar;
    public GameObject AmbulanceNoBagPanel, AmbulanceAmbuPanel, AmbulanceKidsAmbuPanel, AmbulanceMedicPanel, AmbulanceDefibrilationPanel, AmbulanceOxygenPanel, AmbulanceMonitorPanel;
    public GameObject NatanNoBagPanel, NatanAmbuPanel, NatanKidsAmbuPanel, NatanMedicPanel, NatanQuickDrugsPanel, NatanOxygenPanel, NatanMonitorPanel;
    public GameObject TeamLeaderMenu;
    public GameObject TeamLeaderNavigationBtn;

    [Header("Pikud10")]
    public GameObject Pikud10Menu;
    public GameObject DropdownRefua10, DropdownPinuy10, DropdownHenyon10;
    public TMP_Dropdown PlayerListDropdownRefua10, PlayerListDropdownPinuy10, PlayerListDropdownHenyon10;
    public Button Pikud10MenuHandle;
    public Button AssignRefua10, AssignPinuy10, AssignHenyon10;
    public Button MarkUrgent, MarkUnUrgent, MarkVehicles, MarkGeneral, MarkDeceased, MarkBomb;

    [Header("Henyon10")]
    public GameObject Henyon10Menu;
    public Button Henyon10MenuHandle;
    public Button Henyon10CarsMenu,RefreshButton;
    public GameObject CarPrefab;
    public Transform AmbulanceListContent, NatanListContent;

    [Header("Pinuy10")]
    public GameObject Pinuy10Menu;
    public Button Pinuy10MenuHandle;
    public Transform TaggedPatientListContent;
    public Button RefresTaghButton;


    #endregion

    #region Patient UI 
    [Header("Patient UI Parents")]
    public GameObject JoinPatientPopUp;
    public GameObject PatientInfoParent, ActionLogParent, MonitorParent, TagMiunMenu, PatientMenu;
    public Button TagMiunSubmitBtn;

    [Header("Patient UI Texts")]
    public TextMeshProUGUI SureName;
    public TextMeshProUGUI LastName, Id, Age, Gender, PhoneNumber, InsuranceCompany, Adress, Complaint;
    #endregion

    #region Evacuation UI

    [Header("Evacuation UI Drop Down")]
    public TMP_Dropdown _dropDown;

    public GameObject EvacPatientPopUp;
    #endregion

    #region Vehicle UI
    [Header("Vehicle UI Texts")]
    public GameObject VehicleUI;
    
    #endregion

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

        //_lastSelectedGameObject = _currentSelectedGameObject;
        CurrentActionBarParent = AmbulanceBar;
    }

    private void Start()
    {
        AddRoomToList();
    }

    // catch last gameObject to fire an event
    //public GameObject GetLastGameObjectSelected()
    //{
    //    Debug.Log($"Attemting to get last client who tried to join patient");
    //
    //    if (_eventSystem.currentSelectedGameObject != _currentSelectedGameObject)
    //    {
    //        _lastSelectedGameObject = _currentSelectedGameObject;
    //        _currentSelectedGameObject = _eventSystem.currentSelectedGameObject;
    //
    //        //Debug.Log($"{_currentSelectedGameObject.name}");
    //        return _currentSelectedGameObject;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    public void CloseAllPatientWindows()
    {
        JoinPatientPopUp.SetActive(false);
        PatientInfoParent.SetActive(false);
        ActionLogParent.SetActive(false);
        EvacPatientPopUp.SetActive(false);
    }

    public void CloseAllAmbulanceBags(GameObject currentWindow)
    {
        if (!currentWindow.activeInHierarchy)
        {
            AmbulanceNoBagPanel.SetActive(false);
            AmbulanceAmbuPanel.SetActive(false);
            AmbulanceKidsAmbuPanel.SetActive(false);
            AmbulanceMedicPanel.SetActive(false);
            AmbulanceDefibrilationPanel.SetActive(false);
            AmbulanceOxygenPanel.SetActive(false);
            AmbulanceMonitorPanel.SetActive(false);
        }
    }

    public void CloseAllNatanBags(GameObject currentWindow)
    {
        if (!currentWindow.activeInHierarchy)
        {
            NatanNoBagPanel.SetActive(false);
            NatanAmbuPanel.SetActive(false);
            NatanKidsAmbuPanel.SetActive(false);
            NatanMedicPanel.SetActive(false);
            NatanQuickDrugsPanel.SetActive(false);
            NatanOxygenPanel.SetActive(false);
            NatanMonitorPanel.SetActive(false);
        }
    }

    public void PauseHomeBtn()
    {
        MapWindow.SetActive(false);
        ContentPanel.SetActive(true);
    }

    //Evacuation DropDown in UI 
    void AddRoomToList()
    {
        string[] enumNames = Enum.GetNames(typeof(EvacRoom));
        List<string> roomNames = new List<string>(enumNames);

        _dropDown.AddOptions(roomNames);
    }

    public void OpenCloseTopMenu(string menuName) // "Leader", "Pikud10"
    {
        foreach (PhotonView photonView in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            PlayerData desiredPlayerData = photonView.GetComponent<PlayerData>();

            if (photonView.IsMine)
            {
                Debug.Log("Trying To Pull Leader Menu");

                switch (menuName)
                {
                    case "Leader":
                        if (!_isLeaderMenuOpen && desiredPlayerData.IsCrewLeader)
                        {
                            TeamLeaderMenu.transform.position -= _leaderMenuOffset;
                            _isLeaderMenuOpen = true;
                        }
                        else if (_isLeaderMenuOpen && desiredPlayerData.IsCrewLeader)
                        {
                            TeamLeaderMenu.transform.position += _leaderMenuOffset;
                            _isLeaderMenuOpen = false;
                        }
                        break;

                    case "Pikud10":
                        if (!_isPikud10MenuOpen && desiredPlayerData.AranRole == AranRoles.Pikud10)
                        {
                            Pikud10Menu.transform.position -= _leaderMenuOffset;
                            _isPikud10MenuOpen = true;
                        }
                        else if (_isPikud10MenuOpen && desiredPlayerData.AranRole == AranRoles.Pikud10)
                        {
                            Pikud10Menu.transform.position += _leaderMenuOffset;
                            _isPikud10MenuOpen = false;
                        }
                        break;
                    case "Henyon10":
                        if (!_isHenyon10MenuOpen && desiredPlayerData.AranRole == AranRoles.Henyon10)
                        {
                            Henyon10Menu.transform.position -= _leaderMenuOffset;
                            _isHenyon10MenuOpen = true;
                        }
                        else if (_isHenyon10MenuOpen && desiredPlayerData.AranRole == AranRoles.Henyon10)
                        {
                            Henyon10Menu.transform.position += _leaderMenuOffset;
                            _isHenyon10MenuOpen = false;
                        }
                        break;
                    case "Pinuy10":
                        if (!_isPinuy10MenuOpen && desiredPlayerData.AranRole == AranRoles.Pinuy10)
                        {
                            Pinuy10Menu.transform.position -= _leaderMenuOffset;
                            _isPinuy10MenuOpen = true;
                        }
                        else if (_isPinuy10MenuOpen && desiredPlayerData.AranRole == AranRoles.Pinuy10)
                        {
                            Pinuy10Menu.transform.position += _leaderMenuOffset;
                            _isPinuy10MenuOpen = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
