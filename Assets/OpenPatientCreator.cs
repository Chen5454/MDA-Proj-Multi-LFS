
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPatientCreator : MonoBehaviour
{
  //test

    [SerializeField]
    Button button;
    void Awake()
    {
        if(!button)
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);

    }

    void OnClick()
    {
        UIManager.Instance.PatientCreationWindow.SetActive(true);
    }

    private void OnDisable()
    {
        if(button)
        button.onClick.RemoveAllListeners();
    }

}
