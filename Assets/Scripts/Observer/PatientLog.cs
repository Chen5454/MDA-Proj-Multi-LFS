using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientLog : MonoBehaviour
{
    private void Start()
    {
        PointOfInterest.OnPointOfInterest += OnPointOfInterestTriggered;
    }

    private void OnPointOfInterestTriggered(PointOfInterest pointOfInterest)
    {
        // logic here
    }

    private void OnDestroy()
    {
        PointOfInterest.OnPointOfInterest -= OnPointOfInterestTriggered;
    }
}
