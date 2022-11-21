
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPatientCreator : MonoBehaviour
{
  

    [SerializeField]
    Button button;
    void Start()
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
        button.onClick.RemoveAllListeners();
    }

}
