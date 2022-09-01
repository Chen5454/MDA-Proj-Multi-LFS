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

    [SerializeField] private Transform _ambulanceListContent, _natanListContent;
    [SerializeField] private GameObject _vehicleListRow;
    [SerializeField] private List<PhotonView> _natanList = new List<PhotonView>(), _ambulanceList = new List<PhotonView>();

    void Start()
    {
        GameManager.Instance.Henyon10View = _photonView;
        Init();
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
    }



    [PunRPC]
    private void UpdateVehicleListsRPC()
    {
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
}
