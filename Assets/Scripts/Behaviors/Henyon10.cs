using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Henyon10 : MonoBehaviour
{
    private PhotonView _photonView => GetComponent<PhotonView>();

    private bool _isHenyon10MenuOpen;
    public Button TopMenuHandle,ShowCarsMenu,RefreshButton;
    public GameObject Henyon10Menu;
    private bool _isMarking = false;
    private int _currentMarkIndex;
    public Button parkingMarker;
    private CameraController _camController;
    private LayerMask _groundLayer;
    private Vector2 _targetPos;
    private GameObject worldMark;
    [SerializeField] private List<GameObject> _allWorldMarks;

    [SerializeField] private float _areaOffset = 14.0f, _targetHeight = 0.1f, _worldMarkHeight = 2.5f;


    [SerializeField] private Transform _ambulanceListContent, _natanListContent;
    [SerializeField] private GameObject _vehicleListRow;
    [SerializeField] private List<PhotonView> _natanList = new List<PhotonView>(), _ambulanceList = new List<PhotonView>();

    void Start()
    {
        GameManager.Instance.Henyon10View = _photonView;
        Init();
        if (_photonView.IsMine)
        {
            _camController = GetComponent<CameraController>();
        }
        worldMark = UIManager.Instance.MarkerPrefab;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isMarking)
            {
                ChooseAreaPos();
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
                    Debug.Log(nameIndx);
                    _photonView.RPC("DestroyWorldMark_RPC", RpcTarget.AllBufferedViaServer, nameIndx);
                }
            }
        }
    }



    public void OpenCloseHenyon10Menu()
    {
        if (!_isHenyon10MenuOpen)
        {
            UIManager.Instance.OpenCloseTopMenu("Henyon10");
            _isHenyon10MenuOpen = true;
        }
        else
        {
            UIManager.Instance.OpenCloseTopMenu("Henyon10");
            _isHenyon10MenuOpen = false;
        }

        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenCloseHenyon10Menu(); });
    }

    public void RefreshVehicleLists()
    {
        _photonView.RPC("UpdateVehicleListsRPC", RpcTarget.AllViaServer);
    }


    public void Init()
    {
        UIManager.Instance.TeamLeaderMenu.SetActive(false);
        UIManager.Instance.Henyon10Menu.SetActive(true);

        _ambulanceListContent = UIManager.Instance.AmbulanceListContent;
        _natanListContent = UIManager.Instance.NatanListContent;
        _vehicleListRow = UIManager.Instance.CarPrefab;
        ShowCarsMenu = UIManager.Instance.Henyon10CarsMenu;
        Henyon10Menu = UIManager.Instance.Henyon10Menu;
        TopMenuHandle = UIManager.Instance.Henyon10MenuHandle;
        RefreshButton = UIManager.Instance.RefreshButton;
        RefreshButton.onClick.RemoveAllListeners();
        RefreshButton.onClick.AddListener(delegate { RefreshVehicleLists(); });
        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenCloseHenyon10Menu(); });


        _allWorldMarks = new List<GameObject>();
        parkingMarker = UIManager.Instance.MarkVehiclesHenyon;
        parkingMarker.onClick.AddListener(delegate{OnClickMarker();});


        _groundLayer = LayerMask.GetMask("Ground");
        _groundLayer += LayerMask.GetMask("Road");
    }



    [PunRPC]
    private void UpdateVehicleListsRPC()
    {
        _ambulanceList.Clear();

        for (int i = 0; i < _natanListContent.childCount; i++)
        {
            Destroy(_ambulanceListContent.GetChild(i).gameObject);
        }

        _ambulanceList.AddRange(GameManager.Instance.AmbulanceCarList);

        for (int i = 0; i < _ambulanceList.Count; i++)
        {
            GameObject vehicleListRow = Instantiate(_vehicleListRow, _ambulanceListContent);
            Transform vehicleListRowTr = vehicleListRow.transform;
            PhotonView ambulance = _ambulanceList[i];
            CarControllerSimple ambulanceController = ambulance.GetComponent<CarControllerSimple>();

            string name = ambulanceController.RandomName;
            int num = ambulanceController.RandomNumber;

            vehicleListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {num}";

            if (ambulanceController.IsInPinuy)
            {
                vehicleListRowTr.GetChild(1).gameObject.SetActive(true);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                vehicleListRowTr.GetChild(1).gameObject.SetActive(false);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(true);
            }
        }

        _natanList.Clear();

        for (int i = 0; i < _natanListContent.childCount; i++)
        {
            Destroy(_natanListContent.GetChild(i).gameObject);
        }

        _natanList.AddRange(GameManager.Instance.NatanCarList);

        for (int i = 0; i < _natanList.Count; i++)
        {
            GameObject vehicleListRow = Instantiate(_vehicleListRow, _natanListContent);
            Transform vehicleListRowTr = vehicleListRow.transform;
            PhotonView natan = _natanList[i];
            CarControllerSimple natanController = natan.GetComponent<CarControllerSimple>();

            string name = natanController.RandomName;
            int num = natanController.RandomNumber;

            vehicleListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {num}";

            if (natanController.IsInPinuy)
            {
                vehicleListRowTr.GetChild(1).gameObject.SetActive(true);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                vehicleListRowTr.GetChild(1).gameObject.SetActive(false);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(true);
            }
        }
    }

    public void CreateMarkedArea()
    {
        _isMarking = true;
    }
    public void OnClickMarker() 
    {
        CreateMarkedArea();
        Debug.Log("Clicked Marker");
    }
    private void ChooseAreaPos()
    {
        SetMarkRPC();
    }

    private void SetMarkRPC()
    {

        Ray ray = _camController.PlayerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit areaPosRaycastHit, 20f, _groundLayer))
        {
            _targetPos = new Vector2(areaPosRaycastHit.point.x, areaPosRaycastHit.point.z);
            string IndexRandom = Random.value.ToString();
            _photonView.RPC("SettingPrefabPos_RPC", RpcTarget.AllBufferedViaServer, _targetPos, IndexRandom);
        }

        _isMarking = false;
    }

    private void ChangeColorForArea(GameObject worldMark)
    {
        var colorArea = worldMark.transform.GetComponentInChildren<Renderer>().material;
        var ColorFillArea = worldMark.transform.Find("FillArea").GetComponentInChildren<Renderer>().material;
        colorArea.color = Color.blue;
        ColorFillArea.color = new Color(0, 0, 1, 0.2f);


    }


    [PunRPC]
    private void SettingPrefabPos_RPC( Vector2 targetPos, string IndexRandom)
    {

        var instantiateWorldMark = Instantiate(worldMark, new Vector3(targetPos.x, _worldMarkHeight, targetPos.y), Quaternion.identity);
        ChangeColorForArea( instantiateWorldMark);
        instantiateWorldMark.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = instantiateWorldMark.GetComponent<WorldMark>().Marks[2];
        instantiateWorldMark.GetComponent<WorldMark>().nameID = IndexRandom;
        _allWorldMarks.Add(instantiateWorldMark);
    }


    [PunRPC]
    private void DestroyWorldMark_RPC(string nameIndex)
    {
        foreach (var mark in _allWorldMarks)
        {
            if (mark.GetComponent<WorldMark>().nameID == nameIndex)
            {
                _allWorldMarks.Remove(mark);
                Destroy(mark);
                break;
            }
        }
    }


}
