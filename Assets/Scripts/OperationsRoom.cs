using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperationsRoom : MonoBehaviour, IPunObservable
{
    private Coroutine updatePlayerListCoroutine;
    private PhotonView _photonView;
    private OwnershipTransfer _transfer;
    [SerializeField] private bool isUsed;

    [Header("General")]
    public GameObject MokdnMenuUI;

    [Header("Pikud 10")]
    [SerializeField] private GameObject _DropDown;
    [SerializeField] private TMP_Dropdown _playerListDropDown;

    [Header("Vehicle Lists")]
    [SerializeField] private List<PhotonView> _natanList = new List<PhotonView>(), _ambulanceList = new List<PhotonView>();
    [SerializeField] private GameObject _vehicleListRow;
    [SerializeField] private Transform _ambulanceListContent, _natanListContent;

    [Header("Tagged Patient List")]
    [SerializeField] private List<Patient> _taggedPatientList;
    [SerializeField] private GameObject _taggedPatientListRow;
    [SerializeField] private Transform _taggedPatientListContent;

    #region MonobehaviourCallbacks
    private void Start()
    {
        _transfer = GetComponent<OwnershipTransfer>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            InteractUICrew();
        }
        else
        {
            MokdnMenuUI.GetComponentInParent<CanvasGroup>().interactable = false;

        }
    }
    #endregion

    #region Private Methods
    private void InteractUICrew()
    {
        if (isUsed)
        {
            MokdnMenuUI.GetComponentInParent<CanvasGroup>().interactable = true;

        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region OnClick
    public int GetPikud10Index()
    {
        int Index = 0;

        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (_DropDown.GetComponentInChildren<TextMeshProUGUI>().text == ActionsManager.Instance.AllPlayersPhotonViews[i].Owner.NickName)
            {
                Index = i;
            }
        }
        return Index;
    }
    public void ShowMokdanMenu()
    {
        _transfer.TvOwner();
        _photonView.RPC("ShowMokdanMenu_RPC", RpcTarget.AllBufferedViaServer);
        updatePlayerListCoroutine = StartCoroutine(HandleDropDownUpdates(0.5f));

    }
    public void RefreshPatientList()
    {
        _photonView.RPC("UpdateTaggedPatientListRPC", RpcTarget.AllViaServer);
    }
    public void RefreshVehicleLists()
    {
        _photonView.RPC("UpdateVehicleListsRPC", RpcTarget.AllViaServer);
    }
    public void GivePikudRoleClick()
    {
        _photonView.RPC("GivePikudRole", RpcTarget.AllBufferedViaServer, GetPikud10Index());
    }
    public void CloseMokdanRoomMenu()
    {
        StopCoroutine(updatePlayerListCoroutine);
        _photonView.RPC("CloseMokdanMenu_RPC", RpcTarget.AllBufferedViaServer);
    }
    public void ReTagPatient(Patient patientToReTag, TextMeshProUGUI patientNameTMP)
    {
        patientNameTMP.color = Color.red;
        patientToReTag.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllBufferedViaServer);
        UIManager.Instance.JoinPatientPopUp.SetActive(true);
    }
    #endregion

    #region PunRPC
    [PunRPC]
    public void ShowMokdanMenu_RPC()
    {
        MokdnMenuUI.SetActive(true);
        isUsed = true;
    }

    [PunRPC]
    public void CloseMokdanMenu_RPC()
    {
        isUsed = false;
        MokdnMenuUI.SetActive(false);
    }

    [PunRPC]
    private void UpdateTaggedPatientListRPC()
    {
        for (int i = 0; i < _taggedPatientListContent.childCount; i++)
        {
            Destroy(_taggedPatientListContent.GetChild(i).gameObject);
        }
        _taggedPatientList.Clear();
        _taggedPatientList = GameManager.Instance.AllTaggedPatients;

        for (int i = 0; i < _taggedPatientList.Count; i++)
        {
            GameObject taggedPatientListRow = Instantiate(_taggedPatientListRow, _taggedPatientListContent);
            Transform taggedPatientListRowTr = taggedPatientListRow.transform;
            Patient taggedPatient = _taggedPatientList[i];

            string name = taggedPatient.NewPatientData.Name;
            string sureName = taggedPatient.NewPatientData.SureName;
            //string patientCondition = GameManager.Instance.AllTaggedPatients[i].NewPatientData.Co

            taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {sureName}";
            taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"enoN";
            taggedPatientListRowTr.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { ReTagPatient(taggedPatient, taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>()); });
        }  
    }

    [PunRPC]
    private void UpdateVehicleListsRPC()
    {
        //_ambulanceList.Clear();
        _natanList.Clear();

        /* Ambulance Clear gameObjects
         * for (int i = 0; i < _ambulanceListContent.childCount; i++)
         * {
         *     Destroy(_ambulanceListContent.GetChild(i).gameObject);
         * }
         */

        for (int i = 0; i < _natanListContent.childCount; i++)
        {
            Destroy(_natanListContent.GetChild(i).gameObject);
        }

        //_ambulanceList.AddRange(GameManager.Instance.AmbulanceList);
        _natanList.AddRange(GameManager.Instance.NatanCarList);

        /* Ambulance Logic
         * for (int i = 0; i < _ambulanceList.Count; i++)
         * {
         *     GameObject vehicleListRow = Instantiate(_vehicleListRow, _amb
         *     Transform vehicleListRowTr = vehicleListRow.transform;
         *     PhotonView ambulance = _natanList[i];
         *     CarControllerSimple ambulanceController = ambulance.GetCompon
         * 
         *     string name = ambulanceController.RandomName;
         *     int num = ambulanceController.RandomNumber;
         * 
         *     vehicleListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().
         * 
         *     if (ambulanceController.IsInPinuy)
         *     {
         *         vehicleListRowTr.GetChild(1).gameObject.SetActive(true);
         *         vehicleListRowTr.GetChild(2).gameObject.SetActive(false);
         *     }
         *     else
         *     {
         *         vehicleListRowTr.GetChild(1).gameObject.SetActive(false);
         *         vehicleListRowTr.GetChild(2).gameObject.SetActive(true);
         *     }
         * }
         */

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

    [PunRPC]
    private void DropdownPlayersNickNamesPikud()
    {
        List<string> value = new List<string>();
        foreach (PhotonView player in ActionsManager.Instance.AllPlayersPhotonViews)
        {
            value.Add(player.Owner.NickName);
        }
        _playerListDropDown.ClearOptions();
        _playerListDropDown.AddOptions(value);
       
    }

    [PunRPC]
    public void GivePikudRole(int index)
    {
        PlayerData chosenPlayerData = ActionsManager.Instance.AllPlayersPhotonViews[index].GetComponent<PlayerData>();
        chosenPlayerData.IsPikud10 = true;
        chosenPlayerData.AssignAranRole(AranRoles.Pikud10);
    }
    #endregion

    #region Coroutines
    IEnumerator HandleDropDownUpdates(float nextUpdate)
    {
        while (true)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews.Count != _playerListDropDown.options.Count)
            {
                _photonView.RPC("DropdownPlayersNickNamesPikud", RpcTarget.AllBufferedViaServer);
            }

            yield return new WaitForSeconds(nextUpdate);
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_playerListDropDown.value);
        }
        else
        {
            _playerListDropDown.value = (int)stream.ReceiveNext();
        }
    }
}
