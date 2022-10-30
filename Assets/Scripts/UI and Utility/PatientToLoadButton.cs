using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
namespace PatientCreationSpace
{

    public class PatientToLoadButton : MonoBehaviour
    {

        [SerializeField]
        TMP_Text text;
        //[SerializeField]
        //Button button;
        PatientRoster patientRoster;//SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        FilteredPatientsRoster filteredPatientRoster; //SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        string fileNameToLoad;

        public void Set(string patientName, PatientRoster pr)
        {
            patientRoster = pr;
            text.text = patientName;
            fileNameToLoad = patientName;
        }
        public void Set(string patientName, FilteredPatientsRoster fpr)
        {
            filteredPatientRoster = fpr;
            text.text = patientName;
            fileNameToLoad = patientName;
        }



        public void LoadThisPatient() // called in inspector by button //SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        {
            if (patientRoster)
                patientRoster.LoadPatient(fileNameToLoad); 
            else if (filteredPatientRoster)
                filteredPatientRoster.LoadPatient(fileNameToLoad);
        }
    }

}