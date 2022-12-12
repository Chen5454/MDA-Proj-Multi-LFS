using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using VivoxUnity;

public class Pikud10 : MonoBehaviour,IPunObservable
{
    private PhotonView _photonView => GetComponent<PhotonView>();
    private Coroutine _updatePlayerListCoroutine;
    private GameObject _worldMarkCanvas;
   [SerializeField] private List<GameObject> _allWorldMarks;
    private GameObject _dropdownRefua10, _dropdownPinuy10, _dropdownHenyon10;
    private CameraController _camController;
   // private LineRenderer _lineRenderer;
    private LayerMask _groundLayer;
    private Vector2 _targetPos;
    private int _currentMarkIndex;
    private bool _isPikud10MenuOpen;
    private bool _isMarking = false;
 
    [SerializeField] private float _areaOffset = 14.0f, _targetHeight = 0.1f, _worldMarkHeight = 2.5f;

    [Header("Pikod10 UI")] public GameObject Pikud10Menu;
    public Camera Pikud10Camera;
    public TMP_Dropdown PlayerListDropdownRefua10, PlayerListDropdownPinuy10, PlayerListDropdownHenyon10;
    public Button TopMenuHandle, AssignRefua10, AssignPinuy10, AssignHenyon10;
    public Button[] AllAreaMarkings = new Button[6];

    private GameObject worldMark;
    //private GameObject worldMark;
    private OwnershipTransfer _transfer;

    [SerializeField] private PlayerData thisPlayerdata;
    #region MonobehaviourCallbacks


    private void Start()
    {
        thisPlayerdata = GetComponent<PlayerData>();
        _transfer = GetComponent<OwnershipTransfer>();

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
                        Debug.Log(nameIndx);
                        _photonView.RPC("DestroyWorldMark_RPC", RpcTarget.AllBufferedViaServer, nameIndx);
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
                Destroy(mark);
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
    public int GetRefuaIndex()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_dropdownRefua10.GetComponentInChildren<TextMeshProUGUI>().text == ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
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
            if (_dropdownPinuy10.GetComponentInChildren<TextMeshProUGUI>().text == ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
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
            if (_dropdownHenyon10.GetComponentInChildren<TextMeshProUGUI>().text == ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
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
                _photonView.RPC("DropdownPlayersNickNamesPikud10", RpcTarget.AllBufferedViaServer);
            }

            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != PlayerListDropdownPinuy10.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesPikud10", RpcTarget.AllBufferedViaServer);
            }

            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != PlayerListDropdownHenyon10.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesPikud10", RpcTarget.AllBufferedViaServer);
            }

