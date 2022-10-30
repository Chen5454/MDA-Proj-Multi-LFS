using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;

public class FilteredPatientsRoster : MonoBehaviour
{
    [SerializeField]
    GameObject patientButtonPrefab;
    [SerializeField]
    Transform verticalGroup;

    ////List<string> names;
    //List<NewPatientData> alsPatients;
    //List<NewPatientData> blsPatients;
    List<string> names;

    List<PatientToLoadButton> patientButtons;

    bool isShowingALS;
    /// <summary>
    /// False being BLS
    /// </summary>
    /// <param name="isALS"></param>
    public void SetFilterALS(bool isALS)
    {
        isShowingALS = isALS;
        SetUpNamesAsButtons();
    }

    public void SetUpNamesAsButtons()
    {
        if (patientButtons == null)
            patientButtons = new List<PatientToLoadButton>();
        //if (alsPatients == null)
        //    alsPatients = new List<NewPatientData>(); 
        //if(blsPatients == null)
        //    blsPatients= new List<NewPatientData>();
        
        
        names = PatientCreator.GetExistingPatientNames(isShowingALS);
        if (names == null || names.Count == 0)
        {
            Debug.Log("No files to load");
            return;
        }

        if (patientButtons.Count > names.Count)
        {
            int delta = patientButtons.Count - names.Count;
            for (int i = 0; i < delta; i++)
            {
                Destroy(patientButtons[patientButtons.Count - 1]);
                patientButtons.Remove(patientButtons[patientButtons.Count - 1]);
            }
        }
        else if (patientButtons.Count < names.Count)
        {
            int delta = names.Count - patientButtons.Count;
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

}

