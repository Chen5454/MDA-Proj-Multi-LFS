using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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

public class Action : MonoBehaviourPunCallbacks
{
    [Header("Player Data")]
    protected PhotonView LocalPlayerPhotonView;
    protected PlayerData LocalPlayerData;
    protected Color CrewColor;
    protected int LocalPlayerCrewIndex;
    protected string LocalPlayerName;

    [Header("Currently joined Patient's Data")]
    protected Patient CurrentPatient;
    //protected PatientData CurrentPatientData;
    protected NewPatientData CurrentPatientData;
    protected Transform PatientChestPosPlayerTransform;
    protected Transform PatientChestPosEquipmentTransform, PatientHeadPosPlayerTransform, PatientHeadPosEquipmentTransform, PatientLegPosPlayerTrasform;

    protected List<Transform> PlayerTreatingPositions;
    protected List<Transform> EquipmentPositions;

    [Header("Conditions")]
    [SerializeField] protected bool _shouldUpdateLog = true;

    [Header("Documentaion")]
    protected string TextToLog;

    private void AddTransformsToLists()
    {
        PlayerTreatingPositions = new List<Transform>();
        if (PatientHeadPosPlayerTransform)
            PlayerTreatingPositions.Add(PatientHeadPosPlayerTransform);
        if (PatientChestPosPlayerTransform)
            PlayerTreatingPositions.Add(PatientChestPosPlayerTransform);
        if (PatientLegPosPlayerTrasform)
            PlayerTreatingPositions.Add(PatientLegPosPlayerTrasform);

        EquipmentPositions = new List<Transform>();
        if (PatientChestPosEquipmentTransform)
            EquipmentPositions.Add(PatientChestPosEquipmentTransform);
        if (PatientHeadPosEquipmentTransform)
            EquipmentPositions.Add(PatientHeadPosEquipmentTransform);
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
                //CurrentPatientData = CurrentPatient.PatientData;
                CurrentPatientData = CurrentPatient.NewPatientData;

                if (CurrentPatient.ChestPosPlayerTransform)
                    PatientChestPosPlayerTransform = CurrentPatient.ChestPosPlayerTransform;
                if (CurrentPatient.ChestPosEquipmentTransform)
                    PatientChestPosEquipmentTransform = CurrentPatient.ChestPosEquipmentTransform;
                if (CurrentPatient.HeadPosPlayerTransform)
                    PatientHeadPosPlayerTransform = CurrentPatient.HeadPosPlayerTransform;
                if (CurrentPatient.HeadPosEquipmentTransform)
                    PatientHeadPosEquipmentTransform = CurrentPatient.HeadPosEquipmentTransform;
                if (CurrentPatient.LegPosPlayerTrasform)
                    PatientLegPosPlayerTrasform = CurrentPatient.LegPosPlayerTrasform;

                AddTransformsToLists();
                // if found local player no need for loop to continue
                break;
            }
        }
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        base.OnPlayerEnteredRoom(newPlayer);
        GetActionData();
      //  CurrentPatient.PhotonView.RPC("UpdatePatientAction", newPlayer);

    }

}
