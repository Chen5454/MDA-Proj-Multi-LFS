using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeClothing : Action
{
    [Header("Component's Data")]
    [SerializeField] private Clothing _clothing;
    [SerializeField] private string _textToLog;

    public void ChangeClothingAction()
    {
        GetActionData();

        TextToLog = $"Patient's clothing changed to: {_textToLog}";

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            CurrentPatient.PhotonView.RPC("ChangeClothingRPC", RpcTarget.AllBufferedViaServer, (int)_clothing);

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
