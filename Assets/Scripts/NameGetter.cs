using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;
[CreateAssetMenu()]
public class NameGetter : MonoBehaviour
{
    public List<string> _currentNames;
    public List<GameObject> _selected;

    TreatmentDB<Test> testDB;

    public void GetNamesOfSelected()
    {
        if (_selected.Count == 0)
            return;

        _currentNames = new List<string>();
        foreach (var item in _selected)
        {
            _currentNames.Add(item.name);
        }
    }
    [ContextMenu("Get from text children")]
     public void GetTextValuesFromSelectedChildren()
    {
        if (_selected.Count == 0)
            return;
        _currentNames = new List<string>();
        foreach (var item in _selected)
        {
            TMPro.TMP_Text textBox = item.GetComponentInChildren<TMPro.TMP_Text>();
            _currentNames.Add(textBox.text);
        }
    }
     [ContextMenu("Get from text children, but only new ones please")]
     public void GetNEWTextValuesFromSelectedChildren()
    {
        if (_selected.Count == 0)
            return;
        //_currentNames = new List<string>();
        

        foreach (var item in _selected)
        {
            TMPro.TMP_Text textBox = item.GetComponentInChildren<TMPro.TMP_Text>();
            if(!_currentNames.Contains(textBox.text))
            _currentNames.Add(textBox.text);
        }
    }
    [ContextMenu("Remove from list text from children")]
     public void RemoveNEWTextValuesFromSelectedChildren()
    {
        if (_selected.Count == 0)
            return;
        //_currentNames = new List<string>();
        

        foreach (var item in _selected)
        {
            TMPro.TMP_Text textBox = item.GetComponentInChildren<TMPro.TMP_Text>();
            _currentNames.Remove(textBox.text);
            //_currentNames.Add(textBox.text);
        }
    }

    [ContextMenu("Save")]

    public void SaveToDB()
    {
        testDB = new TreatmentDB<Test>();
        List<Test> meds = new List<Test>();

        foreach (var item in _currentNames)
        {
            Test temp = new Test();
            temp.testName = item;
            meds.Add(temp);
        }
        testDB.treatments = meds;
        testDB.SaveDatabase();
    }

}
