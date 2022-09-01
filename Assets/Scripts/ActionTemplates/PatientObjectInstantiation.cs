using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PatientObjectInstantiation : Action
{
    [Header("Prefab References")]
    [SerializeField] private GameObject _item;

    [Header("Item Name")]
    [SerializeField] private string _itemName;

    [Header("Item Offsets")]
    [SerializeField] private Vector3 _offsetPos;
    [SerializeField] private Vector3 _offsetRot;

    public void InstantiateOnPatient()
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            Vector3 desiredPosition = CurrentPatient.transform.position + _offsetPos;
            Quaternion desiredRotation = new Quaternion(_offsetRot.x, _offsetRot.y, _offsetRot.z, Quaternion.identity.w);

            GameObject item = PhotonNetwork.Instantiate(_item.name, desiredPosition, desiredRotation);
            item.transform.SetParent(CurrentPatient.transform);

            TextToLog = $"Used {_itemName}";

            if (_shouldUpdateLog)
            {
                LogText(TextToLog);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_offsetPos, _item.transform.localScale);
    }
}
