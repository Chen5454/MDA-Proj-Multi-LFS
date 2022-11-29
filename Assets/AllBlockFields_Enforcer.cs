using System.Collections;
using System.Collections.Generic;
using PatientCreationSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllBlockFields_Enforcer : MonoBehaviour, IPointerEnterHandler
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
        canInteract = newPatientWindow.AreAllTreatmentFieldsFilled();
        enforcedButton.interactable = canInteract;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        CheckCondition();
    }
}
