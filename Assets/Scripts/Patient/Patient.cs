using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public enum Clothing { FullyClothed, ShirtOnly, PantsOnly, UnderwearOnly }
public enum Props { Venflon, BloodPressureSleeve, Ambu, HeadVice, OxygenMask, Tube, NeckBrace, ThroatTube, Asherman, ECG }

public class Patient : MonoBehaviour
{
    #region Photon
    [Header("Photon")]
    public PhotonView PhotonView;
    #endregion

    #region Script References
    [Header("Data & Scripts")]
    public PatientData PatientData;
    public NewPatientData NewPatientData;
    public List<ActionSequence> ActionSequences;
    #endregion

    #region UI
    [Header("UI - by UI Manager")]
    public Image MonitorWindow;

    [Header("World Canvas")]
    public GameObject WorldCanvas;
    public GameObject UrgentEvacuationCanvas;
    #endregion

    #region GameObjects
    [Header("Props")]
    public List<GameObject> KidPropList;
    public List<GameObject> PropList;
    public List<GameObject> OldPropList;

    [Header("Bandages")]
    public bool UseTourniquet = false;
    [SerializeField] private List<GameObject> _unusedBandagesOnPatient;
    [SerializeField] private List<Mesh> _bandageMeshList, _tourniquetMeshList;
    #endregion

    #region Transforms
    [Header("Treatment Positions")]
    public Transform ChestPosPlayerTransform;
    public Transform ChestPosEquipmentTransform, HeadPosPlayerTransform, HeadPosEquipmentTransform, LegPosPlayerTrasform;
    #endregion

    #region Joined Player & Crews Lists
    [Header("Joined Players & Crews Lists")]
    public List<PlayerData> NearbyUsers;
    public List<PlayerData> TreatingUsers;
    public List<PlayerData> AllUsersTreatedThisPatient;
    public List<int> TreatingCrews;
    public List<int> AllCrewTreatedThisPatient;
    #endregion

    #region Model Related
    [Header("Appearance Material")]
    public Material FullyClothedMaterial;
    public Material ShirtOnlyMaterial, PantsOnlyMaterial, UnderwearOnlyMaterial;

    [Header("Renderer")]
    public Renderer PatientRenderer;
    #endregion

    #region Monovehavior Callbacks
    private void Awake()
    {
        PatientRenderer.material = FullyClothedMaterial;
        DontDestroyOnLoad(gameObject.transform);
    }

    private void Start()
    {
        GameManager.Instance.AllPatients.Add(this);
        MonitorWindow = UIManager.Instance.MonitorParent.transform.GetChild(0).GetChild(0).GetComponent<Image>();

        UIManager.Instance.PatientInfoParent = GameManager.Instance.IsAranActive ? UIManager.Instance.TagMiunMenu : UIManager.Instance.PatientMenu;
    }

    private void OnDestroy()
    {
        GameManager.Instance.AllPatients.Remove(this);

        if (GameManager.Instance.AllTaggedPatients.Contains(this))
            GameManager.Instance.AllTaggedPatients.Remove(this);
    }
    #endregion

