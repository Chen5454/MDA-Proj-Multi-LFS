using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PatientCreationSpace;

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
        applicationMethodDropdown.value = m.applicationMethod;
        for (int i = 0; i < measurementInputFields.Count; i++)
        {
            measurementInputFields[i].text = m.measurements.MeasurementValues[i];
        }
        minDosageInputField.text = m.minDosage.ToString();
        maxDosageInputField.text = m.maxDosage.ToString();

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
    public void OnDisable()
    {
        foreach (var item in measurementInputFields)
        {
            item.text = "";
            item.gameObject.SetActive(false);
        }
        measurementToReveal.value = 0;
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
            if (string.IsNullOrEmpty(measurementInputFields[i].text)) //Initial Measurements nullorempty checks here!
            {
                measurementArray[i] = "";
                continue;
            }
            measurementArray[i] = measurementInputFields[i].text;
        }
        patientMeasurements.SetMeasurementValues(measurementArray);
        return MedicineCreator.CreateMedicine(temp.ID(), temp.medicineName, patientMeasurements, float.Parse(minDosageInputField.text), float.Parse(maxDosageInputField.text), applicationMethodDropdown.value);
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
}
