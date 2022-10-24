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
    [SerializeField] protected List<GameObject> _medicActionPermissions;
    [SerializeField] protected List<GameObject> _seniorMedicActionPermissions, _paramedicDocActionPermissions;

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
        _medicActionPermissions.Add(GameManager.Instance.CallAmbulanceActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.CallNatanActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.AspirinActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.EpipenAdultActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.BlueVenflonActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.GreenVenflonActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.OrangeVenflonActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.PinkVenflonActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.YellowVenflonActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.LoicoplastActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.Injector10ActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.Injector25ActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.Injector5ActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.Saline10ActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.Saline100ActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.Saline500ActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.SpongetaActionBtn);
        _medicActionPermissions.Add(GameManager.Instance.GauzePadActionBtn);
    }
    protected void InitializeSeniorMedicPermissions()
    {
        _seniorMedicActionPermissions.Add(GameManager.Instance.CallAmbulanceActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.CallNatanActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.AspirinActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.EpipenAdultActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.BlueVenflonActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.GreenVenflonActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.OrangeVenflonActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.PinkVenflonActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.YellowVenflonActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.LoicoplastActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.Injector10ActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.Injector25ActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.Injector5ActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.Saline10ActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.Saline100ActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.Saline500ActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.SpongetaActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.GauzePadActionBtn);

        _seniorMedicActionPermissions.Add(GameManager.Instance.InfusionKitActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.CPapActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.LmaActionBtn);
        _seniorMedicActionPermissions.Add(GameManager.Instance.DefibrilationAmbulanceActionBtn);
    }
    protected void InitializeParamedicDocPermissions()
    {
        _paramedicDocActionPermissions.Add(GameManager.Instance.CallAmbulanceActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.CallNatanActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.AspirinActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.EpipenAdultActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.BlueVenflonActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.GreenVenflonActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.OrangeVenflonActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.PinkVenflonActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.YellowVenflonActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.LoicoplastActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Injector10ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Injector25ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Injector5ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Saline10ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Saline100ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Saline500ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.SpongetaActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.GauzePadActionBtn);

        _paramedicDocActionPermissions.Add(GameManager.Instance.InfusionKitActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.CPapActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.LmaActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.DefibrilationNatanActionBtn);

        _paramedicDocActionPermissions.Add(GameManager.Instance.AdultQTKitActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.IntubationKitActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.ZondaKitActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.RespiratorActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.KateterBujiActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.PeepActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.BreslauScaleActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.BigKidsActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.InhalationMaskActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.OxygenIndicatorsActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.CapnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.NezaliFitterActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.CapnoTubusActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.CapnoNezaliActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.ConnectDefibrilationActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.SyncedFlipActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.PacingActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.ZofranActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Injector50ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.InjectorPOActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.KatumaNeedleActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.NezaliFitterActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.NeedleActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.NitrolingualActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.NarkanActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.SugmadexActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.SodiumTiosolfatActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.SolimedrolActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.SuperDropsActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.LopresorActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.MagneziumActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.OralTermometerActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.VentolinActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.TermadexActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Injector20ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.FusidActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.DriedPlazmaActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.PanetnileActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.ChanokitActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.KataminActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.CalciumActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.RukoroniumActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.D5wActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.AdenozinActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.AdrenalineActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.OptalginActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.AtomidatActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.AtropinActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.IzokatActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.IrobantActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.AcamolIVActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.BCarbonetActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.HighPressureStopcockActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.HlucogelActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Hlucose50ActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.DopaminActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.DormicomActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.DropridolActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.HaperinActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.HexacfronActionBtn);
        _paramedicDocActionPermissions.Add(GameManager.Instance.Hertman);
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
