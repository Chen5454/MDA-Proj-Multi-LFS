using System.Collections;
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
        TestDatabase testDatabase;

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



        private void RefreshDropdownTests()
        {
            dropdown.ClearOptions();
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
