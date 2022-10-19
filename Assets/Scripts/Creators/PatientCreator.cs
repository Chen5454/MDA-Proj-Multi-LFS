using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace PatientCreationSpace
{

    public static class PatientCreator
    {
        public static readonly string scriptableObjects_FolderPath = "Assets/StreamingAssets/Patients/";
        //public static string patientID => newPatient.Id;
        //public static Patient currentPatient;
        public static NewPatientData newPatient;


        public static System.Action OnLoadPatient;
        public static System.Action OnPatientClear;


        /// <summary>
        /// Returns true if a loaded patient was cleared.
        /// False if there was no loaded patient to clear.
        /// </summary>
        /// <returns></returns>
        public static bool ClearLoadedPatient()
        {
            bool wasCleared = (newPatient != null);
            newPatient = null;
            OnPatientClear?.Invoke();
            return wasCleared;
        }
        public static NewPatientData CreateNewPatient(string name, string sureName, int id, int age, string gender, string phoneNum, string medicalCompany, string adress, string complaint, string[] measurements)
        {
            newPatient = new NewPatientData();

            newPatient.Initialize(name, sureName, id, age, gender, phoneNum, medicalCompany, adress, complaint, measurements);

            //create file already?


            return newPatient;
        }

       
        public static bool SaveNewPatient()
        {
            if (newPatient == null)
            {
                Debug.LogError("no new patient!");
                return false;
            }

            string patientJSON = JsonUtility.ToJson(newPatient);
            if (!Directory.Exists($"{scriptableObjects_FolderPath}"))
            {
                Directory.CreateDirectory($"{scriptableObjects_FolderPath}");
            }
            StreamWriter sw = File.CreateText($"{scriptableObjects_FolderPath}{newPatient.Name}_{newPatient.SureName}.txt");

            sw.Write(patientJSON);
            sw.Close();

            string treatmentSequence = SerializeTreatmentSequence(newPatient.FullTreatmentSequence);

            StreamWriter sw2 = File.CreateText($"{scriptableObjects_FolderPath}{newPatient.Name}_{newPatient.SureName}_treatmentSequence.txt");

            sw2.Write(treatmentSequence);
            sw2.Close();


            return true; //successful save!
        }


        public static string SerializeTreatmentSequence(TreatmentSequence treatmentSequence)
        {
            string toReturn = "";
            foreach (var block in treatmentSequence.sequenceBlocks)
            {
                switch (block.typeString)
                {
                    //case "Question":
                    default:
                        toReturn += $"{block.typeString}_{JsonUtility.ToJson(block)}\n";
                        break;
                    case "TreatmentGroup":
                        TreatmentGroup tg = (TreatmentGroup)block;
                        toReturn += "Start_TreatmentGroup\n";

                        foreach (var inner_block in tg.SequenceBlocks())
                        {
                            toReturn += $"{inner_block.typeString}_{JsonUtility.ToJson(inner_block)}\n";
                        }
                        toReturn += "End_TreatmentGroup\n";
                        break;
                }
            }
            //TreatmentSequence tsTest = DeSerializeTreatmentSequence(toReturn);

            //Debug.Log(tsTest.AllDisplayStrings());

            return toReturn;
        }

        /// <summary>
        /// path - enter only patient name, with {Name_SureName} WITHOUT .txt
        /// </summary>
        /// <param name="patientFullName"></param>
        public static void LoadPatient(string patientFullName)
        {

            string json = File.ReadAllText($"{scriptableObjects_FolderPath}{patientFullName}.txt");
            NewPatientData newPatientData = JsonUtility.FromJson<NewPatientData>(json);

            string ts_json = File.ReadAllText($"{scriptableObjects_FolderPath}{patientFullName}_treatmentSequence.txt");
            TreatmentSequence ts = DeSerializeTreatmentSequence(ts_json);


            newPatientData.FullTreatmentSequence = ts;

            newPatient = newPatientData;
            OnLoadPatient?.Invoke();
        }

        public static TreatmentSequence DeSerializeTreatmentSequence(string serializedTreatmentSequence)
        {
            TreatmentSequence toReturn = new TreatmentSequence();
            toReturn.Init();
            TreatmentGroup tempGroup = null;

            string[] lines = serializedTreatmentSequence.Split('\n');
            foreach (var line in lines)
            {
                string[] fields = line.Split('_');
                List<string> data = fields.ToList();
                data.Remove(fields[0]);
                string datastring = string.Concat(data);
                if (tempGroup != null)
                {
                    switch (fields[0])
                    {
                        case "Question":
                            Question q = JsonUtility.FromJson<Question>(datastring);
                            tempGroup.AddTreatment(q);
                            break;
                        case "Test":
                            Test t = JsonUtility.FromJson<Test>(datastring);
                            tempGroup.AddTreatment(t);
                            break;
                        case "Medicine":
                            Medicine m = JsonUtility.FromJson<Medicine>(datastring);
                            tempGroup.AddTreatment(m);
                            break;

                        case "End":
                            tempGroup = null;
                            break;

                        default:
                            break;
                    }
                }
                else
                {

                    switch (fields[0])
                    {
                        case "Question":
                            Question q = JsonUtility.FromJson<Question>(datastring);
                            toReturn.AddToSequence(q);
                            break;
                        case "Test":
                            Test t = JsonUtility.FromJson<Test>(datastring);
                            toReturn.AddToSequence(t);
                            break;
                        case "Medicine":
                            Medicine m = JsonUtility.FromJson<Medicine>(datastring);
                            toReturn.AddToSequence(m);
                            break;
                        case "Start":
                            tempGroup = new TreatmentGroup();
                            tempGroup.Init();
                            toReturn.AddToSequence(tempGroup);
                            break;

                        default:
                            break;
                    }
                }
            }
            return toReturn;
        }

        public static List<string> GetExistingPatientNames()
        {
            List<string> toReturn = new List<string>();

            if (!Directory.Exists(scriptableObjects_FolderPath))
            {
                Debug.LogError("Patient folder not found!");
                return null;
            }
            var collection = Directory.GetFiles(scriptableObjects_FolderPath, "*.txt");
            toReturn = collection.Where(x => !x.Contains("treatmentSequence")).ToList();
            for (int i = 0; i < toReturn.Count; i++)
            {
                toReturn[i] = Path.GetFileName(toReturn[i]);
                toReturn[i] = toReturn[i].Substring(0, toReturn[i].Length - 4); //removes ".txt"
            }

            return toReturn;
        }
        
    }

}