﻿using System.Collections;
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
        TMP_InputField Age;
        [SerializeField]
        TMP_Dropdown Gender;
        [SerializeField]
        TMP_InputField Weight;
        [SerializeField]
        TMP_InputField Height, Complaint;

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
        GameObject treatmentSequenceEditorWindow;
        [SerializeField]
        PatientRoster patientRoster; //Dont like this either TBF

        [SerializeField]
        AddBlockMaster addBlockMaster;

        //BAD BUT HAVE TO DO THIS FOR NOW:
        [SerializeField]
        GameObject questionBlockPrefab;
        [SerializeField]
        GameObject testBlockPrefab;
        [SerializeField]
        GameObject medicineBlockPrefab;
        [SerializeField]
        GameObject treatmentGroupBlockPrefab;

        //END BAD

        [SerializeField]
        GameObject editSequenceButton;
        [SerializeField]
        GameObject newSequenceButton;


       public PhotonView _photonView;
        public void SetEditOrNew(bool isEdit)
        {
            editSequenceButton.SetActive(isEdit);
            newSequenceButton.SetActive(!isEdit);
        }
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
            //AddressLocation.text = "";
            Complaint.text = "";

            IsALS.DeSetMe();
            IsBLS.DeSetMe();
            IsTrauma.DeSetMe();
            IsIllness.DeSetMe();
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
            
            newCreatedPatient = PatientCreator.CreateNewPatient(Name.text, EventName.text, 1/*TBF! UniqueID*/, int.Parse(Age.text), /*Gender.options[Gender.value].text*/ ((PatientGender)Gender.value).ToString(), Weight.text, //TBF
                Height.text, Complaint.text, measurementArray, ((DestinationRoom)DestinationDropdown.value), IsALS.IsBtnSelected, IsTrauma.IsBtnSelected);//parsing for ints is temp TBF


            treatmentSequenceEditorWindow.SetActive(true);
            //treatmentSequenceEditorWindow.Init(createdPatient);
            //treatmentSequenceEditorWindow.Init(newCreatedPatient);
            //continue work on setting the patient and filling their Treatment Sequence
        }

        public void ClickOnEditExisting()
        {
            //check are REQUIRED(?) fields TBD
            if(PatientCreator.newPatient == null)
            {
                Debug.LogError("no loaded patient found!");
                return;
            }
            
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
            
            newCreatedPatient = PatientCreator.CreateNewPatient(Name.text, EventName.text, 1/*TBF! UniqueID*/, int.Parse(Age.text), /*Gender.options[Gender.value].text*/ ((PatientGender)Gender.value).ToString(), Weight.text, //TBF
                Height.text, Complaint.text, measurementArray, ((DestinationRoom)DestinationDropdown.value), IsALS.IsBtnSelected, IsTrauma.IsBtnSelected);//parsing for ints is temp TBF
            newCreatedPatient.FullTreatmentSequence = PatientCreator.newPatient.FullTreatmentSequence;

            treatmentSequenceEditorWindow.SetActive(true);

            //load up the treatment sequence:
            foreach (var item in newCreatedPatient.FullTreatmentSequence.sequenceBlocks)
            {
                switch (item.typeString)
                {
                    case "TreatmentGroup":

                        TreatmentGroup tg = item as TreatmentGroup;
                        GameObject groupPrefab = Instantiate(treatmentGroupBlockPrefab);
                        TreatmentGroupBlock tgb = groupPrefab.GetComponent<TreatmentGroupBlock>();

                        foreach (var sBlock in tg.SequenceBlocks())
                        {
                            switch (sBlock.typeString)
                            {

                                case "Question":
                                    
                                    QuestionBlock qb1 = Instantiate(questionBlockPrefab).GetComponent<QuestionBlock>();
                                    Question q1 = item as Question;
                                    qb1.SetContent(q1.questionText, q1.answerText);
                                    tgb.myAddBlock.AddInstantiatedBlockToSequence(qb1);

                                    break;
                                case "Test":
                                    
                                    TestBlock tb1 = Instantiate(testBlockPrefab).GetComponent<TestBlock>();
                                    Test t1 = item as Test;
                                    tb1.SetTest(t1);
                                    tgb.myAddBlock.AddInstantiatedBlockToSequence(tb1);

                                    break;
                                case "Medicine":
                                    
                                    MedicineBlock mb1 = Instantiate(medicineBlockPrefab).GetComponent<MedicineBlock>();
                                    Medicine m1 = item as Medicine;
                                    mb1.SetMedicine(m1);
                                    tgb.myAddBlock.AddInstantiatedBlockToSequence(mb1);
                                    break;

                            }
                        }
                                        
                        

                        break;
                    case "Question":
                        GameObject go = Instantiate(questionBlockPrefab);
                        QuestionBlock qb = go.GetComponent<QuestionBlock>();
                        Question q = item as Question;
                        qb.SetContent(q.questionText, q.answerText);
                        addBlockMaster.AddInstantiatedBlockToSequence(qb);

                        break;
                    case "Test":
                        GameObject go1 = Instantiate(testBlockPrefab);
                        TestBlock tb = go1.GetComponent<TestBlock>();
                        Test t = item as Test;
                        tb.SetTest(t);
                        addBlockMaster.AddInstantiatedBlockToSequence(tb);

                        break;
                    case "Medicine":
                        GameObject go2 = Instantiate(medicineBlockPrefab);
                        MedicineBlock mb = go2.GetComponent<MedicineBlock>();
                        Medicine m = item as Medicine;
                        mb.SetMedicine(m);
                        addBlockMaster.AddInstantiatedBlockToSequence(mb);
                        break;

                }

            }
            
            //treatmentSequenceEditorWindow.Init(createdPatient);
            //treatmentSequenceEditorWindow.Init(newCreatedPatient);
            //continue work on setting the patient and filling their Treatment Sequence
        }


        //public void ClickOnEditLoaded()
        //{
        //    //If measurements were changed! TBF
        //    if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(EventName.text) ||
        //        string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Gender.options[Gender.value].text)
        //         || string.IsNullOrEmpty(Weight.text) || string.IsNullOrEmpty(Height.text)
        //           || string.IsNullOrEmpty(Complaint.text))
        //    {
        //        Debug.LogError("all basic info fields need to be filled!");
        //        return;
        //    }
        //    //Initial Measurements nullorempty checks: in the grabbing bleow

        //    PatientMeasurements patientMeasurements = new PatientMeasurements();

        //    string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
        //    for (int i = 0; i < measurementInputFields.Count; i++)
        //    {
        //        if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
        //        {
        //            Debug.LogError("all initial measurement fields need to be filled!");

        //            return;
        //        }
        //        measurementArray[i] = measurementInputFields[i].text;
        //    }
        //    PatientCreator.newPatient.SetPatientMeasurement(measurementArray); //sets new measurements


        //    treatmentSequenceEditorWindow.SetActive(true);
        //    //treatmentSequenceEditorWindow.Init(createdPatient);
        //    //treatmentSequenceEditorWindow.Init(PatientCreator.newPatient);
        //    //continue work on setting the patient and filling their Treatment Sequence
        //}

        [PunRPC]
        public void CallPatientCreator(string patientJson, string treatmentSequenceJson,string name,string sureName)
        {
            PatientCreator.CreateSaveFiles(patientJson, treatmentSequenceJson,name,sureName);
        }

        public void SavePatient()
        {
            //PatientCreator.SaveCurrentPatient();
            TreatmentSequence treatmentSequence = new TreatmentSequence();
            treatmentSequence.Init();
            //WAITING WITH ALL THIS - need to see if groups don't need parsing but will just return the TreatmentGroup 

            foreach (var item in addBlockMaster.basicBlocks) 
            {
                Treatment t = item.GetTreatment();
                if (t!=null)
                {
                    //Treatment confirmed!
                    treatmentSequence.AddToSequence(t as SequenceBlock);
                    //if(t is Question)
                    //{

                    //}
                    //else if(t is Test)
                    //{

                    //}
                    //else if (t is Medicine)
                    //{

                    //}
                }
                else
                {
                    TreatmentGroup tg = item.GetTreatmentGroup();
                    if(tg ==null)
                    {
                        //neither treatment nor group??
                        Debug.LogError("this block returns neither treatment nor group");
                        continue;
                    }
                    //TreatmentGroup confirmed!
                    treatmentSequence.AddToSequence(tg as SequenceBlock);
                }
            }
            PatientCreator.newPatient.FullTreatmentSequence = treatmentSequence;

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
            int option = -1;
            if(patient.Gender.Equals("זכר"))
            {
                option = (int)PatientGender.זכר;
            }
                else if(patient.Gender.Equals("נקבה"))
            {
                option = (int)PatientGender.נקבה;

            }
            else
            {
                Debug.LogError("No binary options sadly are not available :(");
                return;
            }
            Gender.value = option;
            Weight.text = patient.PhoneNumber;
            Height.text = patient.MedicalCompany;
            //AddressLocation.text = patient.AddressLocation;
            Complaint.text = patient.Complaint;

            if (patient.isALS)
                IsALS.ClickMe();
            else
                IsBLS.ClickMe();

            if (patient.isTrauma)
                IsTrauma.ClickMe();
            else
                IsIllness.ClickMe();

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