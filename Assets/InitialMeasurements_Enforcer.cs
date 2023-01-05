using System.Collections;
using System.Collections.Generic;
using PatientCreationSpace;
using UnityEngine;
using UnityEngine.UI;

public class InitialMeasurements_Enforcer : MonoBehaviour
{
    [SerializeField]
    NewPatientWindow newPatientWindow;
    [SerializeField]
    Button enforcedButton;
    [SerializeField]
    Button nextCreateBtn, nextEditBtn;



    bool canInteract;

    private void OnEnable()
    {
        CheckCondition();
        PatientCreator.OnLoadPatient += CheckCondition;
    }

    private void OnDisable()
    {
        PatientCreator.OnLoadPatient -= CheckCondition;
    }
    public void CheckCondition()
    {
        canInteract = newPatientWindow.AreAllInitialMeasurementsFilled();
        enforcedButton.interactable = canInteract;

        if (nextCreateBtn.gameObject.activeInHierarchy)
            nextCreateBtn.interactable = canInteract;
        else if (nextEditBtn.gameObject.activeInHierarchy)
            nextEditBtn.interactable = canInteract;

        Color onColor;
        Color offColor;

        ColorUtility.TryParseHtmlString("#F41B49", out onColor);
        ColorUtility.TryParseHtmlString("#2A2A2A", out offColor);

        nextCreateBtn.image.color = canInteract ? onColor : offColor;
        nextEditBtn.image.color = canInteract ? onColor : offColor;
    }
}
