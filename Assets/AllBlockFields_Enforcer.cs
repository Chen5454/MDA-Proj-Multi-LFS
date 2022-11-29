using System.Collections;
using System.Collections.Generic;
using PatientCreationSpace;
using UnityEngine;
using UnityEngine.UI;

public class AllBlockFields_Enforcer : MonoBehaviour
{
    [SerializeField]
    NewPatientWindow newPatientWindow;
    [SerializeField]
    Button enforcedButton;



    bool canInteract;

    private void OnEnable()
    {
        CheckCondition();
    }
    public void CheckCondition()
    {
        canInteract = newPatientWindow.AreAllInitialMeasurementsFilled();
        enforcedButton.interactable = canInteract;
    }
}
