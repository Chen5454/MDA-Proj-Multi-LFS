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
        PatientRoster patientRoster;
        string fileNameToLoad;

        public void Set(string patientName, PatientRoster pr)
        {
            patientRoster = pr;
            text.text = patientName;
            fileNameToLoad = patientName;
        }



        public void LoadThisPatient() // called in inspector by button
        {
            patientRoster.LoadPatient(fileNameToLoad);
        }
    }

}