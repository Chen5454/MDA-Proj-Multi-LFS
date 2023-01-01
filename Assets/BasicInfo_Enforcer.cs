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
    [SerializeField]
    Button nextBtn;

    private void OnEnable()
    {
        CheckCondition();
        PatientCreator.OnLoadPatient += CheckCondition;
    }

    private void OnDisable()
    {
        PatientCreator.OnLoadPatient -= CheckCondition;
    }

    bool canInteract;
    public void CheckCondition()
    {
        canInteract = newPatientWindow.AreAllBasicInfoFieldFilled();
        enforcedButton.interactable = canInteract;
        nextBtn.interactable = canInteract;

        Color onColor;
        Color offColor;

        ColorUtility.TryParseHtmlString("#F41B49", out onColor);
        ColorUtility.TryParseHtmlString("#2A2A2A", out offColor);

        nextBtn.image.color = canInteract ? onColor : offColor;
    }

    IEnumerator CheckMeCoroutine()
    {
        while (true)
        {
            CheckCondition();
            yield return new WaitForSeconds(1f / 1);
        }
    }
    private void OnDisable()
    {
        StopCoroutine(nameof(CheckMeCoroutine));
    }
}
