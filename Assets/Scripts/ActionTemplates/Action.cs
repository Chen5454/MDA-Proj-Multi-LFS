using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
 * for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
   {
       if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
       {
           
       }
   }
 *
 */

public enum PlayerTreatingPosition { Head = 0, Chest = 1, Leg = 2}
public enum EquipmentPosition { Head = 0, Chest = 1}

public class Action : MonoBehaviour
{
    [Header("Player Data")]
    protected PhotonView LocalPlayerPhotonView;
    protected PlayerData LocalPlayerData;
    protected Color CrewColor;
    protected int LocalPlayerCrewIndex;
    protected string LocalPlayerName;

    [Header("Currently joined Patient's Data")]
    protected Patient CurrentPatient;
    protected PatientData CurrentPatientData;
    protected NewPatientData CurrentNewPatientData;
    protected Transform PatientChestPosPlayerTransform;
    protected Transform PatientChestPosEquipmentTransform, PatientHeadPosPlayerTransform, PatientHeadPosEquipmentTransform, PatientLegPosPlayerTrasform;

    protected List<Transform> PlayerTreatingPositions;
    protected List<Transform> EquipmentPositions;

    [Header("Conditions")]
    [SerializeField] protected bool _shouldUpdateLog = true;

    [Header("Documentaion")]
    protected string TextToLog;

    [Header("Permissions")]
    [SerializeField] protected GameObject _callAmbulanceActionBtn;
    [SerializeField] protected GameObject _callNatanActionBtn, _aspirinActionBtn, _epipenAdultActionBtn, _infusionKitActionBtn, _cPapActionBtn, _lmaActionBtn, _QTKitActionBtn, _adultQTKitActionBtn, _intubationKitActionBtn, _zondaKitActionBtn, _respiratorActionBtn, _kateterBujiActionBtn, _peepActionBtn, _breslauScaleActionBtn, _bigKidsActionBtn, _inhalationMaskActionBtn, _oxygenIndicatorsActionBtn, _capnoTubusActionBtn, _capnoNezaliActionBtn, _connectDefibrilationActionBtn, _defibrilationAmbulanceActionBtn, _defibrilationNatanActionBtn,  _syncedFlipActionBtn, _pacingActionBtn, _pinkVenflonActionBtn, _greenVenflonActionBtn, _blueVenflonActionBtn, _orangeVenflonActionBtn, _yellowVenflonActionBtn, _veinBlockerActionBtn, _loicoplastActionBtn, _injector10ActionBtn, _injector25ActionBtn, _injector5ActionBtn, _saline10ActionBtn, _saline100ActionBtn, _saline500ActionBtn, _spongetaActionBtn, _gauzePadActionBtn, _zofranActionBtn, _injector50ActionBtn, _injectorPOActionBtn, _katumaNeedleActionBtn, _nezaliFitterActionBtn, _needleActionBtn, _nitrolingualActionBtn, _narkanActionBtn, _sugmadexActionBtn, _sodiumTiosolfatActionBtn, _solimedrolActionBtn, _superDropsActionBtn, _lopresorActionBtn, _magneziumActionBtn, _oralTermometerActionBtn, _ventolinActionBtn, _termadexActionBtn, _injector20ActionBtn, _fusidActionBtn, _driedPlazmaActionBtn, _panetnileActionBtn, _chanokitActionBtn, _kataminActionBtn, _calciumActionBtn, _rukoroniumActionBtn, _d5wActionBtn, _adenozinActionBtn, _adrenalineActionBtn, _optalginActionBtn, _atomidatActionBtn, _atropinActionBtn, _izokatActionBtn, _irobantActionBtn, _acamolIVActionBtn, _bCarbonetActionBtn, _highPressureStopcockActionBtn, _glucogelActionBtn, _glucose50ActionBtn, _dopaminActionBtn, _dormicomActionBtn, _dropridolActionBtn, _haperinActionBtn, _hexacfronActionBtn, _hertman;

    [SerializeField] protected List<GameObject> _medicActionPermissions, _seniorMedicActionPermissions, _paramedicDocActionPermissions;

    [SerializeField] protected bool _useMedicPermissions, _useSeniorMedicPermissions, _useParamedicDocPermissions;

    private void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (PatientHeadPosPlayerTransform)
            PlayerTreatingPositions.Add(PatientHeadPosPlayerTransform);
        if (PatientChestPosPlayerTransform)
            PlayerTreatingPositions.Add(PatientChestPosPlayerTransform);
        if (PatientLegPosPlayerTrasform)
            PlayerTreatingPositions.Add(PatientLegPosPlayerTrasform);

