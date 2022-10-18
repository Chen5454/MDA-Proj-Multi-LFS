using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu()]
namespace PatientCreationSpace
{

    [System.Serializable]
    public class Patient
    {

        public NewPatientData patientData;
        //patient ID card
        //temp id TBF - needs to be it's own struct of all ID card info
        public string id;
        [SerializeField]
        public string paitent_name;
        [SerializeField]
        public string paitent_age;
        //patientData - measurements 
        //temp data TBF - needs to be the patientMeasurement class from the project
        //public object patientData;


        //Treatment sequence!
        [SerializeField]//?
        public TreatmentSequence paitent_FullTreatmentSequence;
        public TreatmentSequence GetTreatmeantSequence { get => paitent_FullTreatmentSequence; }

        //public SequenceBlock SequenceBlock(int i) => 

        public void Init(string _id, string newName, string newAge)
        {
            id = _id;
            paitent_name = newName;
            //PatientCreator.patientID = newName;
            paitent_age = newAge;
            CreateNewTreatmentSequence();
        }
        //public void Init(string name, string sureName, int id, int age, string gender, string phoneNum, string medicalCompany, string adress, string complaint, string[] measurements)
        //{
        //    id = _id;
        //    paitent_name = newName;
        //    //PatientCreator.patientID = newName;
        //    paitent_age = newAge;
        //    CreateNewTreatmentSequence();
        //}
        private void CreateNewTreatmentSequence()
        {
            //paitent_FullTreatmentSequence = CreateInstance<TreatmeantSequence>();
            //temp

            //paitent_FullTreatmentSequence = SO_Creator<TreatmentSequence>.CreateT(id, paitent_name);
            paitent_FullTreatmentSequence = new TreatmentSequence();
            paitent_FullTreatmentSequence.Init();
        }

        //public void AddToTreatmentSequence(SequenceBlock sequenceBlock)
        //{

        //}

    }

}