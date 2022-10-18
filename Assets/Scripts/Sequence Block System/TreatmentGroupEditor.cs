using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public class TreatmentGroupEditor : BlockCollectionEditor
    {
        NewPatientData newPatient;

        public TreatmentGroup treatmentGroup { get => (TreatmentGroup)blockCollection; set => blockCollection = value; }

        [SerializeField]
        TreatmentSequenceEditorWindow treatmentSequenceEditorWindow;
        [SerializeField]
        BlockCollectionDisplayer sequenceDisplayer; //also works for treatmentGroup

        public System.Action OnSequenceChange;
        //public void Init(Patient p)
        //{
        //    newPatient = p;
        //    //treatmentGroup = SO_Creator<TreatmentGroup>.CreateT(p.paitent_name, $"{PatientCreator.patientID}/TreatmentGroups/");
        //    treatmentGroup = new TreatmentGroup();
        //    treatmentGroup.Init();

        //    treatmentGroup.OnSequenceChange += sequenceDisplayer.Display;

        //    //sequenceDisplayer.Set(treatmentGroup as IBlockCollection);
        //    sequenceDisplayer.Set(this);
        //}
        public void Init(NewPatientData p)
        {
            newPatient = p;
            //treatmentGroup = SO_Creator<TreatmentGroup>.CreateT(p.paitent_name, $"{PatientCreator.patientID}/TreatmentGroups/");
            treatmentGroup = new TreatmentGroup();
            treatmentGroup.Init();

            treatmentGroup.OnSequenceChange += sequenceDisplayer.Display;

            //sequenceDisplayer.Set(treatmentGroup as IBlockCollection);
            sequenceDisplayer.Set(this);
        }

        public override void AddTreatmentToCollection(SequenceBlock sequenceBlock)
        {
            if (newPatient == null)
            {
                Debug.LogError("missing patient");
                return;
            }
            treatmentGroup.AddTreatment(sequenceBlock as Treatment);
        }
        public override void RemoveTreatment(int index)
        {
            treatmentGroup.RemoveTreatment(index);
        }
        public void RefreshDisplay()
        {
            sequenceDisplayer.Display();
        }

        public void AddTreatmentGroupToSequence()
        {
            treatmentSequenceEditorWindow.AddTreatmentToCollection(treatmentGroup);

            gameObject.SetActive(false);
        }
    }

}