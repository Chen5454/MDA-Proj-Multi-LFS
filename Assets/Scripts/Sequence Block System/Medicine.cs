using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class Medicine : Treatment
    {

        public string medicineName; //with or without dosage (for display purposes only)
                                    //Currently dosage is part of the unique ID, so basically 10mg of drug X


        //[SerializeField]
        //string TEMP_patientDataString; //PLACEHOLDER FOR PatientMeasurementData
        public PatientMeasurements measurements;
        public float minDosage;
        public float maxDosage;

        public int applicationMethod; //index of applicationMethod enum
        //Medicine Setter TBF (also a creator? I dont think so)
        public void Init(string medNAme, PatientMeasurements newMeasurements, float newMinDose, float newMaxDose, int appMethod) //temp! TBF //working on it
        {
            medicineName = medNAme;
            measurements = newMeasurements;
            minDosage = newMinDose;
            maxDosage = newMaxDose;
            applicationMethod = appMethod;

            Set(medNAme, GetType().ToString().Substring(21));//Ignores the initial namespace and period ("PatientCreationSpace.") - 21 chars
        }
        public override object Result()
        {
            //return patientData;
            return measurements;
        }
        public override string DisplayStringAsPartOfSequence()
        {
            return $"שם התרופה:{medicineName} \n תגובה לתרופה:{measurements}";
        }
        public override string TreatmentDisplayNameAsPartOfDatabase()
        {
            return medicineName;
        }
    }

}