        if (PatientHeadPosEquipmentTransform)
            EquipmentPositions.Add(PatientHeadPosEquipmentTransform);
        if (PatientChestPosEquipmentTransform)
            EquipmentPositions.Add(PatientChestPosEquipmentTransform);


        if (LocalPlayerData.UserRole == Roles.Medic)
            InitializeMedicPermissions();
        else if (LocalPlayerData.UserRole == Roles.SeniorMedic)
            InitializeSeniorMedicPermissions();
        else if (LocalPlayerData.UserRole == Roles.Paramedic || LocalPlayerData.UserRole == Roles.Doctor)
            InitializeParamedicDocPermissions();
    }

    protected void InitializeMedicPermissions()
    {
        _medicActionPermissions.Add(_callAmbulanceActionBtn);
        _medicActionPermissions.Add(_callNatanActionBtn);
        _medicActionPermissions.Add(_aspirinActionBtn);
        _medicActionPermissions.Add(_epipenAdultActionBtn);
        _medicActionPermissions.Add(_blueVenflonActionBtn);
        _medicActionPermissions.Add(_greenVenflonActionBtn);
        _medicActionPermissions.Add(_orangeVenflonActionBtn);
        _medicActionPermissions.Add(_pinkVenflonActionBtn);
        _medicActionPermissions.Add(_yellowVenflonActionBtn);
        _medicActionPermissions.Add(_loicoplastActionBtn);
        _medicActionPermissions.Add(_injector10ActionBtn);
        _medicActionPermissions.Add(_injector25ActionBtn);
        _medicActionPermissions.Add(_injector5ActionBtn);
        _medicActionPermissions.Add(_saline10ActionBtn);
        _medicActionPermissions.Add(_saline100ActionBtn);
        _medicActionPermissions.Add(_saline500ActionBtn);
        _medicActionPermissions.Add(_spongetaActionBtn);
        _medicActionPermissions.Add(_gauzePadActionBtn);
    }

    protected void InitializeSeniorMedicPermissions()
    {
        _seniorMedicActionPermissions.Add(_callAmbulanceActionBtn);
        _seniorMedicActionPermissions.Add(_callNatanActionBtn);
        _seniorMedicActionPermissions.Add(_aspirinActionBtn);
        _seniorMedicActionPermissions.Add(_epipenAdultActionBtn);
        _seniorMedicActionPermissions.Add(_blueVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_greenVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_orangeVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_pinkVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_yellowVenflonActionBtn);
        _seniorMedicActionPermissions.Add(_loicoplastActionBtn);
        _seniorMedicActionPermissions.Add(_injector10ActionBtn);
        _seniorMedicActionPermissions.Add(_injector25ActionBtn);
        _seniorMedicActionPermissions.Add(_injector5ActionBtn);
        _seniorMedicActionPermissions.Add(_saline10ActionBtn);
        _seniorMedicActionPermissions.Add(_saline100ActionBtn);
        _seniorMedicActionPermissions.Add(_saline500ActionBtn);
        _seniorMedicActionPermissions.Add(_spongetaActionBtn);
        _seniorMedicActionPermissions.Add(_gauzePadActionBtn);

        _seniorMedicActionPermissions.Add(_infusionKitActionBtn);
        _seniorMedicActionPermissions.Add(_cPapActionBtn);
        _seniorMedicActionPermissions.Add(_lmaActionBtn);
        _seniorMedicActionPermissions.Add(_defibrilationAmbulanceActionBtn);
    }

    protected void InitializeParamedicDocPermissions()
    {
        _paramedicDocActionPermissions.Add(_callAmbulanceActionBtn);
        _paramedicDocActionPermissions.Add(_callNatanActionBtn);
        _paramedicDocActionPermissions.Add(_aspirinActionBtn);
        _paramedicDocActionPermissions.Add(_epipenAdultActionBtn);
        _paramedicDocActionPermissions.Add(_blueVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_greenVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_orangeVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_pinkVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_yellowVenflonActionBtn);
        _paramedicDocActionPermissions.Add(_loicoplastActionBtn);
        _paramedicDocActionPermissions.Add(_injector10ActionBtn);
        _paramedicDocActionPermissions.Add(_injector25ActionBtn);
        _paramedicDocActionPermissions.Add(_injector5ActionBtn);
        _paramedicDocActionPermissions.Add(_saline10ActionBtn);
        _paramedicDocActionPermissions.Add(_saline100ActionBtn);
        _paramedicDocActionPermissions.Add(_saline500ActionBtn);
        _paramedicDocActionPermissions.Add(_spongetaActionBtn);
        _paramedicDocActionPermissions.Add(_gauzePadActionBtn);

        _paramedicDocActionPermissions.Add(_infusionKitActionBtn);
        _paramedicDocActionPermissions.Add(_cPapActionBtn);
        _paramedicDocActionPermissions.Add(_lmaActionBtn);
        _paramedicDocActionPermissions.Add(_defibrilationNatanActionBtn);

        _paramedicDocActionPermissions.Add(_adultQTKitActionBtn);
        _paramedicDocActionPermissions.Add(_intubationKitActionBtn);
        _paramedicDocActionPermissions.Add(_zondaKitActionBtn);
        _paramedicDocActionPermissions.Add(_respiratorActionBtn);
        _paramedicDocActionPermissions.Add(_kateterBujiActionBtn);
        _paramedicDocActionPermissions.Add(_peepActionBtn);
        _paramedicDocActionPermissions.Add(_breslauScaleActionBtn);
        _paramedicDocActionPermissions.Add(_bigKidsActionBtn);
        _paramedicDocActionPermissions.Add(_inhalationMaskActionBtn);
        _paramedicDocActionPermissions.Add(_oxygenIndicatorsActionBtn);
        _paramedicDocActionPermissions.Add(_capnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(_nezaliFitterActionBtn);
        _paramedicDocActionPermissions.Add(_capnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(_capnoNezaliActionBtn);
        _paramedicDocActionPermissions.Add(_connectDefibrilationActionBtn);
        _paramedicDocActionPermissions.Add(_syncedFlipActionBtn);
        _paramedicDocActionPermissions.Add(_pacingActionBtn);
        _paramedicDocActionPermissions.Add(_zofranActionBtn);
        _paramedicDocActionPermissions.Add(_injector50ActionBtn);
        _paramedicDocActionPermissions.Add(_injectorPOActionBtn);
        _paramedicDocActionPermissions.Add(_katumaNeedleActionBtn);
        _paramedicDocActionPermissions.Add(_nezaliFitterActionBtn);
        _paramedicDocActionPermissions.Add(_needleActionBtn);
        _paramedicDocActionPermissions.Add(_nitrolingualActionBtn);
        _paramedicDocActionPermissions.Add(_narkanActionBtn);
        _paramedicDocActionPermissions.Add(_sugmadexActionBtn);
        _paramedicDocActionPermissions.Add(_sodiumTiosolfatActionBtn);
        _paramedicDocActionPermissions.Add(_solimedrolActionBtn);
        _paramedicDocActionPermissions.Add(_superDropsActionBtn);
        _paramedicDocActionPermissions.Add(_lopresorActionBtn);
        _paramedicDocActionPermissions.Add(_magneziumActionBtn);
        _paramedicDocActionPermissions.Add(_oralTermometerActionBtn);
        _paramedicDocActionPermissions.Add(_ventolinActionBtn);
        _paramedicDocActionPermissions.Add(_termadexActionBtn);
        _paramedicDocActionPermissions.Add(_injector20ActionBtn);
        _paramedicDocActionPermissions.Add(_fusidActionBtn);
        _paramedicDocActionPermissions.Add(_driedPlazmaActionBtn);
        _paramedicDocActionPermissions.Add(_panetnileActionBtn);
        _paramedicDocActionPermissions.Add(_chanokitActionBtn);
        _paramedicDocActionPermissions.Add(_kataminActionBtn);
        _paramedicDocActionPermissions.Add(_calciumActionBtn);
        _paramedicDocActionPermissions.Add(_rukoroniumActionBtn);
        _paramedicDocActionPermissions.Add(_d5wActionBtn);
        _paramedicDocActionPermissions.Add(_adenozinActionBtn);
        _paramedicDocActionPermissions.Add(_adrenalineActionBtn);
        _paramedicDocActionPermissions.Add(_optalginActionBtn);
        _paramedicDocActionPermissions.Add(_atomidatActionBtn);
        _paramedicDocActionPermissions.Add(_atropinActionBtn);
        _paramedicDocActionPermissions.Add(_izokatActionBtn);
        _paramedicDocActionPermissions.Add(_irobantActionBtn);
        _paramedicDocActionPermissions.Add(_acamolIVActionBtn);
        _paramedicDocActionPermissions.Add(_bCarbonetActionBtn);
        _paramedicDocActionPermissions.Add(_highPressureStopcockActionBtn);
        _paramedicDocActionPermissions.Add(_glucogelActionBtn);
        _paramedicDocActionPermissions.Add(_glucose50ActionBtn);
        _paramedicDocActionPermissions.Add(_dopaminActionBtn);
        _paramedicDocActionPermissions.Add(_dormicomActionBtn);
        _paramedicDocActionPermissions.Add(_dropridolActionBtn);
        _paramedicDocActionPermissions.Add(_haperinActionBtn);
        _paramedicDocActionPermissions.Add(_hexacfronActionBtn);
        _paramedicDocActionPermissions.Add(_hertman);
    }

    public void GetActionData()
    {
        // loops through all players photonViews
        for (int i = 0; i < ActionsManager.Instance.AllPlayersPhotonViews.Count; i++)
        {
            if (ActionsManager.Instance.AllPlayersPhotonViews[i].IsMine)
            {
                PhotonView photonView = ActionsManager.Instance.AllPlayersPhotonViews[i];
                // Get local photonView
                LocalPlayerPhotonView = photonView;

                // Get local PlayerData
                LocalPlayerData = photonView.GetComponent<PlayerData>();
                LocalPlayerName = LocalPlayerData.UserName;
                LocalPlayerCrewIndex = LocalPlayerData.CrewIndex;
                CrewColor = LocalPlayerData.CrewColor;

                // check if local player joined with a Patient
                if (!LocalPlayerData.CurrentPatientNearby.IsPlayerJoined(LocalPlayerData))
                    return;

                // get Patient & PatientData
                CurrentPatient = LocalPlayerData.CurrentPatientNearby;
                CurrentPatientData = CurrentPatient.PatientData;

                PatientChestPosPlayerTransform = CurrentPatient.ChestPosPlayerTransform;
                PatientChestPosEquipmentTransform = CurrentPatient.ChestPosEquipmentTransform;
                PatientHeadPosPlayerTransform = CurrentPatient.HeadPosPlayerTransform;
                PatientHeadPosEquipmentTransform = CurrentPatient.HeadPosEquipmentTransform;
                PatientLegPosPlayerTrasform = CurrentPatient.LegPosPlayerTrasform;

                // if found local player no need for loop to continue
                break;
            }
        }
        //// loops through all players photonViews
        //foreach (PhotonView photonView in ActionsManager.Instance.AllPlayersPhotonViews)
        //{
        //    // execute only if this instance if of the local player
        //    if (photonView.IsMine)
        //    {
        //        // Get local photonView
        //        LocalPlayerPhotonView = photonView;
        //
        //        // Get local PlayerData
        //        LocalPlayerData = photonView.GetComponent<PlayerData>();
        //        LocalPlayerName = LocalPlayerData.UserName;
        //        LocalPlayerCrewIndex = LocalPlayerData.CrewIndex;
        //        CrewColor = LocalPlayerData.CrewColor;
        //
        //        // check if local player joined with a Patient
        //        if (!LocalPlayerData.CurrentPatientNearby.IsPlayerJoined(LocalPlayerData))
        //            return;
        //
        //        // get Patient & PatientData
        //        CurrentPatient = LocalPlayerData.CurrentPatientNearby;
        //        CurrentPatientData = CurrentPatient.PatientData;
        //
        //        PatientChestPosPlayerTransform = CurrentPatient.ChestPosPlayerTransform;
        //        PatientChestPosEquipmentTransform = CurrentPatient.ChestPosEquipmentTransform;
        //        PatientHeadPosPlayerTransform = CurrentPatient.HeadPosPlayerTransform;
        //        PatientHeadPosEquipmentTransform = CurrentPatient.HeadPosEquipmentTransform;
        //        PatientLegPosPlayerTrasform = CurrentPatient.LegPosPlayerTrasform;
        //
        //        // if found local player no need for loop to continue
        //        break;
        //    }
        //}
    }

    public void ShowTextAlert(string title, string content)
    {
        ActionTemplates.Instance.ShowAlertWindow(title, content);
    }

    public void ShowNumAlert(string title, int number)
    {
        ActionTemplates.Instance.ShowAlertWindow(title, number);
    }

    public void LogText(string textToLog)
    {
        ActionTemplates.Instance.UpdatePatientLog(LocalPlayerCrewIndex, LocalPlayerName, textToLog);
    }
}