            yield return new WaitForSeconds(nextUpdate);
        }
    }
    #endregion

    #region Private Methods
    //private void InitializePikud10()
    //{
    //    _photonView.RPC("InitializePikud10RPC", RpcTarget.AllViaServer);
    //}
    //private void SetLineTargetPos()
    //{
    //    _lineRenderer.SetPosition(0, new Vector3(_targetPos.x - _areaOffset, _targetHeight, _targetPos.y));
    //    _lineRenderer.SetPosition(1,
    //        new Vector3(_targetPos.x - _areaOffset, _targetHeight, _targetPos.y + _areaOffset));
    //    _lineRenderer.SetPosition(2,
    //        new Vector3(_targetPos.x + _areaOffset, _targetHeight, _targetPos.y + _areaOffset));
    //    _lineRenderer.SetPosition(3,
    //        new Vector3(_targetPos.x + _areaOffset, _targetHeight, _targetPos.y - _areaOffset));
    //    _lineRenderer.SetPosition(4,
    //        new Vector3(_targetPos.x - _areaOffset, _targetHeight, _targetPos.y - _areaOffset));
    //    _lineRenderer.SetPosition(5, new Vector3(_targetPos.x - _areaOffset, _targetHeight, _targetPos.y));
    //}
    private void ChooseAreaPos(int markIndex)
    {
        SetMarkRPC(markIndex);
    }
        private void CameraTransmition()
    {
        //GameManager.Instance.Pikud10TextureRenderer = transform.GetChild(1).GetComponent<RenderTexture>();
        _photonView.RPC("SpectatePikudCamera_RPC", RpcTarget.AllBufferedViaServer);
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
        _photonView.RPC("GiveRefuaRole", RpcTarget.AllBufferedViaServer, GetRefuaIndex());
    }
    public void OnClickPinoye()
    {
        _photonView.RPC("GivePinoyeRole", RpcTarget.AllBufferedViaServer, GetPinoyeIndex());
    }
    public void OnClickHenyon()
    {
        _photonView.RPC("GiveHenyonRole", RpcTarget.AllBufferedViaServer, GetHenyonIndex());
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


        AssignRefua10.onClick.RemoveAllListeners();
        AssignRefua10.onClick.AddListener(delegate { OnClickRefua(); });

        AssignPinuy10.onClick.RemoveAllListeners();
        AssignPinuy10.onClick.AddListener(delegate { OnClickPinoye(); });

        AssignHenyon10.onClick.RemoveAllListeners();
        AssignHenyon10.onClick.AddListener(delegate { OnClickHenyon(); });

        gameObject.AddComponent<LineRenderer>();
       // _lineRenderer = GetComponent<LineRenderer>();
       // _lineRenderer.positionCount = 6;
       // _lineRenderer.widthMultiplier = 0.1f;
        //_lineRenderer.material = GameManager.Instance.LineMaterial;
        _groundLayer = LayerMask.GetMask("Ground");
        _groundLayer += LayerMask.GetMask("Road");

        GameManager.Instance.Pikud10View = _photonView;
    }

    private void SetMarkRPC(int markIndex)
    {

        Ray ray = _camController.PlayerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit areaPosRaycastHit, 20f, _groundLayer))
        {
            _targetPos = new Vector2(areaPosRaycastHit.point.x, areaPosRaycastHit.point.z);
            string IndexRandom = Random.value.ToString();
            _photonView.RPC("SettingPrefabPos_RPC",RpcTarget.AllBufferedViaServer,markIndex, _targetPos, IndexRandom);
        }

        _isMarking = false;
    }

    [PunRPC]
    private void SettingPrefabPos_RPC(int markIndex, Vector2 targetPos, string IndexRandom)
    {
        
        var instantiateWorldMark = Instantiate(worldMark, new Vector3(targetPos.x, _worldMarkHeight, targetPos.y), Quaternion.identity);
        ChangeColorForArea(markIndex, instantiateWorldMark);
            instantiateWorldMark.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = instantiateWorldMark.GetComponent<WorldMark>().Marks[markIndex];
            instantiateWorldMark.GetComponent<WorldMark>().nameID = IndexRandom;
        _allWorldMarks.Add(instantiateWorldMark);
    }

    //[PunRPC]
    //private void DestoryPrefabPos_RPC()
    //{
    //    Destroy(hit.transform.root.gameObject);
    //    Debug.Log("Hit");
    //}


    private void ChangeColorForArea(int markIndex, GameObject worldMark)
    {
        var colorArea = worldMark.transform.GetComponentInChildren<Renderer>().material;
        var ColorFillArea = worldMark.transform.Find("FillArea").GetComponentInChildren<Renderer>().material;

        switch (markIndex)
        {
            case 0:
                colorArea.color = Color.red;
                ColorFillArea.color = new Color(1,0,0,0.2f);
                break;
            case 1:
                colorArea.color = Color.green;
                ColorFillArea.color = new Color(0, 1, 0, 0.2f);
                break;
            case 2:
                colorArea.color = Color.blue;
                ColorFillArea.color = new Color(0, 0, 1, 0.2f);
                break;
            case 3:
                colorArea.color = Color.white;
                ColorFillArea.color = new Color(1, 1, 1, 0.2f);
                break;
            case 4:
                colorArea.color = Color.black;
                ColorFillArea.color = new Color(0, 0, 0, 0.2f);
                break;
            case 5:
                colorArea.color = Color.yellow;
                ColorFillArea.color = new Color(1, 1, 0, 0.2f);
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isMarking);
            stream.SendNext(_currentMarkIndex);
          //stream.SendNext(_targetPos);

        }
        else
        {
            _isMarking = (bool)stream.ReceiveNext();
            _currentMarkIndex = (int)stream.ReceiveNext();
          //_targetPos = (Vector2)stream.ReceiveNext();

        }
    }
    #endregion
}
