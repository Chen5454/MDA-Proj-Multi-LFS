using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
namespace PatientCreationSpace
{

    public class NewPatientWindow : MonoBehaviour
    {

        public static NewPatientWindow Instance;

        [Header("Bsaic Info Input Fields")]
        [SerializeField]
        TMP_InputField Name;
        [SerializeField]
        TMP_InputField EventName;
        [SerializeField]
        TMP_InputField Id, Age;
        [SerializeField]
        TMP_Dropdown Gender;
        [SerializeField]
        TMP_InputField Weight;
        [SerializeField]
        TMP_InputField Height, AddressLocation, Complaint;

        [SerializeField]
        TMP_Dropdown DestinationDropdown;

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


        [SerializeField]
        List<TMP_InputField> measurementInputFields;


        Patient createdPatient;
        NewPatientData newCreatedPatient;

        [SerializeField]
        TreatmentSequenceEditorWindow treatmentSequenceEditorWindow;
        [SerializeField]
        PatientRoster patientRoster; //Dont like this either TBF

       public PhotonView _photonView;
        //private void Start()
        //{
        //    LoadPatient("ש​_נ​");
        //}
        private void OnEnable()
        {
            PatientCreator.OnLoadPatient += DisplayPatient;
        }
        private void OnDisable()
        {
            PatientCreator.OnLoadPatient -= DisplayPatient;
            ClearPatientInfoFields();
            ClearPatientMeasurementFields();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// this also clears the currently loaded patient
        /// </summary>
        public void ClearPatientInfoFields() //inspector button
        {
            Name.text = "";
            EventName.text = "";
            Age.text = "";
            Gender.value = 0;
            Weight.text = "";
            Height.text = "";
            AddressLocation.text = "";
            Complaint.text = "";

            //PatientCreator.ClearLoadedPatient();
        }
        public void ClearPatientMeasurementFields()//inspector button
        {
            foreach (var item in measurementInputFields)
            {
                item.text = "";
            }
        }

        public void ClickOnCreateNew()
        {
            //check are REQUIRED(?) fields TBD

            //Basic info nullorempty checks:
            if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(EventName.text) ||
                string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Gender.options[Gender.value].text)
                 || string.IsNullOrEmpty(Weight.text) || string.IsNullOrEmpty(Height.text)
                  ||  string.IsNullOrEmpty(Complaint.text))
            {
                Debug.LogError("all basic info fields need to be filled!");
                return;
            }
            //Initial Measurements nullorempty checks: in the grabbing bleow

            //PatientMeasurements patientMeasurements = new PatientMeasurements();

            string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
                {
                    Debug.LogError("all initial measurement fields need to be filled!");

                    return;
                }
                measurementArray[i] = measurementInputFields[i].text;
            }
            if(!IsALS.IsBtnSelected && !IsTrauma.IsBtnSelected && !IsBLS.IsBtnSelected && !IsIllness.IsBtnSelected)
            {
                Debug.LogError("no type selected somehow?");
                return;
            }
            //patientMeasurements.Initialize(measurementArray);

            //Other Settings section TBD



            //get unique ID placeholder - TBD
            //string s = System.DateTime.Now.ToString("m-s");

            //createdPatient = PatientCreator.CreatePatient(s, patient_name.text, patient_age.text);
            newCreatedPatient = PatientCreator.CreateNewPatient(Name.text, EventName.text, 1/*TBF! UniqueID*/, int.Parse(Age.text), Gender.options[Gender.value].text, Weight.text, //TBF
                Height.text, Complaint.text, measurementArray, ((DestinationRoom)DestinationDropdown.value), IsALS.IsBtnSelected, IsTrauma.IsBtnSelected);//parsing for ints is temp TBF


            treatmentSequenceEditorWindow.gameObject.SetActive(true);
            //treatmentSequenceEditorWindow.Init(createdPatient);
            treatmentSequenceEditorWindow.Init(newCreatedPatient);
            //continue work on setting the patient and filling their Treatment Sequence
        }
        public void ClickOnEditLoaded()
        {
            //If measurements were changed! TBF
            if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(EventName.text) ||
                string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Gender.options[Gender.value].text)
                 || string.IsNullOrEmpty(Weight.text) || string.IsNullOrEmpty(Height.text)
                  || string.IsNullOrEmpty(AddressLocation.text) || string.IsNullOrEmpty(Complaint.text))
            {
                Debug.LogError("all basic info fields need to be filled!");
                return;
            }
            //Initial Measurements nullorempty checks: in the grabbing bleow

            PatientMeasurements patientMeasurements = new PatientMeasurements();

            string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
                {
                    Debug.LogError("all initial measurement fields need to be filled!");

                    return;
                }
                measurementArray[i] = measurementInputFields[i].text;
            }
            PatientCreator.newPatient.SetPatientMeasurement(measurementArray); //sets new measurements


            treatmentSequenceEditorWindow.gameObject.SetActive(true);
            //treatmentSequenceEditorWindow.Init(createdPatient);
            treatmentSequenceEditorWindow.Init(PatientCreator.newPatient);
            //continue work on setting the patient and filling their Treatment Sequence
        }

        [PunRPC]
        public void CallPatientCreator(string patientJson, string treatmentSequenceJson,string name,string sureName)
        {
            PatientCreator.CreateSaveFiles(patientJson, treatmentSequenceJson,name,sureName);
        }

        public void SavePatient()
        {
            //PatientCreator.SaveCurrentPatient();
            PatientCreator.SaveNewPatient();
        }
        public void LoadPatient(string patientName)
        {
            PatientCreator.LoadPatient(patientName);
            //Invoke("DisplayPatient", 1);
        }
        public void DisplayPatient()
        {
            if (PatientCreator.newPatient == null)
            {
                Debug.LogError("how?");
                return;
            }

            NewPatientData patient = PatientCreator.newPatient;

            Name.text = patient.Name;
            EventName.text = patient.SureName;
            Age.text = patient.Age.ToString();
            Gender.value = int.Parse(patient.Gender);
            Weight.text = patient.PhoneNumber;
            Height.text = patient.MedicalCompany;
            AddressLocation.text = patient.AddressLocation;
            Complaint.text = patient.Complaint;

            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                measurementInputFields[i].text = patient.GetMeasurement(i);
                //measurementInputFields[i].ForceMeshUpdate(true);
            }

        }
        public void RefreshLoadedPatients()
        {
            patientRoster.SetUpNamesAsButtons();
        }


   
    }
    //cancel patient creation - delete all SOs that need to be deleted (keep questions, because why not?)
    //public void CancelPatientCreation()
    //{
    //    if(createdPatient == null)
    //    {
    //        Debug.LogError("no patient to cancel!");
    //        return;
    //    }    
    //}



}