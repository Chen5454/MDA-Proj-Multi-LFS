using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public static class MedicineCreator
    {
        public static Medicine CreateMedicine(string newID, string medicineName, PatientMeasurements newPatientMeasurements, float minDosage, float maxDosage, int appMethod) //don't really need to ID a patients specific medicineSO's
        {
            //Medicine med = SO_Creator<Medicine>.CreateT(medicineName, $"{PatientCreator.patientID}/Medicines/");
            Medicine med = new Medicine();

            med.Init(medicineName, newPatientMeasurements, minDosage, maxDosage, appMethod);
            return med;
        }
        //public static Medicine CreateMedicine(Medicine medicineTemplate) //don't really need to ID a patients specific medicineSO's
        //{
        //    Medicine med = SO_Creator<Medicine>.CreateT(medicineTemplate.ID(), $"{PatientCreator.patientID}/Medicines/");

        //    med.Init(medicineID, newPatientData);
        //    return med;
        //}

    }

}