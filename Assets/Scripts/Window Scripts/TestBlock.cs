using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PatientCreationSpace;

public class TestBlock : MonoBehaviour, BasicBlock
{
    [SerializeField]
    TMP_Dropdown dropdown;
    [SerializeField]
    TMP_InputField searchText;

    bool _isInteractable;

    Databases databases => Databases.Instance;
    AddBlockMaster abm;
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

    public Treatment GetTreatment()
    {
        return databases.testDB.GetTreatmentByIndex(dropdown.value);
    }
    public TreatmentGroup GetTreatmentGroup()
    {
        return null;
    }
    public bool IsInteractable()
    {
        return _isInteractable;
    }

    public  void OnEnable()
    {
        if (databases == null)
        {
            Debug.LogError("Test database not found!");
            return;
        }

        RefreshDropdownTests();
    }

    public void RefreshDropdownTests()
    {
        dropdown.ClearOptions();
        List<Test> treatments = new List<Test>();
        List<string> strings = new List<string>();
        if (!string.IsNullOrEmpty(searchText.text))
        {
            treatments = databases.testDB.GetTreatmentsWithLinq(x => x.testName.StartsWith(searchText.text)).ToList();
            if (treatments == null || treatments.Count == 0)
            {
                treatments = databases.testDB.GetTreatmentsWithLinq(x => x.testName.Contains(searchText.text)).ToList();
            }
        }

        foreach (Test t in treatments)
        {
            strings.Add(t.testName);
        }

        if (strings.Count > 0)
            dropdown.AddOptions(strings);
        else
            dropdown.AddOptions(databases.testDB.GetListOfTreatmentNames());

        dropdown.RefreshShownValue();
    }

   

    public void SetInteractable(bool isInteractable) //set solely by in scene buttons
    {
        dropdown.interactable = isInteractable;
        searchText.interactable = isInteractable;
        _isInteractable = isInteractable;
    }

    GameObject BasicBlock.gameObject()
    {
        return gameObject;
    }

    //public override void Save()
    //{
    //    base.Save();
    //}
    //public override void Cancel()
    //{
    //    base.Cancel();
    //}
    //public void OnClickAdd()
    //{
    //    treatmentSequenceEditorWindow.AddTreatmentToCollection(databases.testDB.GetTreatmentByIndex(dropdown.value));
    //}
}

