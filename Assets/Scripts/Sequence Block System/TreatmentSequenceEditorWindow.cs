using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public class TreatmentSequenceEditorWindow : BlockCollectionEditor
    {
        //Patient newPatient;
        NewPatientData newPatient;

        public TreatmentSequence NewTreatmeantSequence { get => (TreatmentSequence)blockCollection; set => blockCollection = value; }
        //public TreatmentSequence NewTreatmeantSequence;

        [SerializeField]
        TreatmentGroupEditor treatmentGroupEditor;
        [SerializeField]
        BlockCollectionDisplayer sequenceDisplayer;


        //public void Init(Patient patient)
        //{
        //    newPatient = patient;
        //    newTreatmeantSequence = newPatient.GetTreatmeantSequence;
        //    //sequenceDisplayer.Set(newTreatmeantSequence as IBlockCollection);
        //    sequenceDisplayer.Set(this);
        //}
        public void Init(NewPatientData patient)
        {
            newPatient = patient;
            NewTreatmeantSequence = newPatient.FullTreatmentSequence;
            //sequenceDisplayer.Set(newTreatmeantSequence as IBlockCollection);
            sequenceDisplayer.Set(this);
        }
        [ContextMenu("Refresh")]
        public void RefreshDisplay()
        {
            //sequenceTextBox.text = newPatient.GetTreatmeantSequence.AllDisplayStrings();
            sequenceDisplayer.Display();
        }

        //BASE logic for all ADD TREATMENT types
        public override void AddTreatmentToCollection(SequenceBlock sequenceBlock)
        {
            if (newPatient == null || newPatient.FullTreatmentSequence == null)
            {
                Debug.LogError("missing patient or treatmentsequence");
                return;
            }
            NewTreatmeantSequence.AddToSequence(sequenceBlock);
        }
        public override void RemoveTreatment(int index)
        {
            NewTreatmeantSequence.RemoveFromSequence(index);
        }

        public void OpenTreatmentGroupEditor()
        {
            treatmentGroupEditor.gameObject.SetActive(true);
            treatmentGroupEditor.Init(newPatient);
        }
    }

}