using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;
using WebSocketSharp;

namespace PatientCreationSpace
{

    public class NewPatientWindow : MonoBehaviour
    {

        public static NewPatientWindow Instance;

        [Header("Bsaic Info Input Fields")]
        [SerializeField]
        TMP_Dropdown PatientType;
        [SerializeField]
        TMP_InputField Name;
        [SerializeField]
        TMP_InputField EventName;
        [SerializeField]
        TMP_InputField Age;
        [SerializeField] private TMP_Dropdown _patientType;
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

        [SerializeField] private ToggleButton _isMale, _isFemale;


        [SerializeField]
        //UnityEngine.UI.Toggle IsTrauma;
        ToggleButton IsTrauma;
        [SerializeField]
        //UnityEngine.UI.Toggle IsTrauma;
        ToggleButton IsIllness;


        [SerializeField]
        List<TMP_InputField> measurementInputFields;

        [SerializeField] private List<TMP_Dropdown> _measurementDropdowns;

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
        GameObject editSequenceButtonNav, newSequenceButtonNav;

        public GameObject EditSequenceButton, NewSequenceButton;


        public PhotonView _photonView;
        public void SetEditOrNew(bool isEdit)
        {
            editSequenceButtonNav.SetActive(isEdit);
            newSequenceButtonNav.SetActive(!isEdit);
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
            _patientType.value = 0;
            Weight.text = "";
            Height.text = "";
            //AddressLocation.text = "";
            Complaint.text = "";
            DestinationDropdown.value = 0;

            _isMale.DeSetMe(true);
            _isFemale.DeSetMe(false);
            IsBLS.DeSetMe(true);
            IsALS.DeSetMe(false);
            IsTrauma.DeSetMe(true);
            IsIllness.DeSetMe(false);
            //PatientCreator.ClearLoadedPatient();
        }

        public bool AreAllBasicInfoFieldFilled()
        {
            //Basic info nullorempty checks:
            if (!
                (string.IsNullOrEmpty(Name.text) ||
                string.IsNullOrEmpty(EventName.text) ||
                string.IsNullOrEmpty(Age.text) || //!float.TryParse(Age.text, out float nothing)) || //age is filled, and a number
                string.IsNullOrEmpty(Weight.text) || 
                string.IsNullOrEmpty(Height.text)|| 
                string.IsNullOrEmpty(Complaint.text)))
            {
                if (((!IsALS.IsBtnSelected && !IsBLS.IsBtnSelected) ||
                  (!IsTrauma.IsBtnSelected && !IsIllness.IsBtnSelected) ||
                  (!_isMale.IsBtnSelected && !_isFemale.IsBtnSelected)))
                {

                    return false;
                }
                return true;
            }
            //implied else 
            return false;
        }

        public bool AreAllInitialMeasurementsFilled()
        {
            foreach (TMP_InputField item in measurementInputFields)
            {
                if (item == measurementInputFields[5] || item == measurementInputFields[7])
                    continue;

                if (string.IsNullOrEmpty(item.text))
                return false;
            }

            foreach (TMP_Dropdown item in _measurementDropdowns)
            {
                if (string.IsNullOrEmpty(item.itemText.text))
                    return false;
            }

            //foreach (TMP_Dropdown dropdown in _measurementDropdowns)
            //{
            //    foreach (TMP_Dropdown.OptionData option in dropdown.options)
            //    {
            //        if (string.IsNullOrEmpty(option.text))
            //            return false;
            //    }
            //}

            return true;
        }
        public bool AreAllTreatmentFieldsFilled()
        {
            if (addBlockMaster.basicBlocks == null || addBlockMaster.basicBlocks.Count == 0)
                return false;

            foreach (var item in addBlockMaster.basicBlocks)
            {
                if (!item.AllInputsGood())
                    return false;
            }

            //could need to add dropdown measurements
            return true;
        }

        public void ClearPatientMeasurementFields()//inspector button
        {
            foreach (var item in measurementInputFields)
            {
                item.text = "";
            }

            // clear dropdowns
        }

        public void ClickOnCreateNew()
        {
            //TO BE REMOVED!

            //Basic info nullorempty checks:
            if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(EventName.text) 
                || string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Weight.text)
                || string.IsNullOrEmpty(Height.text) ||  string.IsNullOrEmpty(Complaint.text))
            {
                Debug.LogError("all basic info fields need to be filled!");
                return;
            }
            if(!IsALS.IsBtnSelected && !IsTrauma.IsBtnSelected && !IsBLS.IsBtnSelected && !IsIllness.IsBtnSelected && !_isMale.IsBtnSelected && !_isFemale.IsBtnSelected)
            {
                Debug.LogError("no type selected somehow?");
                return;
            }

