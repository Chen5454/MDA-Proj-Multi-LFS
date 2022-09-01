using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instantiate : Action
{
    [Header("Prefab References")]
    [SerializeField] private GameObject _item;

    [Header("Item Offsets")]
    [SerializeField] private Vector3 _offsetPos;
    [SerializeField] private Quaternion _offsetRot;

    [Header("Alerts")]
    [SerializeField] private string _alertTitle;
    [SerializeField] private string _alertText;

    [Header("Conditions")]
    [SerializeField] private bool _showAlert = false;
    [SerializeField] private bool _updateLog = true;
    [SerializeField] private bool _useColliders = false;

    private Transform[] _equipmentColliders;

    // 0 = HeadPos, 1 = ChestPos, 2 = Knees
    public void InstantiateOnPatient(int colliderIndex)
    {
        GetActionData();

        if (CurrentPatient.IsPlayerJoined(LocalPlayerData))
        {
            if (_useColliders)
            {
                _equipmentColliders[0] = PatientHeadPosEquipmentTransform;
                _equipmentColliders[1] = PatientChestPosEquipmentTransform;
                //_equipmentColliders[2] = currentPatient.KneesPosEquipmentTransform;

                GameObject item = PhotonNetwork.Instantiate(_item.name, _equipmentColliders[colliderIndex].position, _equipmentColliders[colliderIndex].rotation);

                item.transform.SetParent(_equipmentColliders[colliderIndex]);
            }
            else
            {
                Vector3 desiredPosition = CurrentPatient.transform.position + _offsetPos;
                Quaternion desiredRotation = new Quaternion(CurrentPatient.transform.rotation.x + _offsetRot.y, CurrentPatient.transform.rotation.y + _offsetRot.x, CurrentPatient.transform.rotation.z + _offsetRot.z, CurrentPatient.transform.rotation.w + _offsetRot.w);

                GameObject item = PhotonNetwork.Instantiate(_item.name, desiredPosition, desiredRotation);
                item.transform.SetParent(CurrentPatient.transform);
            }

            TextToLog = $"Placed {_item.name} on {CurrentPatientData.Name} {CurrentPatientData.SureName}";

            if (_showAlert)
            {
                ShowTextAlert(_alertTitle, _alertText);
            }

            if (_updateLog)
            {
                LogText(TextToLog);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, _offsetPos);
    }
}
