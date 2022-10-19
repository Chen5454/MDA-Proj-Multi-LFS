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
            if (!editPatientButton) //This is only relevant for the PatientCreationScene - irrelevant in Patient Selection
                return;

            PatientCreator.OnPatientClear  += DisableEditButton;
            PatientCreator.OnLoadPatient +=  EnableEditButton;
        }
        private void OnDisable()
        {
            if (!editPatientButton)//This is only relevant for the PatientCreationScene - irrelevant in Patient Selection
                return;

            PatientCreator.OnPatientClear -= DisableEditButton;
            PatientCreator.OnLoadPatient -= EnableEditButton;

        }
        /// <summary>
        /// Loads patient into the PatientCreator - to edit or use as base for a new patient
        /// </summary>
        /// <param name="patientFullName"></param>
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