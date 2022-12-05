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
        [SerializeField] private Button _btn;
        [SerializeField] private CrewRoomManager crewRoom;

        [SerializeField]
        TMP_Text text;
        //[SerializeField]
        //Button button;
        PatientRoster patientRoster;//SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        FilteredPatientsRoster filteredPatientRoster; //SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        NoPUN_FilteredPatientRoster noPUN_filteredPatientRoster; //SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        string fileNameToLoad;

        private void OnEnable()
        {
            if (!_btn)
            {
                _btn = GetComponent<Button>();
            }

            if (!crewRoom)
            {
                crewRoom = GetComponentInParent<CrewRoomManager>();
            }
        }
        private void OnDestroy()
        {
            _btn.onClick.RemoveListener(delegate { filteredPatientRoster.CrewRoomManager.SetStartIncidentBtn(); });
        }

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
            _btn.onClick.AddListener(delegate { filteredPatientRoster.CrewRoomManager.SetStartIncidentBtn(); });
        }
        public void Set(string patientName, NoPUN_FilteredPatientRoster np_fpr)
        {
            noPUN_filteredPatientRoster = np_fpr;
            text.text = patientName;
            fileNameToLoad = patientName;
        }



        public void LoadThisPatient() // called in inspector by button //SHOULD HAVE MADE THESE CLASSES with inheritence, alas - no time
        {
            if (patientRoster)
                patientRoster.LoadPatient(fileNameToLoad);
            else if (filteredPatientRoster)
                filteredPatientRoster.LoadPatient(fileNameToLoad);
            else if (noPUN_filteredPatientRoster)
                noPUN_filteredPatientRoster.LoadPatient(fileNameToLoad);


            crewRoom.RemoveOverlayUI();
        }
    }

}