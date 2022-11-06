using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatientCreationSpace
{

    //public enum DestinationRoom { CT, Maternity, Bypass_ALS, Bypass_BLS };

    /// <summary>
    /// Basically the record of treatment.
    /// This functions as BOTH data and controller.
    /// Basically via methods in this script, treatments will attempt to effect the patient.
    /// See "AttemptTreatment()" summary for more
    /// </summary>
    public class AnswerSheet : MonoBehaviour
    {
        public NewPatientData data;
        //public string patientID => data.Id;
        int currentBlockIndex;

        public TreatmentSequence treatmeantSequence => data.FullTreatmentSequence;

        public void Set(NewPatientData newPatientData)
        {
            data = newPatientData;
        }
        
        public void AttemptTreatment(Treatment treatment) //this is probably not going to work without a more specific type...
                                                          //but, with unique ID's this could rely on the Treatment.Result() if the ID mathces a relevant step
                                                          //Regardless of Treatment type
        {
            if(currentBlockIndex >= treatmeantSequence.sequenceBlocks.Count)
            {
                Debug.LogError("current blockindex is larger than the amount of blocks in the treatmentsequence");
                return;
            }
            //THINK OF A SMART WAY TO CHECK IF block.Containts(Treatment) - to bypass checking for treatments in the groups?

            if(treatmeantSequence.sequenceBlocks[currentBlockIndex].)

        }
    }

}