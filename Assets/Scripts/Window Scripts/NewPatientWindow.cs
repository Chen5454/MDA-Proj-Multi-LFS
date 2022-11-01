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
        TMP_InputField SureName;
        [SerializeField]
        TMP_InputField Id, Age;
        [SerializeField]
        TMP_InputField Gender;
        [SerializeField]
        TMP_InputField PhoneNumber;
        [SerializeField]
        TMP_InputField MedicalCompany, AddressLocation, Complaint;

        /// <summary>
        /// true = ALS | false = BLS
        /// </summary>
        [SerializeField]
        UnityEngine.UI.Toggle IsALS;
           /// <summary>
        /// true = trauma | false = illness
        /// </summary>
        [SerializeField]
        UnityEngine.UI.Toggle IsTrauma;


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
            SureName.text = "";
            Age.text = "";
            Gender.text = "";
            PhoneNumber.text = "";
            MedicalCompany.text = "";
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
            if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(SureName.text) ||
                string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Gender.text)
                 || string.IsNullOrEmpty(PhoneNumber.text) || string.IsNullOrEmpty(MedicalCompany.text)
                  || string.IsNullOrEmpty(AddressLocation.text) || string.IsNullOrEmpty(Complaint.text))
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

            //patientMeasurements.Initialize(measurementArray);

            //Other Settings section TBD



            //get unique ID placeholder - TBD
            //string s = System.DateTime.Now.ToString("m-s");

            //createdPatient = PatientCreator.CreatePatient(s, patient_name.text, patient_age.text);
            newCreatedPatient = PatientCreator.CreateNewPatient(Name.text, SureName.text, 1, 3, Gender.text, PhoneNumber.text, //TBF
                MedicalCompany.text, AddressLocation.text, Complaint.text, measurementArray, IsALS.isOn, IsTrauma.isOn);//parsing for ints is temp TBF


            treatmentSequenceEditorWindow.gameObject.SetActive(true);
            //treatmentSequenceEditorWindow.Init(createdPatient);
            treatmentSequenceEditorWindow.Init(newCreatedPatient);
            //continue work on setting the patient and filling their Treatment Sequence
        }
        public void ClickOnEditLoaded()
        {
            //If measurements were changed! TBF
            if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(SureName.text) ||
                string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Gender.text)
                 || string.IsNullOrEmpty(PhoneNumber.text) || string.IsNullOrEmpty(MedicalCompany.text)
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
            SureName.text = patient.SureName;
            Age.text = patient.Age.ToString();
            Gender.text = patient.Gender;
            PhoneNumber.text = patient.PhoneNumber;
            MedicalCompany.text = patient.MedicalCompany;
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