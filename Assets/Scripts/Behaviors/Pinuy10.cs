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

    private Patient _currentTaggedPatient;
    private bool _isPinuy10MenuOpen;

    public Button TopMenuHandle, RefreshButton;
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
        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenClosePinuy10Menu(); });
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

            taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {sureName}";
            taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"enoN";
            taggedPatientListRowTr.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { UrgentEvacuation(taggedPatient); });
        }
    }
}
