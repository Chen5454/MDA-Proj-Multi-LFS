using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PatientCreationSpace;

public enum MedicineApplicationMethod {Po,Iv,IM,IO,SC,SL}

public class MedicineBlock : MonoBehaviour, BasicBlock
{ 
    [SerializeField]
    TMP_Dropdown dropdown;
    [SerializeField]
    TMP_Dropdown applicationMethodDropdown;

    [SerializeField]
    TMP_InputField searchText;

    [SerializeField]
    TMP_Dropdown measurementToReveal;

    [SerializeField]
    List<TMP_InputField> measurementInputFields;

    [SerializeField]
    TMP_InputField minDosageInputField;
    [SerializeField]
    TMP_InputField maxDosageInputField;

    //PatientMeasurementInput TBF!
    //^^^ much like a displayer, this component will provide string input fields, corresponding to the parameters of patientMeasurementData.
    //^^^ this UI prefab will then easily parse all input fields (each field to int/float/enum etc...) - to create a fresh patientMeasurementData.
    //^^^ use this to initially set all patientMeasurementData on a new patient AND for medicine effect (as a "delta_patientMeasurementData" -> where only "fields to be changed" have value)

    
    Databases databases => Databases.Instance;
    

    bool _isInteractable;
    AddBlockMaster abm;

    public void SetMedicine(Medicine m)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text.Equals(m.medicineName))
            {
                dropdown.value = i;
                dropdown.RefreshShownValue();
            }
        }

        for (int i = 0; i < measurementInputFields.Count; i++)
        {
            if(m.measurements.MeasurementValues[i] == "")
            {
                continue;
            }
            measurementInputFields[i].transform.parent.gameObject.SetActive(true);
            measurementInputFields[i].text = m.measurements.MeasurementValues[i];
        }
        minDosageInputField.text = m.minDosage.ToString();
        maxDosageInputField.text = m.maxDosage.ToString();
        applicationMethodDropdown.value = m.applicationMethod;
        applicationMethodDropdown.RefreshShownValue();

    }

    public AddBlockMaster addBlockMaster()
    {
        return abm;
    }

    public void DestroyMe()
    {
        abm.basicBlocks.Remove(this);
        Destroy(gameObject);
    }
    public void SetAddBlockMaster(AddBlockMaster addBlockMaster)
    {
        abm = addBlockMaster;
    }
    public void OnEnable()
    {
        if (databases == null)
        {
            Debug.LogError("Test database not found!");
            return;
        }
        RefreshDropdownMedicine();


    }
    private void Start()
    {
        measurementToReveal.ClearOptions();
        measurementToReveal.AddOptions(System.Enum.GetNames(typeof(Measurements)).ToList());
    }
    //public void OnDisable()
    //{
    //    Invoke(nameof(LateDisable),1);
    //}

    //private void LateDisable()
    //{
    //    foreach (var item in measurementInputFields)
    //    {
    //        item.text = "";
    //        item.gameObject.SetActive(false);
    //    }
    //    measurementToReveal.value = 0;
    //}

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
        List<string> medAppMethods = System.Enum.GetNames(typeof(MedicineApplicationMethod)).ToList();
        applicationMethodDropdown.ClearOptions();
        applicationMethodDropdown.AddOptions(medAppMethods);
    }

    public void RevealMeasurement()
    {
        measurementInputFields[measurementToReveal.value].transform.parent.gameObject.SetActive(true); //Turns on the whole title and field 
    }

    public Treatment GetTreatment()
    {
        Medicine temp = databases.medicineDB.GetTreatmentByIndex(dropdown.value);

        PatientMeasurements patientMeasurements = new PatientMeasurements();

        string[] measurementArray = new string[System.Enum.GetValues(typeof(Measurements)).Length];
        for (int i = 0; i < measurementInputFields.Count; i++)
        {
            measurementArray[i] = measurementInputFields[i].text;   
        }
        patientMeasurements.SetMeasurementValues(measurementArray);
        return MedicineCreator.CreateMedicine(temp.ID(), dropdown.options[dropdown.value].text, patientMeasurements, float.Parse(minDosageInputField.text), float.Parse(maxDosageInputField.text), applicationMethodDropdown.value);
    }

    public TreatmentGroup GetTreatmentGroup()
    {
        return null;
    }

    public bool IsInteractable()
    {
        return _isInteractable;
    }

    public void SetInteractable(bool isInteractable)
    {
        minDosageInputField.interactable = isInteractable;
        maxDosageInputField.interactable = isInteractable;

        foreach (var item in measurementInputFields)
        {
            item.interactable = isInteractable;
        }

        measurementToReveal.interactable = isInteractable;
        searchText.interactable = isInteractable;
        dropdown.interactable = isInteractable;

        _isInteractable = isInteractable;
    }

    GameObject BasicBlock.gameObject()
    {
        return gameObject;
    }

    public bool AllInputsGood()
    {
        bool toReturn = false;
        foreach (var inputField in measurementInputFields)
        {
            if (!inputField.gameObject.activeInHierarchy)
                continue;

            if (!string.IsNullOrEmpty(inputField.text)) //AT LEAST ONE REAVEALED FIELD IS FILLED! we can make sure all revealed fields are filled, but then add a Remove field button! TBD TBF
                toReturn = true;
        }

        if(!float.TryParse(minDosageInputField.text, out float nothing) ||
            !float.TryParse(maxDosageInputField.text, out float nothing1))
        {
            return false;
        }
        return toReturn;
    }
}
