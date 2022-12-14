using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Pinuy10 : MonoBehaviour
{
    private PhotonView _photonView => GetComponent<PhotonView>();
    
    [SerializeField] private List<Patient> _taggedPatientList = new List<Patient>();
    [SerializeField] private GameObject _taggedPatientListRow;
    [SerializeField] private Transform _taggedPatientListContent;
    [SerializeField] private List<PhotonView> _natanList = new List<PhotonView>(), _ambulanceList = new List<PhotonView>();
    [SerializeField] private Transform _ambulanceListContent, _natanListContent;
    [SerializeField] private GameObject _vehicleListRow;
    private Patient _currentTaggedPatient;
    private bool _isPinuy10MenuOpen;

    public Button TopMenuHandle, RefreshButton,RefreshCarBtn;
    public GameObject Pinuy10Menu;

    void Start()
    {
        Init();
        GameManager.Instance.Pinuy10View = _photonView;
    }

    public void Init()
    {
        UIManager.Instance.TeamLeaderMenu.SetActive(false);
        UIManager.Instance.Pinuy10Menu.SetActive(true);

        _taggedPatientList.AddRange(GameManager.Instance.AllTaggedPatients);
        _taggedPatientListRow = GameManager.Instance.TaggedPatientListRow;
        _taggedPatientListContent = UIManager.Instance.TaggedPatientListContent;


        Pinuy10Menu = UIManager.Instance.Pinuy10Menu;
        TopMenuHandle = UIManager.Instance.Pinuy10MenuHandle;

        RefreshButton = UIManager.Instance.RefresTaghButton;
        RefreshButton.onClick.RemoveAllListeners();
        RefreshButton.onClick.AddListener(delegate { RefreshPatientList(); });

        RefreshCarBtn = UIManager.Instance.RefresCarshButton;
        RefreshCarBtn.onClick.RemoveAllListeners();
        RefreshCarBtn.onClick.AddListener(delegate { RefreshCarsList(); });



        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenClosePinuy10Menu(); });




        _ambulanceListContent = UIManager.Instance.AmbulanceListContentPinuy10;
        _natanListContent = UIManager.Instance.NatanListContentPinuy10;
        _vehicleListRow = UIManager.Instance.CarPrefab;
    }

    public void OpenClosePinuy10Menu()
    {
        if (!_isPinuy10MenuOpen)
        {
            UIManager.Instance.OpenCloseTopMenu("Pinuy10");
            _isPinuy10MenuOpen = true;
        }
        else
        {
            UIManager.Instance.OpenCloseTopMenu("Pinuy10");
            _isPinuy10MenuOpen = false;
        }

        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenClosePinuy10Menu(); });
    }
    public void UrgentEvacuation(Patient currentTaggedPatient)
    {
        _currentTaggedPatient = currentTaggedPatient;
        currentTaggedPatient.PhotonView.RPC("UrgentEvactionRPC", RpcTarget.AllViaServer);
    }
    public void RefreshPatientList()
    {
        _photonView.RPC("UpdateTaggedPatientListRPC", RpcTarget.AllViaServer);
    }


    public void RefreshCarsList()
    {
        _photonView.RPC("UpdateVehicleListsPinuy10RPC", RpcTarget.AllViaServer);
    }


    [PunRPC]
    private void UpdateVehicleListsPinuy10RPC()
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
              //  vehicleListRowTr.gameObject.SetActive(false);

                vehicleListRowTr.GetChild(1).gameObject.SetActive(true);
                vehicleListRowTr.GetChild(2).gameObject.SetActive(false);
            }
        }
    }



    [PunRPC]
    private void UpdateTaggedPatientListRPC()
    {
        for (int i = 0; i < _taggedPatientListContent.childCount; i++)
        {
            Destroy(_taggedPatientListContent.GetChild(i).gameObject);
        }

        _taggedPatientList.Clear();
        _taggedPatientList.AddRange(GameManager.Instance.AllTaggedPatients);

        for (int i = 0; i < _taggedPatientList.Count; i++)
        {
            GameObject taggedPatientListRow = Instantiate(_taggedPatientListRow, _taggedPatientListContent);
            Transform taggedPatientListRowTr = taggedPatientListRow.transform;
            Patient taggedPatient = _taggedPatientList[i];

            string name = taggedPatient.NewPatientData.Name;
            string sureName = taggedPatient.NewPatientData.SureName;
            //string patientCondition = GameManager.Instance.AllTaggedPatients[i].NewPatientData.Co
            if (!taggedPatient.isEvac)
            {
                taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {sureName}";
                taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().text = taggedPatient.HebrewStatus;
                taggedPatientListRowTr.GetChild(2).GetComponent<Button>().gameObject.SetActive(false);
            }
            else
            {
                taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                taggedPatientListRowTr.GetChild(2).GetComponent<Button>().gameObject.SetActive(false);

            }

            //  taggedPatientListRowTr.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { UrgentEvacuation(taggedPatient); });
        }
    }
}
