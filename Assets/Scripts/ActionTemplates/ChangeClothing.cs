using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChangeClothing : Action
{
    [Header("Component's Data")]
    [SerializeField] private Clothing _clothing;
    [SerializeField] private string _textToLog;

    public void ChangeClothingAction()
    {
        GetActionData();

        TextToLog = $"{_textToLog}";

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            CurrentPatient.PhotonView.RPC("ChangeClothingRPC", RpcTarget.AllViaServer, (int)_clothing);

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }


    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
            CurrentPatient.PhotonView.RPC("ChangeClothingRPC", newPlayer, (int)_clothing);


    }
}