    #region Collision & Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerData possiblePlayer))
        {
            return;
        }
        else if (!NearbyUsers.Contains(possiblePlayer))
        {
            WorldCanvas.SetActive(true);
            NearbyUsers.Add(possiblePlayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerData possiblePlayer))
        {
            if (!NearbyUsers.Contains(possiblePlayer))
            {
                return;
            }
            else
            {
                WorldCanvas.SetActive(false);
                NearbyUsers.Remove(possiblePlayer);
            }
        }
    }
    #endregion

    #region Tests
    [ContextMenu("Test Measurements")]
    public void TestMeasurements()
    {
        PatientData.SetMeasurementValues(new string[] { "", "", "", "fourth", "", "", "", "last" });
    }
    #endregion

    #region Enumerators
    private IEnumerator PauseBeforeBandage(int bandageIndex)
    {
        yield return new WaitForSeconds(0.1f);
        PhotonView.RPC("RemoveBandageFromUnusedListRPC", RpcTarget.AllBufferedViaServer, bandageIndex);
    }
    #endregion

    #region Public Methods
    public bool IsPlayerJoined(PlayerData playerData)
    {
        Debug.Log("Attempting to check if player is joined");

        if (TreatingUsers.Contains(playerData))
        {
            Debug.Log("Checked if player is joined, it is true");
            return true;
        }
        else
        {
            Debug.Log("Checked if player is joined, it is false");
            return false;
        }
    }

    public void OnInteracted()
    {
        ActionsManager.Instance.OnPatientClicked();
        UIManager.Instance.TagMiunSubmitBtn.onClick.AddListener(delegate { AddToTaggedPatientsList(); });
    }

    public void SetUnusedBandages(bool enableBandage)
    {
        foreach (GameObject bandage in _unusedBandagesOnPatient)
        {
            bandage.SetActive(enableBandage);
        }
    }

    public void EnableBandage(GameObject bandage)
    {
        int bandageIndex = 0;

        // loops through unused bandages list and find current bandage index => insert index as argument for RPC method (GameObjects are not passable arguments in RPC)
        for (int i = 0; i < _unusedBandagesOnPatient.Count; i++)
        {
            if (_unusedBandagesOnPatient[i].name == bandage.name)
            {
                bandageIndex = i;
                //PhotonView.RPC("RemoveBandageFromUnusedListRPC", RpcTarget.AllBufferedViaServer, bandageIndex);
                StartCoroutine(PauseBeforeBandage(bandageIndex));
            }
        }
        //_unUsedBandagesOnPatient.Remove(bandage);
        SetUnusedBandages(false);
    }

    public void InitializePatientData(NewPatientData newPatientDataFromSO)
    {
        NewPatientData = new NewPatientData(newPatientDataFromSO);
    }

    public void AddToTaggedPatientsList()
    {
        if (!GameManager.Instance.AllTaggedPatients.Contains(this) && GameManager.Instance.IsAranActive)
        {
            GameManager.Instance.AllTaggedPatients.Add(this);
        }
    }
    #endregion

    #region PunRPC invoke by Patient
    [PunRPC]
    public void AddUserToTreatingLists(string currentPlayer)
    {
        // recieve crew index as int/ string no need for PatientData
        PlayerData currentPlayerData = GameObject.Find(currentPlayer).GetComponent<PlayerData>();
        //PlayerData currentPlayerData = currentPlayer != null ? currentPlayer as PlayerData : null;

        if (currentPlayerData == null)
        {
            return;
        }

        for (int i = 0; i < 1; i++)
        {
            if (TreatingUsers.Contains(currentPlayerData))
            {
                continue;
            }
            else
            {
                TreatingUsers.Add(currentPlayerData);

                if (AllUsersTreatedThisPatient.Contains(currentPlayerData))
                {
                    continue;
                }
                AllUsersTreatedThisPatient.Add(currentPlayerData);
            }

            if (TreatingCrews.Contains(currentPlayerData.CrewIndex))
            {
                return;
            }
            else
            {
                TreatingCrews.Add(currentPlayerData.CrewIndex);
                AllCrewTreatedThisPatient.Add(currentPlayerData.CrewIndex);
            }
        }
    }

    [PunRPC]
    public void UpdatePatientInfoDisplay()
    {
        UIManager.Instance.SureName.text = NewPatientData.Name;
        UIManager.Instance.LastName.text = NewPatientData.SureName;
        UIManager.Instance.Gender.text = NewPatientData.Gender;
        UIManager.Instance.Adress.text = NewPatientData.AddressLocation;
        UIManager.Instance.InsuranceCompany.text = NewPatientData.MedicalCompany;
        UIManager.Instance.Complaint.text = NewPatientData.Complaint;

        UIManager.Instance.Age.text = NewPatientData.Age.ToString();
        UIManager.Instance.Id.text = NewPatientData.Id.ToString();
        UIManager.Instance.PhoneNumber.text = NewPatientData.PhoneNumber.ToString();
    }

    [PunRPC]
    public void SetMeasurementsValuesRPC(string[] newMeasurements)
    {
        PatientData.SetMeasurementValues(newMeasurements);
    }

    [PunRPC]
    private void ChangeHeartRateRPC(int newBPM)
    {
        PatientData.HeartRateBPM = newBPM;
    }

    [PunRPC]
    private void SetMeasurementsRPC(string[] x) => NewPatientData.SetPatientMeasurement(x);

    [PunRPC]
    private void SetMeasurementByIndexRPC(int index, int value) // can do better without the new List
    {
        PatientData.Measurements = new List<int>() { PatientData.HeartRateBPM, PatientData.PainLevel, PatientData.RespiratoryRate, PatientData.CincinnatiLevel, PatientData.BloodSuger, PatientData.BloodPressure, PatientData.OxygenSaturation, PatientData.ETCO2 };
        PatientData.Measurements[index] = value;

        Measurements measurements = (Measurements)index;

        switch (measurements)
        {
            case Measurements.BPM:
                PatientData.HeartRateBPM = PatientData.Measurements[index];
                break;

            case Measurements.PainLevel:
                PatientData.PainLevel = PatientData.Measurements[index];
                break;

            case Measurements.RespiratoryRate:
                PatientData.RespiratoryRate = PatientData.Measurements[index];
                break;

            case Measurements.CincinnatiLevel:
                PatientData.CincinnatiLevel = PatientData.Measurements[index];
                break;

            case Measurements.BloodSugar:
                PatientData.BloodSuger = PatientData.Measurements[index];
                break;

            case Measurements.BloodPressure:
                PatientData.BloodPressure = PatientData.Measurements[index];
                break;

            case Measurements.OxygenSaturation:
                PatientData.OxygenSaturation = PatientData.Measurements[index];
                break;

            case Measurements.ETCO2:
                PatientData.ETCO2 = PatientData.Measurements[index];
                break;
        }
    }

    [PunRPC]
    private void ChangeClothingRPC(int index)
    {
        Clothing clothing = (Clothing)index;

        switch (clothing)
        {
            case Clothing.FullyClothed:
                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = FullyClothedMaterial;
                break;

            case Clothing.ShirtOnly:
                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = ShirtOnlyMaterial;
                break;

            case Clothing.PantsOnly:
                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = PantsOnlyMaterial;
                break;

            case Clothing.UnderwearOnly:
                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = UnderwearOnlyMaterial;
                break;

            default:
                break;
        }
    }

    [PunRPC]
    private void SetMonitorGraphRPC(int MonitorSpriteNum)
    {
        MonitorWindow.sprite = NewPatientData.MonitorSpriteList[MonitorSpriteNum];
    }

    [PunRPC]
    private void RemoveBandageFromUnusedListRPC(int BandageIndex) // need fixing, meshes are just fine
    {
        if (UseTourniquet)
        {
            if (BandageIndex == 0 || BandageIndex == 2) // Shin
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _tourniquetMeshList[0];
            }
            else if (BandageIndex == 1 || BandageIndex == 3) // Knee
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _tourniquetMeshList[1];

                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = ShirtOnlyMaterial;
            }
            else if (BandageIndex == 4 || BandageIndex == 6) // ForeArm
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _tourniquetMeshList[2];
            }
            else if (BandageIndex == 5 || BandageIndex == 7) // Bicep
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _tourniquetMeshList[3];

                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = PantsOnlyMaterial;
            }
        }
        else
        {
            if (BandageIndex == 0 || BandageIndex == 2) // Shin
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _bandageMeshList[0];
            }
            else if (BandageIndex == 1 || BandageIndex == 3) // Knee
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _bandageMeshList[1];

                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = ShirtOnlyMaterial;
            }
            else if (BandageIndex == 4 || BandageIndex == 6) // ForeArm
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _bandageMeshList[2];
            }
            else if (BandageIndex == 5 || BandageIndex == 7) // Bicep
            {
                _unusedBandagesOnPatient[BandageIndex].GetComponent<MeshFilter>().mesh = _bandageMeshList[3];

                transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = PantsOnlyMaterial;
            }
        }

        _unusedBandagesOnPatient[BandageIndex].SetActive(true);
        _unusedBandagesOnPatient.RemoveAt(BandageIndex);
    }

    [PunRPC]
    private void RevealPropOnPatientRPC(int propIndex)
    {
        switch (NewPatientData.PatientType)
        {
            case PatientType.Old:
                OldPropList[propIndex].SetActive(true);
                break;
            case PatientType.Grown:
                PropList[propIndex].SetActive(true);
                break;
            case PatientType.Kid:
                KidPropList[propIndex].SetActive(true);
                break;
            default:
                break;
        }
        PropList[propIndex].SetActive(true);
    }

    [PunRPC]
    private void UrgentEvactionRPC()
    {
        UrgentEvacuationCanvas.SetActive(true);
    }
    #endregion
}
