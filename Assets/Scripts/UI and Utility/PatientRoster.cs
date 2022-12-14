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

        List<PatientToLoadButton> patientButtons;

        public void SetUpNamesAsButtons()
        {
            if (patientButtons == null)
                patientButtons = new List<PatientToLoadButton>();

            //names = PatientCreator.GetExistingPatientNames();
            names = RequestTest.Instance.GetAllFirstsInRow();


            if(names == null || names.Count ==0)
            {
                Debug.Log("No files to load");
                return;
            }

            if(patientButtons.Count > names.Count)
            {
                int delta = patientButtons.Count - names.Count;
                for (int i = 0; i < delta ; i++)
                {
                    Destroy(patientButtons[patientButtons.Count - 1]);
                    patientButtons.Remove(patientButtons[patientButtons.Count - 1]);
                }
            }
            else if(patientButtons.Count < names.Count)
            {
                int delta = names.Count- patientButtons.Count;
                for (int i = 0; i < delta; i++)
                {
                    GameObject g = Instantiate(patientButtonPrefab, verticalGroup);

                    patientButtons.Add(g.GetComponent<PatientToLoadButton>());
                }

            }

            for (int i = 0; i < names.Count; i++)
            {
                patientButtons[i].Set(names[i], this);
            }

           
        }
        private void OnEnable()
        {
            SetUpNamesAsButtons();
          

            PatientCreator.OnPatientClear  += DisableEditButton;
            PatientCreator.OnLoadPatient +=  EnableEditButton;
        }
        private void OnDisable()
        {
           

            PatientCreator.OnPatientClear -= DisableEditButton;
            PatientCreator.OnLoadPatient -= EnableEditButton;

        }
        /// <summary>
        /// Loads patient into the PatientCreator - to edit or use as base for a new patient
        /// </summary>
        /// <param name="patientFullName"></param>
        public NewPatientData LoadPatient(string patientFullName)
        {
            if (string.IsNullOrEmpty(patientFullName))
            {
                Debug.LogError("no file name");
                return null;
            }
            return PatientCreator.LoadPatient(patientFullName); 
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