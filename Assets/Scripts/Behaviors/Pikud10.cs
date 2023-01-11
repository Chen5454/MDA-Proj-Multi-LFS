using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using VivoxUnity;

public class Pikud10 : MonoBehaviour, IPunObservable
{
    private PhotonView _photonView => GetComponent<PhotonView>();
    private Coroutine _updatePlayerListCoroutine;
    private GameObject _worldMarkCanvas;
    [SerializeField] private List<GameObject> _allWorldMarks;
    private GameObject _dropdownRefua10, _dropdownPinuy10, _dropdownHenyon10;
    [SerializeField] private GameObject Pikud10Panel;
    private Coroutine updatePlayerListCoroutine;


    private CameraController _camController;

    // private LineRenderer _lineRenderer;
    private LayerMask _groundLayer;
    private Vector2 _targetPos;
    private int _currentMarkIndex;
    private bool _isPikud10MenuOpen;
    private bool _isMarking = false;
    List<Patient> filteredPatients = new List<Patient>();

    [SerializeField] private float _areaOffset = 14.0f, _targetHeight = 0.1f, _worldMarkHeight = 2.5f;

    [Header("Pikod10 UI")]
    public GameObject Pikud10Menu;
    public Camera Pikud10Camera;
    public TMP_Dropdown PlayerListDropdownRefua10, PlayerListDropdownPinuy10, PlayerListDropdownHenyon10;
    public Button TopMenuHandle, AssignRefua10, AssignPinuy10, AssignHenyon10;
    public Button ShowButton,CloseButton;
    public Button[] AllAreaMarkings = new Button[6];


    [Header(" Patient List")] [SerializeField]
    private List<Patient> _taggedPatientList = new List<Patient>();

    [SerializeField] private GameObject _taggedPatientListRow;
    [SerializeField] private Transform _taggedPatientListContent;

    public Button RefreshBtn;
    public Button RefreshPatientBtn;
    [SerializeField] private Transform _ambulanceListContent, _natanListContent;
    [SerializeField] private GameObject _vehicleListRow;

    [SerializeField]
    private List<PhotonView> _natanList = new List<PhotonView>(), _ambulanceList = new List<PhotonView>();

    private Toggle CriticalTGL, UrgentTGL, NonUrgentTGL, DeadTGL;


    private GameObject worldMark;


    //private GameObject worldMark;
    private OwnershipTransfer _transfer;

    [SerializeField] private PlayerData thisPlayerdata;
    [SerializeField] List<Patient> filteredPatientList = new List<Patient>();

    #region MonobehaviourCallbacks


    private void Start()
    {
        thisPlayerdata = GetComponent<PlayerData>();
        _transfer = GetComponent<OwnershipTransfer>();
        GameManager.Instance.Pikud10View = GetComponent<PhotonView>();

        if (_photonView.IsMine)
        {
            _camController = GetComponent<CameraController>();
        }

        InitializePikud10RPC();
        CameraTransmition();
        UIManager.Instance.TeamLeaderMenu.SetActive(false);
        UIManager.Instance.Pikud10Menu.SetActive(true);
        worldMark = UIManager.Instance.MarkerPrefab;

    }

