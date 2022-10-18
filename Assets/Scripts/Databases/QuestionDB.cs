using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public class QuestionDB : TreatmentDB<Question>
    {
        [ContextMenu("test")]
        public void Test()
        {
            QuestionContaining("a");
        }
        public void QuestionsThatStartsWith(string partialTyped)
        {

            tempTreatments = GetTreatmentsWithLinq(x => x.ID().StartsWith(partialTyped));
        }
        public void QuestionContaining(string contained)
        {
            tempTreatments = GetTreatmentsWithLinq(x => x.ID().Contains(contained));
        }
        //bool DoesAStartWithB(string A, string B)
        //{
        //    A.StartsWith()
        //}
    }

}