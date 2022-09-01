using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingBedCollider : MonoBehaviour
{

    [SerializeField] public GameObject BedRefrence;

    private void OnTriggerEnter(Collider other)
    {
     
        if (other.CompareTag("EmergencyBed"))
        {
            Debug.Log("Bed Detected");
            BedRefrence = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EmergencyBed"))
        {

            BedRefrence = null;
        }
    }
}
