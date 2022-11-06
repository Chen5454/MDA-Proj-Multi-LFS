using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField] private Patient _patient;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerData possiblePlayer))
        {
            return;
        }
        else if (!_patient.NearbyUsers.Contains(possiblePlayer))
        {
            _patient.WorldCanvas.SetActive(true);
            _patient.NearbyUsers.Add(possiblePlayer);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerData possiblePlayer))
        {
            if (!_patient.NearbyUsers.Contains(possiblePlayer))
            {
                return;
            }
            else
            {
                _patient.WorldCanvas.SetActive(false);
                _patient.NearbyUsers.Remove(possiblePlayer);
            }
        }
    }
}
