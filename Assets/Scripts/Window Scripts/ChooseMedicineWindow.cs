using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace PatientCreationSpace
{

    public class ChooseMedicineWindow : NewBlockWindow
    {
        [SerializeField]
        UnityEngine.UI.Toggle doClose;
        [SerializeField]
        TMP_Dropdown dropdown;

        [SerializeField]
        TMP_InputField searchText;
        //[SerializeField]
        //TMP_InputField TEMP_patientData; //Look below.

        [SerializeField]
        List<TMP_InputField> measurementInputFields;
        [SerializeField]
        TMP_InputField dosageInputField;
        //List<TMP_Text> measurementInputFields;

        //PatientMeasurementInput TBF!
        //^^^ much like a displayer, this component will provide string input fields, corresponding to the parameters of patientMeasurementData.
        //^^^ this UI prefab will then easily parse all input fields (each field to int/float/enum etc...) - to create a fresh patientMeasurementData.
        //^^^ use this to initially set all patientMeasurementData on a new patient AND for medicine effect (as a "delta_patientMeasurementData" -> where only "fields to be changed" have value)

        [SerializeField]
        Databases databases;
        [SerializeField]
        MedicineDB medicineDatabase;

        [SerializeField]
        BlockCollectionEditor treatmentSequenceEditorWindow;

        public override void OnEnable()
        {
            if (databases == null)
            {
                Debug.LogError("Test database not found!");
                return;
            }

            RefreshDropdownMedicine();
            base.OnEnable();
        }
        public override void OnDisable()
        {
            //set all fields to nothing
            //TEMP_patientData.text = "";
            foreach (var item in measurementInputFields)
            {
                item.text = "";
            }
            base.OnDisable();
        }
        public void RefreshDropdownMedicine()
        {
            dropdown.ClearOptions();
            List<Medicine> treatments = new List<Medicine>();
            List<string> strings = new List<string>();
            if (!string.IsNullOrEmpty(searchText.text))
            {
                treatments = databases.medicineDB.GetTreatmentsWithLinq(x => x.medicineName.StartsWith(searchText.text)).ToList();
                if (treatments == null || treatments.Count == 0)
                {
                    treatments = databases.medicineDB.GetTreatmentsWithLinq(x => x.medicineName.Contains(searchText.text)).ToList();
                }
            }

            foreach (Medicine m in treatments)
            {
                strings.Add(m.medicineName);
            }

            if (strings.Count > 0)
                dropdown.AddOptions(strings);
            else
                dropdown.AddOptions(databases.medicineDB.GetListOfTreatmentNames());



         
            dropdown.RefreshShownValue();
        }

        public void OnClickAdd() //Set in inspector
        {
            //if (string.IsNullOrEmpty(TEMP_patientData.text))
            //    return;

            Medicine temp = databases.medicineDB.GetTreatmentByIndex(dropdown.value);

            PatientMeasurements patientMeasurements = new PatientMeasurements();

            string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
            for (int i = 0; i < measurementInputFields.Count; i++)
            {
                if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
                {
                    measurementArray[i] = "";
                    continue;
                }
                measurementArray[i] = measurementInputFields[i].text;
            }
            patientMeasurements.SetMeasurementValues(measurementArray);
            //Medicine med = MedicineCreator.CreateMedicine(temp.ID(), temp.medicineName, TEMP_patientData.text);
            Medicine med = MedicineCreator.CreateMedicine(temp.ID(), temp.medicineName, patientMeasurements, dosageInputField.text);

            //med.Init(TEMP_patientData.text); //This needs to just send patientMeasurementData tbf
            //Set med's result (as PatientMeasurementData - taken from the PatientMeasurementInput TBF!

            treatmentSequenceEditorWindow.AddTreatmentToCollection(med);
            if (doClose.isOn)
            {
                gameObject.SetActive(false);
            }
        }

    }

}