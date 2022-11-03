using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanContinue : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Button button;
    [SerializeField]
    //UnityEngine.UI.Toggle IsALS;
    ToggleButton IsALS;
    [SerializeField]
    //UnityEngine.UI.Toggle IsALS;
    ToggleButton IsBLS;


    [SerializeField]
    //UnityEngine.UI.Toggle IsTrauma;
    ToggleButton IsTrauma;
    [SerializeField]
    //UnityEngine.UI.Toggle IsTrauma;
    ToggleButton IsIllness;
    private void OnEnable()
    {
        if (!button)
            button = GetComponent<UnityEngine.UI.Button>();

        IsALS.OnClick.AddListener(CheckMe);
        IsBLS.OnClick.AddListener(CheckMe);
        IsTrauma.OnClick.AddListener(CheckMe);
        IsIllness.OnClick.AddListener(CheckMe);
    }

    private void OnDisable()
    {
        IsALS.OnClick.RemoveListener(CheckMe);
        IsBLS.OnClick.RemoveListener(CheckMe);
        IsTrauma.OnClick.RemoveListener(CheckMe);
        IsIllness.OnClick.RemoveListener(CheckMe);
    }
    public void CheckMe() //called on valued changed for those toggles? - on click is enough
    {
        button.interactable = ((IsALS.IsBtnSelected || IsBLS.IsBtnSelected)&&(IsTrauma.IsBtnSelected || IsIllness.IsBtnSelected));
    }
}
