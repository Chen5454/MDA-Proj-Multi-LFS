using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class Test : Treatment
    {
        //public object patientDataDelta; //relevant information to display?
        [SerializeField]
        public string testName;

        public Measurements Measurement;

        //TBF test setter?

        public void Init(string newID, string treatmentType, string newTestName)
        {
            testName = newTestName;
            Set(newID, treatmentType);
        }
        public override void Set(string newID, string treatmentType)
        {
            base.Set(newID, treatmentType);
        }
        public override object Result()
        {
            return "";
        }
        public override string DisplayStringAsPartOfSequence()
        {
            return $"בדיקה: {testName}";
        }
        public override string TreatmentDisplayNameAsPartOfDatabase()
        {
            return testName;
        }
    }

}