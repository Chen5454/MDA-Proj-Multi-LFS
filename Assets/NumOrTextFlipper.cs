using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NumOrTextFlipper : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_InputField inputField;
    private void OnEnable()
    {

        if (!inputField)
            inputField = GetComponent<TMPro.TMP_InputField>();

        inputField.onValueChanged.RemoveAllListeners();

        inputField.onValueChanged.AddListener(delegate { CheckIfRTL(); });
    }
    private void OnDisable()
    {
        inputField.onValueChanged.RemoveAllListeners();
    }
    public void CheckIfRTL()
    {
        
        if (string.IsNullOrEmpty(inputField.text))
            return;
        if (int.TryParse(inputField.text, out int a) || float.TryParse(inputField.text, out float b))
        {
            //IS NUMBER!
            inputField.textComponent.isRightToLeftText = false;
        }
        else
        {
            //IS TEXT!
            inputField.textComponent.isRightToLeftText = true;
        }

    }
}