    private void Update()
    {
        if (this.thisPlayerdata.IsPikud10 && _photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_isMarking)
                {
                    //_photonView.RPC("ChooseAreaPos",RpcTarget.AllBufferedViaServer, _currentMarkIndex);
                    ChooseAreaPos(_currentMarkIndex);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = _camController.PlayerCamera.ScreenPointToRay(Input.mousePosition);
                // Casts the ray and get the first game object hit
                if (Physics.Raycast(ray, out RaycastHit hit))
                {

                    if (hit.collider.tag == "test")
                    {
                        string nameIndx = hit.transform.parent.GetComponent<WorldMark>().nameID;
                        _photonView.RPC("DestroyWorldMark_RPC", GetPikud10Player(), nameIndx);
                    }
                }
            }
        }

    }

    [PunRPC]
    private void DestroyWorldMark_RPC(string nameIndex)
    {
        foreach (var mark in _allWorldMarks)
        {
            if (mark.GetComponent<WorldMark>().nameID == nameIndex)
            {
                _allWorldMarks.Remove(mark);
                PhotonNetwork.Destroy(mark);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        UIManager.Instance.TeamLeaderMenu.SetActive(true);
        UIManager.Instance.Pikud10Menu.SetActive(false);
    }

    #endregion

    #region Getters

   public Player GetRefuaPlayer()
   {
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            PlayerController desiredPlayer = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PlayerController>();

            if (_dropdownRefua10.GetComponentInChildren<TextMeshProUGUI>().text ==
                ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Player refuaPlayer = desiredPlayer.GetComponent<PhotonView>().Owner;
                return refuaPlayer;
            }
        }

        return null;
   }
   public Player GetPinuyPlayer()
   {
       for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
       {
           PlayerController desiredPlayer = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PlayerController>();

           if (_dropdownPinuy10.GetComponentInChildren<TextMeshProUGUI>().text ==
               ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
           {
               Player refuaPlayer = desiredPlayer.GetComponent<PhotonView>().Owner;
               return refuaPlayer;
           }
       }

       return null;
   }
   public Player GetHenyonPlayer()
   {
       for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
       {
           PlayerController desiredPlayer = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PlayerController>();

           if (_dropdownHenyon10.GetComponentInChildren<TextMeshProUGUI>().text ==
               ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
           {
               Player refuaPlayer = desiredPlayer.GetComponent<PhotonView>().Owner;
               return refuaPlayer;
           }
       }

       return null;
   }

    public int GetRefuaIndex()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_dropdownRefua10.GetComponentInChildren<TextMeshProUGUI>().text ==
                ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Index = i;
                break;
            }
        }

        return Index;
    }

    public int GetPinoyeIndex()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_dropdownPinuy10.GetComponentInChildren<TextMeshProUGUI>().text ==
                ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Index = i;
                break;
            }
        }

        return Index;
    }

    public int GetHenyonIndex()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_dropdownHenyon10.GetComponentInChildren<TextMeshProUGUI>().text ==
                ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Index = i;
                break;
            }
        }

        return Index;
    }

    #endregion

    #region Coroutines

    IEnumerator HandleDropDownUpdates(float nextUpdate)
    {
        while (true)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != PlayerListDropdownRefua10.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesPikud10", RpcTarget.AllViaServer);
            }

            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != PlayerListDropdownPinuy10.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesPikud10", RpcTarget.AllViaServer);
            }

            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != PlayerListDropdownHenyon10.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesPikud10", RpcTarget.AllViaServer);
            }

            yield return new WaitForSeconds(nextUpdate);
        }
    }

    #endregion

    #region Private Methods

    private void ChooseAreaPos(int markIndex)
    {
        SetMarkRPC(markIndex);
    }

      private void CameraTransmition()
        {
            if (GameManager.Instance.Pikud10TextureRenderer != null)
            {
                Pikud10Camera.targetTexture = GameManager.Instance.Pikud10TextureRenderer;
            }
            else
            {
                Debug.LogError("GameManager.Instance.Pikud10TextureRenderer is null, cannot set target texture");
            }

            _photonView.RPC("SpectatePikudCamera_RPC", RpcTarget.AllViaServer);
        }
    

    #endregion

    #region Public Methods

    public void OpenClosePikud10Menu()
    {
        if (!_isPikud10MenuOpen)
        {
            UIManager.Instance.OpenCloseTopMenu("Pikud10");
            _updatePlayerListCoroutine = StartCoroutine(HandleDropDownUpdates(0.5f));
            _isPikud10MenuOpen = true;
        }
        else
        {
            UIManager.Instance.OpenCloseTopMenu("Pikud10");
            StopCoroutine(_updatePlayerListCoroutine);
            _isPikud10MenuOpen = false;
        }

        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenClosePikud10Menu(); });
    }

    public void OnClickRefua()
    {
        _photonView.RPC("GiveRefuaRole", GetRefuaPlayer(), GetRefuaIndex());
    }

    public void OnClickPinoye()
    {
        _photonView.RPC("GivePinoyeRole", GetPinuyPlayer(), GetPinoyeIndex());
    }

    public void OnClickHenyon()
    {
        _photonView.RPC("GiveHenyonRole", GetHenyonPlayer(), GetHenyonIndex());
    }

    public void CreateMarkedArea(int markIndex)
    {
        _isMarking = true;
        _currentMarkIndex = markIndex;
    }

    public void OnClickMarker(int markIndex) // markerIndex is responsible for choosing the targeted btn
    {
        CreateMarkedArea(markIndex);
    }

    #endregion

    #region PunRPC

    private void InitializePikud10RPC()
    {
        Pikud10Camera = transform.GetChild(transform.childCount - 2).GetComponent<Camera>();
        Pikud10Menu = UIManager.Instance.Pikud10Menu;
        TopMenuHandle = UIManager.Instance.Pikud10MenuHandle;
        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenClosePikud10Menu(); });

        _allWorldMarks = new List<GameObject>();
        _dropdownRefua10 = UIManager.Instance.DropdownRefua10;
        _dropdownPinuy10 = UIManager.Instance.DropdownPinuy10;
        _dropdownHenyon10 = UIManager.Instance.DropdownHenyon10;

        PlayerListDropdownRefua10 = UIManager.Instance.PlayerListDropdownRefua10;
        PlayerListDropdownPinuy10 = UIManager.Instance.PlayerListDropdownPinuy10;
        PlayerListDropdownHenyon10 = UIManager.Instance.PlayerListDropdownHenyon10;

        AssignRefua10 = UIManager.Instance.AssignRefua10;
        AssignPinuy10 = UIManager.Instance.AssignPinuy10;
        AssignHenyon10 = UIManager.Instance.AssignHenyon10;

        AllAreaMarkings[0] = UIManager.Instance.MarkUrgent;
        AllAreaMarkings[1] = UIManager.Instance.MarkUnUrgent;
        AllAreaMarkings[2] = UIManager.Instance.MarkVehicles;
        AllAreaMarkings[3] = UIManager.Instance.MarkGeneral;
        AllAreaMarkings[4] = UIManager.Instance.MarkDeceased;
        AllAreaMarkings[5] = UIManager.Instance.MarkBomb;
        AllAreaMarkings[0].onClick.AddListener(delegate { OnClickMarker(0); });
        AllAreaMarkings[1].onClick.AddListener(delegate { OnClickMarker(1); });
        AllAreaMarkings[2].onClick.AddListener(delegate { OnClickMarker(2); });
        AllAreaMarkings[3].onClick.AddListener(delegate { OnClickMarker(3); });
        AllAreaMarkings[4].onClick.AddListener(delegate { OnClickMarker(4); });
        AllAreaMarkings[5].onClick.AddListener(delegate { OnClickMarker(5); });


        _ambulanceListContent = UIManager.Instance.AmbulanceListContentPikud10;
        _natanListContent = UIManager.Instance.NatanListContentPikud10;
        _vehicleListRow = UIManager.Instance.CarPrefab;


        _taggedPatientListRow = UIManager.Instance.PatientListPrefab;
        _taggedPatientListContent = UIManager.Instance.PatientContentPikud10;


        RefreshBtn = UIManager.Instance.RefreshBtn;
        RefreshBtn.onClick.RemoveAllListeners();
        RefreshBtn.onClick.AddListener(delegate { RefreshVehicleLists(); });

        RefreshPatientBtn = UIManager.Instance.RefreshPatientBtn;
        RefreshPatientBtn.onClick.RemoveAllListeners();
        RefreshPatientBtn.onClick.AddListener(delegate { RefreshPatientLists(); });


        AssignRefua10.onClick.RemoveAllListeners();
        AssignRefua10.onClick.AddListener(delegate { OnClickRefua(); });

        AssignPinuy10.onClick.RemoveAllListeners();
        AssignPinuy10.onClick.AddListener(delegate { OnClickPinoye(); });

        AssignHenyon10.onClick.RemoveAllListeners();
        AssignHenyon10.onClick.AddListener(delegate { OnClickHenyon(); });

        gameObject.AddComponent<LineRenderer>();
     
        _groundLayer = LayerMask.GetMask("Ground");
        _groundLayer += LayerMask.GetMask("Road");


        CriticalTGL = UIManager.Instance.CriticalTGL;
        UrgentTGL = UIManager.Instance.UrgentTGL;
        NonUrgentTGL = UIManager.Instance.NonUrgentTGL;
        DeadTGL = UIManager.Instance.DeadTGL;

        GameManager.Instance.Pikud10View = _photonView;

        Pikud10Panel = UIManager.Instance.Pikud10Window;

        CloseButton = UIManager.Instance.ClosePikudWindow;
        ShowButton = UIManager.Instance.ShowPikudWindow;

        ShowButton.onClick.AddListener(delegate { ShowParentWindow(); });
        CloseButton.onClick.AddListener(delegate { CloseParentWindow(); });

     
    }

    private void SetMarkRPC(int markIndex)
    {

        Ray ray = _camController.PlayerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit areaPosRaycastHit, 20f, _groundLayer))
        {
            _targetPos = new Vector2(areaPosRaycastHit.point.x, areaPosRaycastHit.point.z);
            string IndexRandom = Random.value.ToString();
            _photonView.RPC("SettingPrefabPos_RPC", GetPikud10Player(), markIndex, _targetPos, IndexRandom);
        }

        _isMarking = false;
    }

    [PunRPC]
    private void SettingPrefabPos_RPC(int markIndex, Vector2 targetPos, string IndexRandom)
    {

        //var instantiateWorldMark = Instantiate(worldMark, new Vector3(targetPos.x, _worldMarkHeight, targetPos.y),
        //    Quaternion.identity);
        //ChangeColorForArea(markIndex, instantiateWorldMark);
        //instantiateWorldMark.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite =
        //    instantiateWorldMark.GetComponent<WorldMark>().Marks[markIndex];
        //instantiateWorldMark.GetComponent<WorldMark>().nameID = IndexRandom;
        //_allWorldMarks.Add(instantiateWorldMark);

        object[] instantiationData = new object[4];
        instantiationData[0] = targetPos;
        instantiationData[1] = IndexRandom;
        instantiationData[2] = markIndex;


        var thisPrefab = PhotonNetwork.Instantiate(worldMark.name, new Vector3(targetPos.x, _worldMarkHeight, targetPos.y), Quaternion.identity, 0, instantiationData);
        _allWorldMarks.Add(thisPrefab);
    }
    


    public void RefreshVehicleLists()
    {
        _photonView.RPC("UpdateVehicleListsPikud10RPC", RpcTarget.AllViaServer);
    }


    [PunRPC]
    private void UpdateVehicleListsPikud10RPC()
    {
        _ambulanceList.Clear();
        _natanList.Clear();


        for (int i = 0; i < _ambulanceListContent.childCount; i++)
        {
            Destroy(_ambulanceListContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < _natanListContent.childCount; i++)
        {
            Destroy(_natanListContent.GetChild(i).gameObject);
        }

        _ambulanceList.AddRange(GameManager.Instance.AmbulanceCarList);
        _natanList.AddRange(GameManager.Instance.NatanCarList);

        for (int i = 0; i < _ambulanceList.Count; i++)
        {
            GameObject vehicleListRow = Instantiate(_vehicleListRow, _ambulanceListContent);
            Transform vehicleListRowTr = vehicleListRow.transform;
            PhotonView ambulance = _ambulanceList[i];
            VehicleController ambulanceController = ambulance.GetComponent<VehicleController>();

            string name = ambulanceController.RandomName;
            int num = ambulanceController.RandomNumber;

            vehicleListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {num}";

            if (!ambulanceController.IsBusy)
            {
                vehicleListRowTr.gameObject.SetActive(true);
                vehicleListRowTr.GetChild(1).gameObject.SetActive(false);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                // vehicleListRowTr.gameObject.SetActive(false);

                vehicleListRowTr.GetChild(1).gameObject.SetActive(true);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(false);
            }
        }



        for (int i = 0; i < _natanList.Count; i++)
        {
            GameObject vehicleListRow = Instantiate(_vehicleListRow, _natanListContent);
            Transform vehicleListRowTr = vehicleListRow.transform;
            PhotonView natan = _natanList[i];
            VehicleController natanController = natan.GetComponent<VehicleController>();

            string name = natanController.RandomName;
            int num = natanController.RandomNumber;

            vehicleListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {num}";

            if (!natanController.IsBusy)
            {
                vehicleListRowTr.gameObject.SetActive(true);
                vehicleListRowTr.GetChild(1).gameObject.SetActive(false);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                //vehicleListRowTr.gameObject.SetActive(false);

                vehicleListRowTr.GetChild(1).gameObject.SetActive(true);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    private Player GetPikud10Player()
    {
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            var playerView = ActionsManager.Instance.AllPlayersPhotonViews[i].GetComponent<PlayerData>();
            var PikudPlayer = playerView.GetComponent<Pikud10>();

            if (PikudPlayer!=null && playerView.IsPikud10)
            {
                Player playerIndex = PikudPlayer.GetComponent<PhotonView>().Controller;
                return playerIndex;

            }
        }

        Debug.LogError("There is no Pikud10 Player");
        return null;
    }

    public void RefreshPatientLists()
    {
        _photonView.RPC("UpdatePatientListPikud10RPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void UpdatePatientListPikud10RPC()
    {
        // Clear the current list of patients
        for (int i = 0; i < _taggedPatientListContent.childCount; i++)
        {
            Destroy(_taggedPatientListContent.GetChild(i).gameObject);
        }

        _taggedPatientList.Clear();
        _taggedPatientList.AddRange(GameManager.Instance.AllTaggedPatients);

        // Create a list to store the filtered patients
        List<Patient> filteredPatientList = new List<Patient>();

        // Iterate over the list of tagged patients
        for (int i = 0; i < _taggedPatientList.Count; i++)
        {
            Patient taggedPatient = _taggedPatientList[i];

            // Check the state of each toggle
            if (CriticalTGL.isOn && taggedPatient.NewPatientData.Status == PatientCondition.Critical)
            {
                filteredPatientList.Add(taggedPatient);
            }
            else if (UrgentTGL.isOn && taggedPatient.NewPatientData.Status == PatientCondition.Urgent)
            {
                filteredPatientList.Add(taggedPatient);
            }
            else if (NonUrgentTGL.isOn && taggedPatient.NewPatientData.Status == PatientCondition.Nonurgent)
            {
                filteredPatientList.Add(taggedPatient);
            }
            else if (DeadTGL.isOn && taggedPatient.NewPatientData.Status == PatientCondition.Dead)
            {
                filteredPatientList.Add(taggedPatient);
            }
        }

        // Iterate over the filtered list of patients
        for (int i = 0; i < filteredPatientList.Count; i++)
        {
            Patient taggedPatient = filteredPatientList[i];

            GameObject taggedPatientListRow = Instantiate(_taggedPatientListRow, _taggedPatientListContent);
            Transform taggedPatientListRowTr = taggedPatientListRow.transform;

            string name = taggedPatient.NewPatientData.Name;
            string sureName = taggedPatient.NewPatientData.SureName;

            taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {sureName}";
            taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().text = taggedPatient.HebrewStatus;
            taggedPatientListRowTr.GetChild(2).GetComponent<Button>().gameObject.SetActive(false);
        }
    }
    void ShowParentWindow()
    {
        if (_photonView.IsMine)
        {
            Pikud10Panel.SetActive(true);
            updatePlayerListCoroutine = StartCoroutine(HandleRefreshUpdates(0.5f));
        }

    }
    void CloseParentWindow()
    {
        Pikud10Panel.SetActive(false);
        StopCoroutine(updatePlayerListCoroutine);
    }

    IEnumerator HandleRefreshUpdates(float nextUpdate)
    {
        while (true)
        {
            RefreshVehicleLists();
            RefreshPatientLists();

            yield return new WaitForSeconds(nextUpdate);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isMarking);
            stream.SendNext(_currentMarkIndex);
          stream.SendNext(Pikud10Camera.gameObject.activeSelf);

        }
        else
        {
            _isMarking = (bool)stream.ReceiveNext();
            _currentMarkIndex = (int)stream.ReceiveNext();
            Pikud10Camera.gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }
    #endregion
}
