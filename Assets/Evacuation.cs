using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuation : MonoBehaviour
{
    public List<Patient> NearbyPatient;
    public EvacRoom RoomEnum;
    [SerializeField] private string _roomName;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Patient possiblePatient))
        {
            return;
        }
        else if (!NearbyPatient.Contains(possiblePatient))
        {
            NearbyPatient.Add(possiblePatient);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Patient possiblePatient))
        {
            if (!NearbyPatient.Contains(possiblePatient))
            {
                return;
            }
            else
            {
                NearbyPatient.Remove(possiblePatient);
            }
        }
    }
}
