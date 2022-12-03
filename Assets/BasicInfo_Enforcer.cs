using System.Collections;
using System.Collections.Generic;
using PatientCreationSpace;
using UnityEngine;
using UnityEngine.UI;

public class BasicInfo_Enforcer : MonoBehaviour
{
    [SerializeField]
    NewPatientWindow newPatientWindow;
    [SerializeField]
    Button enforcedButton;

    private void OnEnable()
    {
        CheckCondition();
    }

    bool canInteract;
    public void CheckCondition()
    {
        canInteract = newPatientWindow.AreAllBasicInfoFieldFilled();
        enforcedButton.interactable = canInteract;
    }
}
