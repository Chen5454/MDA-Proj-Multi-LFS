using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatientCreationSpace
{
   

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
        int currentInnerGroupIndex;
        TreatmentGroup completedGroupSteps; //holds steps already performed in this group - making sure no doubles of the same action are counted as two different treatments

        public TreatmentSequence treatmeantSequence => data.FullTreatmentSequence;

        public void Set(NewPatientData newPatientData)
        {
            data = newPatientData;
            currentBlockIndex = 0;
            currentInnerGroupIndex = 0;
        }
        
        public void AttemptTreatment(Treatment treatment) //this is probably not going to work without a more specific type...
                                                          //but, with unique ID's this could rely on the Treatment.Result() if the ID mathces a relevant step
                                                          //Regardless of Treatment type
        {
            if(currentBlockIndex >= treatmeantSequence.sequenceBlocks.Count)
            {
                Debug.LogError("DONE!");
                return;
            }
            //THINK OF A SMART WAY TO CHECK IF block.Containts(Treatment) - to bypass checking for treatments in the groups?
            //do dumb way now: TBF

            if(treatmeantSequence.sequenceBlocks[currentBlockIndex] is TreatmentGroup)
            {
                TreatmentGroup tg = treatmeantSequence.sequenceBlocks[currentBlockIndex] as TreatmentGroup;

                if (completedGroupSteps == null) //True if this is the first time this group step has been accessed
                {
                    completedGroupSteps = new TreatmentGroup();
                    completedGroupSteps.Init();
                    currentInnerGroupIndex = 0;
                }

                if (!completedGroupSteps.SequenceBlocks().Contains(treatment) && tg.SequenceBlocks().Contains(treatment as SequenceBlock)) //True if the attempted treatment is contained within the group (Comparer would greatly help! TBF)
                {
                    ResolveTreatment(treatment);

                    completedGroupSteps.AddTreatment(treatment);
                    currentInnerGroupIndex++;

                    if(currentInnerGroupIndex >= tg.SequenceBlocks().Count) //True if this treatmentgroup is completed
                    {
                        currentInnerGroupIndex = 0;
                        completedGroupSteps = null; //important!
                        currentBlockIndex++; //Advance to the next Block!
                    }
                }
            }
            else
            {
                if (treatment == treatmeantSequence.sequenceBlocks[currentBlockIndex])
                {
                    ResolveTreatment(treatment);
                    currentBlockIndex++; //Advance to the next Block!
                }
            }
        }
        void ResolveTreatment(Treatment treatment)
        {
            //treatment.Result();
            //basically another if-switch on treatment to handle per-type

            if(treatment is Question)
            {
                UIManager.Instance.QuestionPanel.RecieveAnswer((treatment as Question).answerText);
            }
            else if (treatment is Medicine)
            {
                //newPatientData.ApplyMedicine((treatment as Medicine).measurements);
            }
            else if (treatment is Test)
            {
                //MeasuremnetPanel.SetMeasurement((treatment as Test).measurementType); // tests hold no value, they merely tell MeasurementsPanel to refresh 1 measuremnets displayer
            }

        }
    }

}