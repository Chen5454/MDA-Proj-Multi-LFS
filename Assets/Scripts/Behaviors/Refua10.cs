using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Refua10 : MonoBehaviour
{
    private PhotonView _photonView => GetComponent<PhotonView>();

    [SerializeField] private List<Patient> _taggedPatientList = new List<Patient>();
    [SerializeField] private GameObject _taggedPatientListRow;
    [SerializeField] private Transform _taggedPatientListContent;
    public Button TopMenuHandle, RefreshButton;
    private bool _isRefua10MenuOpen;

    private void Start()
    {
        Init();
        GameManager.Instance.Redua10View = _photonView;
    }

    private void Init()
    {
        UIManager.Instance.TeamLeaderMenu.SetActive(false);
        UIManager.Instance.Refua10Menu.SetActive(true);

        _taggedPatientList.AddRange(GameManager.Instance.AllTaggedPatients);


        _taggedPatientListRow = GameManager.Instance.TaggedPatientListRowRefua;
        _taggedPatientListContent = UIManager.Instance.TaggedPatientListContentRefua;

        TopMenuHandle = UIManager.Instance.Refua10MenuHandle;
        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenCloseRefua10Menu(); });


        RefreshButton = UIManager.Instance.RefresTaghButtonRefua;
        RefreshButton.onClick.RemoveAllListeners();
        RefreshButton.onClick.AddListener(delegate { RefreshPatientList(); });
    }

    public void ReTagPatient(Patient patientToReTag, TextMeshProUGUI patientNameTMP)
    {
        patientNameTMP.color = Color.red;
        patientToReTag.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllBufferedViaServer);
        UIManager.Instance.JoinPatientPopUp.SetActive(true);
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
            taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().text = taggedPatient.HebrewStatus;
            taggedPatientListRowTr.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { ReTagPatient(taggedPatient, taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>()); });
        }
    }


    public void OpenCloseRefua10Menu()
    {
        if (!_isRefua10MenuOpen)
        {
            UIManager.Instance.OpenCloseTopMenu("Refua10");
            _isRefua10MenuOpen = true;
        }
        else
        {
            UIManager.Instance.OpenCloseTopMenu("Refua10");
            _isRefua10MenuOpen = false;
        }

        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenCloseRefua10Menu(); });
    }
}
