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
    [SerializeField] private GameObject Refua10Panel;
    [SerializeField] private Transform _taggedPatientListContent;
    public Button TopMenuHandle, RefreshButton,ShowButton,CloseButton;
    private bool _isRefua10MenuOpen;
    private Coroutine updatePlayerListCoroutine;
    private Toggle CriticalTGL, UrgentTGL, NonUrgentTGL, DeadTGL;


    private void Start()
    {
        Init();
        GameManager.Instance.Redua10View = _photonView;
    }

    private void Init()
    {
        UIManager.Instance.TeamLeaderMenu.SetActive(false);
        UIManager.Instance.Refua10Menu.SetActive(true);
        Refua10Panel = UIManager.Instance.Refua10Window;

        CloseButton = UIManager.Instance.CloseRefuaWindow;
        ShowButton = UIManager.Instance.ShowRefuaWindow;

        ShowButton.onClick.AddListener(delegate{ShowPatientWindow();});
        CloseButton.onClick.AddListener(delegate { ClosePatientWindow();});


        _taggedPatientList.AddRange(GameManager.Instance.AllTaggedPatients);

        _taggedPatientListRow = GameManager.Instance.TaggedPatientListRowRefua;
        _taggedPatientListContent = UIManager.Instance.TaggedPatientListContentRefua;

        TopMenuHandle = UIManager.Instance.Refua10MenuHandle;
        TopMenuHandle.onClick.RemoveAllListeners();
        TopMenuHandle.onClick.AddListener(delegate { OpenCloseRefua10Menu(); });


        RefreshButton = UIManager.Instance.RefresTaghButtonRefua;
        RefreshButton.onClick.RemoveAllListeners();
        RefreshButton.onClick.AddListener(delegate { RefreshPatientList(); });

        CriticalTGL = UIManager.Instance.CriticalTGLRefua;
        UrgentTGL = UIManager.Instance.UrgentTGLRefua;
        NonUrgentTGL = UIManager.Instance.NonUrgentTGLRefua;
        DeadTGL = UIManager.Instance.DeadTGLRefua;
    }

    public void ReTagPatient(Patient patientToReTag, TextMeshProUGUI patientNameTMP)
    {
        patientNameTMP.color = Color.red;
        patientToReTag.PhotonView.RPC("UpdatePatientInfoDisplay", RpcTarget.AllBufferedViaServer);
        UIManager.Instance.JoinPatientPopUp.SetActive(true);
    }

    public void RefreshPatientList()
    {
        // _photonView.RPC("UpdateTaggedPatientListRPC", RpcTarget.AllViaServer);
        UpdateTaggedPatientListRPC();
    }

    private void UpdateTaggedPatientListRPC()
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

    void ShowPatientWindow()
    {
        if (_photonView.IsMine)
        {
            Refua10Panel.SetActive(true);
            updatePlayerListCoroutine = StartCoroutine(HandleRefreshUpdates(0.5f));
        }
     
    }
    void ClosePatientWindow()
    {
        Refua10Panel.SetActive(false);
        StopCoroutine(updatePlayerListCoroutine);
    }

    IEnumerator HandleRefreshUpdates(float nextUpdate)
    {
        while (true)
        {

            RefreshPatientList();

            yield return new WaitForSeconds(nextUpdate);
        }
    }
}
