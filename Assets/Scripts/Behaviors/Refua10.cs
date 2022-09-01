using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Refua10 : MonoBehaviour
{
    private PhotonView _photonView => GetComponent<PhotonView>();

    [SerializeField] private GameObject _taggedPatientListRow;
    [SerializeField] private Transform _taggedPatientListContent;

    private void Start()
    {
        GameManager.Instance.Redua10View = _photonView;
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
        List<Patient> taggedPatientList = new List<Patient>();
        taggedPatientList.Clear();
        taggedPatientList = GameManager.Instance.AllTaggedPatients;

        for (int i = 0; i < taggedPatientList.Count; i++)
        {
            GameObject taggedPatientListRow = Instantiate(_taggedPatientListRow, _taggedPatientListContent);
            Transform taggedPatientListRowTr = taggedPatientListRow.transform;
            Patient taggedPatient = taggedPatientList[i];

            string name = taggedPatientList[i].NewPatientData.Name;
            string sureName = taggedPatientList[i].NewPatientData.SureName;
            //string patientCondition = GameManager.Instance.AllTaggedPatients[i].NewPatientData.Co

            taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{name} {sureName}";
            taggedPatientListRowTr.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"enoN";
            taggedPatientListRowTr.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { ReTagPatient(taggedPatient, taggedPatientListRowTr.GetChild(0).GetComponent<TextMeshProUGUI>()); });
        }
    }
}
