using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ConnectingMonitor : Action
{
    [SerializeField] private bool _showAlert = false;
    [SerializeField] private bool _useGraph = false;

    [Header("Prefab References")]
    [SerializeField] private GameObject _monitor;

    [Header("Component's Data")]
    [SerializeField] private Image _newMonitorGraph;
    [SerializeField] private MonitorSprites _monitorSprites;

    [Header("Alert")]
    [SerializeField] private string _alertTitle;
    [SerializeField] private string _alertText;

    private GameObject _player;
    
    public void ConnectingDevice()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            _player = LocalPlayerData.gameObject;
            _player.transform.SetPositionAndRotation(CurrentPatient.ChestPosPlayerTransform.position, CurrentPatient.ChestPosPlayerTransform.rotation);

            GameObject monitor = PhotonNetwork.Instantiate(_monitor.name, CurrentPatient.ChestPosEquipmentTransform.position, CurrentPatient.ChestPosEquipmentTransform.rotation);

            if (_useGraph)
            {
                UIManager.Instance.MonitorParent.SetActive(true);

                if (!CurrentPatientData.MonitorSpriteList.Contains(_newMonitorGraph.sprite))
                {
                    int monitorSpritesNum = (int)_monitorSprites;
                    CurrentPatient.PhotonView.RPC("SetMonitorGraphRPC", RpcTarget.AllViaServer, monitorSpritesNum);
                }
            }

            TextToLog = $"Used {monitor.name} on Patient";

            if (_showAlert)
            {
                ShowTextAlert(_alertTitle, _alertText);
            }

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
