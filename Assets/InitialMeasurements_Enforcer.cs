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
    Button nextBtn;



    bool canInteract;

    private void OnEnable()
    {
        CheckCondition();
    }
    public void CheckCondition()
    {
        canInteract = newPatientWindow.AreAllInitialMeasurementsFilled();
        enforcedButton.interactable = canInteract;
        nextBtn.interactable = canInteract;

        Color onColor;
        Color offColor;

        ColorUtility.TryParseHtmlString("#F41B49", out onColor);
        ColorUtility.TryParseHtmlString("#2A2A2A", out offColor);

        nextBtn.image.color = canInteract ? onColor : offColor;
    }
}