            //TO BE REMOVED!

            //Initial Measurements nullorempty checks: in the grabbing bleow

            //PatientMeasurements patientMeasurements = new PatientMeasurements();

            string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                if (i == 5 || i == 7)
                    continue; // deal with the things below later (dropdown)
                    //if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
                        //measurementInputFields[i].text = "0";

                if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
                {
                    Debug.LogError("all initial measurement fields need to be filled!");

                    return;
                }
                measurementArray[i] = measurementInputFields[i].text;
            }

            measurementArray[(int)Measurements.אקג] = _measurementDropdowns[0].options[_measurementDropdowns[0].value].text;
            measurementArray[(int)Measurements.הכרה] = _measurementDropdowns[1].options[_measurementDropdowns[1].value].text;

            //if (string.IsNullOrEmpty(_measurementDropdowns[i].options[_measurementDropdowns[i].value].text)) //Initial //Measurements nullorempty checks here!
            //{
            //    Debug.LogError("all initial measurement fields need to be filled!");
            //
            //    return;
            //}

            //string[] measurementDropdownArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
            //for (int i = 0; i < _measurementDropdowns.Count; i++)
            //{
            //    for (int option = 0; option < _measurementDropdowns[i].options.Count; option++)
            //    {
            //        if (string.IsNullOrEmpty(_measurementDropdowns[i].options[i].text)) //Initial Measurements //nullorempty checks here!
            //        {
            //            Debug.LogError("all initial measurement fields need to be filled!");
            //
            //            return;
            //        }
            //
            //        measurementDropdownArray[i] = _measurementDropdowns[i].options[i].text;
            //    }
            //    foreach (TMP_Dropdown.OptionData option in _measurementDropdowns[i].options)
            //    {
            //        
            //
            //        
            //    }
            //    
            //    
            //}

            newCreatedPatient = PatientCreator.CreateNewPatient(PatientType.value, Name.text, EventName.text, 1/*TBF! UniqueID*/, int.Parse(Age.text), /*Gender.options[Gender.value].text*/ _isMale.IsBtnSelected, _isFemale.IsBtnSelected, Weight.text, //TBF
                Height.text, Complaint.text, measurementArray, ((DestinationRoom)DestinationDropdown.value), IsALS.IsBtnSelected, IsTrauma.IsBtnSelected);//parsing for ints is temp TBF

            treatmentSequenceEditorWindow.SetActive(true);

            RequestTest.Instance.SetWriteRangeToNewRow();
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
            if (string.IsNullOrEmpty(Name.text) || string.IsNullOrEmpty(EventName.text)
                ||string.IsNullOrEmpty(Age.text) || string.IsNullOrEmpty(Weight.text)
                || string.IsNullOrEmpty(Height.text) ||  string.IsNullOrEmpty(Complaint.text))
            {
                Debug.LogError("all basic info fields need to be filled!");
                return;
            }
            //Initial Measurements nullorempty checks: in the grabbing bleow

            //PatientMeasurements patientMeasurements = new PatientMeasurements();

            string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                if (i == 5 || i == 7)
                    continue;

                if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
                {
                    Debug.LogError("all initial measurement fields need to be filled!");

                    return;
                }
                measurementArray[i] = measurementInputFields[i].text;
            }

            measurementArray[(int)Measurements.אקג] = _measurementDropdowns[0].options[_measurementDropdowns[0].value].text;
            measurementArray[(int)Measurements.הכרה] = _measurementDropdowns[1].options[_measurementDropdowns[1].value].text;

            //measurementArray[(int)Measurements.אקג] = _measurementDropdowns.
            if (!IsALS.IsBtnSelected && !IsTrauma.IsBtnSelected && !IsBLS.IsBtnSelected && !IsIllness.IsBtnSelected && !_isMale.IsBtnSelected && !_isFemale.IsBtnSelected)
            {
                Debug.LogError("no type selected somehow?");
                return;
            }
            
            //newCreatedPatient = PatientCreator.CreateNewPatient(Name.text, EventName.text, 1/*TBF! UniqueID*/, int.Parse(Age.text), /*Gender.options[Gender.value].text*/ ((PatientGender)Gender.value).ToString(), Weight.text, //TBF
            //    Height.text, Complaint.text, measurementArray, ((DestinationRoom)DestinationDropdown.value), IsALS.IsBtnSelected, IsTrauma.IsBtnSelected);//parsing for ints is temp TBF
            
            treatmentSequenceEditorWindow.SetActive(true);
            
            //load up the treatment sequence:
            foreach (var item in PatientCreator.newPatient.FullTreatmentSequence.sequenceBlocks)
            {
                switch (item.typeString)
                {
                    case "TreatmentGroup":

                        TreatmentGroup tg = item as TreatmentGroup;
                        GameObject groupPrefab = Instantiate(treatmentGroupBlockPrefab);
                        TreatmentGroupBlock tgb = groupPrefab.GetComponent<TreatmentGroupBlock>();
                        addBlockMaster.AddInstantiatedBlockToSequence(tgb);

                        foreach (var sBlock in tg.SequenceBlocks())
                        {
                            switch (sBlock.typeString)
                            {

                                case "Question":
                                    QuestionBlock qb1 = Instantiate(questionBlockPrefab).GetComponent<QuestionBlock>();
                                    Question q1 = sBlock as Question;
                                    qb1.SetContent(q1.questionText, q1.answerText);
                                    tgb.myAddBlock.AddInstantiatedBlockToSequence(qb1);

                                    break;
                                case "Test":
                                    
                                    TestBlock tb1 = Instantiate(testBlockPrefab).GetComponent<TestBlock>();
                                    Test t1 = sBlock as Test;
                                    tb1.SetTest(t1);
                                    tgb.myAddBlock.AddInstantiatedBlockToSequence(tb1);

                                    break;
                                case "Medicine":
                                    
                                    MedicineBlock mb1 = Instantiate(medicineBlockPrefab).GetComponent<MedicineBlock>();
                                    Medicine m1 = sBlock as Medicine;
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
            if(newCreatedPatient == null)
            {
                newCreatedPatient = new NewPatientData();
            }
            newCreatedPatient.Initialize(PatientType.value, Name.text, EventName.text, 1/*TBF! UniqueID*/, int.Parse(Age.text), /*Gender.options[Gender.value].text*/ _isMale.IsBtnSelected, Weight.text, //TBF
                Height.text, Complaint.text, measurementArray, ((DestinationRoom)DestinationDropdown.value), IsALS.IsBtnSelected, IsTrauma.IsBtnSelected);
            newCreatedPatient.FullTreatmentSequence = PatientCreator.newPatient.FullTreatmentSequence;

            PatientCreator.newPatient = newCreatedPatient;
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

        //[PunRPC]
        //public void CallPatientCreator(string patientJson, string treatmentSequenceJson,string name,string sureName)
        //{
        //    PatientCreator.CreateSaveFiles(patientJson, treatmentSequenceJson,name,sureName);
        //}

        public void SavePatient() //CALLED BY INSPECTOR! THIS STARTS SAVING
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
                Debug.LogError("No newPatient");
                return;
            }

            NewPatientData patient = PatientCreator.newPatient;

            Name.text = patient.Name;
            EventName.text = patient.SureName;
            Age.text = patient.Age.ToString();
            int option = -1;

            if (patient.Gender == PatientGender.Male)
                _isMale.ClickMe();
            else
                _isFemale.ClickMe();

            for (int i = 0; i < _patientType.options.Count; i++)
            {
                if (i == (int)patient.PatientType)
                {
                    _patientType.value = i;
                    break;
                }
            }

            for (int i = 0; i < _measurementDropdowns[0].options.Count; i++)
            {
                if (_measurementDropdowns[0].options[i].text == patient.GetMeasurement(Measurements.אקג))
                {
                    _measurementDropdowns[0].value = i;
                    break;
                }
            }
            Weight.text = patient.PhoneNumber;
            Height.text = patient.MedicalCompany;
            //AddressLocation.text = patient.AddressLocation;
            Complaint.text = patient.Complaint;

            

            if (patient.isTrauma)
                IsTrauma.ClickMe();
            else
                IsIllness.ClickMe();

            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                if (i == 5 || i == 7)
                    continue;

                measurementInputFields[i].text = patient.GetMeasurement(i);
                //measurementInputFields[i].ForceMeshUpdate(true);
            }

            for (int i = 0; i < _measurementDropdowns[0].options.Count; i++)
            {
                if (_measurementDropdowns[0].options[i].text == patient.GetMeasurement(Measurements.אקג))
                {
                    _measurementDropdowns[0].value = i;
                    break;
                }
            }

            for (int i = 0; i < _measurementDropdowns[1].options.Count; i++)
            {
                if (_measurementDropdowns[1].options[i].text == patient.GetMeasurement(Measurements.הכרה))
                {
                    _measurementDropdowns[1].value = i;
                    break;
                }
            }
        }
        public void RefreshLoadedPatients()
        {
            if(patientRoster)
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