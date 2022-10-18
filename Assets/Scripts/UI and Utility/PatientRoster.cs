using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PatientCreationSpace
{

    public class PatientRoster : MonoBehaviour
    {
        [SerializeField]
        GameObject patientButtonPrefab;
        [SerializeField]
        Transform verticalGroup;

        [SerializeField]
        Button editPatientButton;
        List<string> names;

        void Start()
        {
            names = PatientCreator.GetExistingPatientNames();
            if(names == null || names.Count ==0)
            {
                Debug.Log("No files to load");
                return;
            }

            foreach (var item in names)
            {
                GameObject g = Instantiate(patientButtonPrefab, verticalGroup);
                g.GetComponent<PatientToLoadButton>().Set(item, this);
            }
        }
        private void OnEnable()
        {
            PatientCreator.OnPatientClear  += DisableEditButton;
            PatientCreator.OnLoadPatient +=  EnableEditButton;
        }
        private void OnDisable()
        {
            PatientCreator.OnPatientClear -= DisableEditButton;
            PatientCreator.OnLoadPatient -= EnableEditButton;

        }

        public void LoadPatient(string patientFullName)
        {
            if (string.IsNullOrEmpty(patientFullName))
            {
                Debug.LogError("no file name");
                return;
            }
            PatientCreator.LoadPatient(patientFullName); 
        }

        void EnableEditButton()
        {
            editPatientButton.interactable = true;
        }
        void DisableEditButton()
        {
            editPatientButton.interactable = false;
        }
    }

}