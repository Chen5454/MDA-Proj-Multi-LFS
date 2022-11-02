using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace PatientCreationSpace
{

    public class ChooseTestWindow : NewBlockWindow
    {

        [SerializeField]
        UnityEngine.UI.Toggle doClose;
        [SerializeField]
        TMP_Dropdown dropdown;
        [SerializeField]
        TMP_InputField searchText;

        [SerializeField]
        Databases databases;

        [SerializeField]
        BlockCollectionEditor treatmentSequenceEditorWindow;
        public override void OnEnable()
        {
            base.OnEnable();
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
            if(!string.IsNullOrEmpty( searchText.text))
            {
                treatments = databases.testDB.GetTreatmentsWithLinq(x => x.testName.StartsWith(searchText.text)).ToList();
                if(treatments==null|| treatments.Count ==0)
                {
                    treatments = databases.testDB.GetTreatmentsWithLinq(x => x.testName.Contains(searchText.text)).ToList();
                }
            }

            foreach(Test t in treatments)
            {
                strings.Add(t.testName);
            }

            if(strings.Count >0)
            dropdown.AddOptions(strings);
            else
            dropdown.AddOptions(databases.testDB.GetListOfTreatmentNames());

            dropdown.RefreshShownValue();
        }

        public void OnClickAdd()
        {
            treatmentSequenceEditorWindow.AddTreatmentToCollection(databases.testDB.GetTreatmentByIndex(dropdown.value));
            if (doClose.isOn)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
