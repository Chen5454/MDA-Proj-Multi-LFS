using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    protected Transform PatientChestPosPlayerTransform;
    protected Transform PatientChestPosEquipmentTransform, PatientHeadPosPlayerTransform, PatientHeadPosEquipmentTransform, PatientLegPosPlayerTrasform;

    protected List<Transform> PlayerTreatingPositions;
    protected List<Transform> EquipmentPositions;

    [Header("Conditions")]
    [SerializeField] protected bool _shouldUpdateLog = true;

    [Header("Documentaion")]
    protected string TextToLog;

    private void Start()
    {
        PlayerTreatingPositions.Add(PatientHeadPosPlayerTransform);
        PlayerTreatingPositions.Add(PatientChestPosPlayerTransform);
        PlayerTreatingPositions.Add(PatientLegPosPlayerTrasform);

        EquipmentPositions.Add(PatientHeadPosEquipmentTransform);
        EquipmentPositions.Add(PatientChestPosEquipmentTransform);
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
