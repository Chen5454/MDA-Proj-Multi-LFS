using UnityEngine;
using Photon.Pun;

public class ActionPlaceHolder : Action
{
    [Header("Component's Data")]
    [SerializeField] private string _someVariable;
    [SerializeField] private string _alertTitle, _alertContent;

    [Header("Alerts")]
    [SerializeField] private bool _showAlert = false;
    [SerializeField] private bool _updateLog = true;

    public void DoAction()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            // CurrentPatient.PhotonView.RPC("SomeMethodRPC", RpcTarget.AllBufferedViaServer, _someVariable);

            TextToLog = $"Patient's {_alertTitle} is: {_alertContent}";

            if (_showAlert)
            {
                ShowTextAlert(_alertTitle, _alertContent);
            }

            if (_updateLog)
            {
                LogText(TextToLog);
            }
        }
    }
}
