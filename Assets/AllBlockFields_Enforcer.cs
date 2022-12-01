using System.Collections;
using System.Collections.Generic;
using PatientCreationSpace;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllBlockFields_Enforcer : MonoBehaviour/*, IPointerEnterHandler*/
{
    [SerializeField]
    float checkConditionPerSecond = 1;
    [SerializeField]
    NewPatientWindow newPatientWindow;
    [SerializeField]
    Button enforcedButton;

    


    bool doCheck = true;
    bool canInteract;

    private void OnEnable()
    {
        CheckCondition();
        StartCoroutine(nameof(CheckMeCoroutine));

    }
    public void CheckCondition()
    {
        canInteract = newPatientWindow.AreAllTreatmentFieldsFilled();
        enforcedButton.interactable = canInteract;
    }

    IEnumerator CheckMeCoroutine()
    {
        while(doCheck)
        {
            CheckCondition();
            yield return new WaitForSeconds(1f/checkConditionPerSecond);
        }
    }
    private void OnDisable()
    {
        StopCoroutine(nameof(CheckMeCoroutine));
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    CheckCondition();
    //}
}
