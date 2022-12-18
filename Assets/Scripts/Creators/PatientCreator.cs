using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;

namespace PatientCreationSpace
{

    public static class PatientCreator
    {

#if UNITY_EDITOR
        public static readonly string streamingAssets_PatientFolderPath = "Assets/StreamingAssets/Patients/";
        public static readonly string streamingAssets_ARAN_PatientFolderPath = "Assets/StreamingAssets/AranPatients/"; //Maybe there should be folders of AranPatient sets ? TBF
#else
        public static readonly string streamingAssets_PatientFolderPath = $"{Application.streamingAssetsPath}/Patients/";
        public static readonly string streamingAssets_ARAN_PatientFolderPath = $"{Application.streamingAssetsPath}/AranPatients/";//Maybe there should be folders of AranPatient sets ? TBF
#endif

        //public static string patientID => newPatient.Id;
        //public static Patient currentPatient;
        public static NewPatientData newPatient; //TBF newPatient shouldn't really be accessible like this
        public static List<NewPatientData> AranListToLoad;


        public static System.Action OnLoadPatient;
        public static System.Action OnPatientClear;


        public static void SetAranList(List<NewPatientData> aranList)
        {
            AranListToLoad = aranList;
        }


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
        public static NewPatientData CreateNewPatient(int patientType, string name, string sureName, int id, int age, int gender, string phoneNum, string medicalCompany, string complaint, string[] measurements, DestinationRoom room, bool isAls, bool trauma)
        {
            newPatient = new NewPatientData(); 

            newPatient.Initialize(patientType, name, sureName, id, age, gender, phoneNum, medicalCompany, complaint, measurements,room,  isAls, trauma);

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
            string treatmentSequenceJSON = SerializeTreatmentSequence(newPatient.FullTreatmentSequence);

            string patientFullName = newPatient.Name + "_" + newPatient.SureName;
            
            RequestTest.Instance.LogPlayer(patientFullName, patientJSON,treatmentSequenceJSON);

          //  NewPatientWindow.Instance._photonView.RPC("CallPatientCreator", RpcTarget.AllBufferedViaServer, patientJSON, treatmentSequenceJSON, newPatient.Name, newPatient.SureName);


            return true; //successful save!
        }

        
        public static void CreateSaveFiles(string patientJSON, string treatmentSequenceJSON, string name, string sureName)
        {
            if (!Directory.Exists($"{streamingAssets_PatientFolderPath}"))
            {
                Directory.CreateDirectory($"{streamingAssets_PatientFolderPath}");
            }
            StreamWriter sw = File.CreateText($"{streamingAssets_PatientFolderPath}{name}_{sureName}.txt");

            sw.Write(patientJSON);
            sw.Close();


            StreamWriter sw2 = File.CreateText($"{streamingAssets_PatientFolderPath}{name}_{sureName}_treatmentSequence.txt");
            //StreamWriter sw2 = File.CreateText($"{streamingAssets_PatientFolderPath}{}");

            sw2.Write(treatmentSequenceJSON);
            sw2.Close();
        }

        //public static void CreateSaveFiles(string patientJSON, string treatmentSequenceJSON)
        //{
        //    if (!Directory.Exists($"{streamingAssets_PatientFolderPath}"))
        //    {
        //        Directory.CreateDirectory($"{streamingAssets_PatientFolderPath}");
        //    }
        //    StreamWriter sw = File.CreateText($"{streamingAssets_PatientFolderPath}{newPatient.Name}_{newPatient.SureName}.txt");
        //
        //    sw.Write(patientJSON);
        //    sw.Close();
        //
        //
        //    StreamWriter sw2 = File.CreateText($"{streamingAssets_PatientFolderPath}{newPatient.Name}_{newPatient.SureName}_treatmentSequence.txt");
        //
        //    sw2.Write(treatmentSequenceJSON);
        //    sw2.Close();
        //}


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

            Debug.Log(toReturn);

            return toReturn;
        }

        /// <summary>
        /// path - enter only patient name, with {Name_SureName} WITHOUT .txt
        /// </summary>
        /// <param name="patientFullName"></param>
        public static NewPatientData LoadPatient(string patientFullName)
        {
            //newPatient = DeSerializePatient_Full(patientFullName);

            newPatient = RequestTest.Instance.GetFullPatientDataByName(patientFullName);

            Debug.LogError($"{newPatient.FullTreatmentSequence.sequenceBlocks.Count} sequence blocks in treatment sequence");
            OnLoadPatient?.Invoke();
            return newPatient;
        }
        /// <summary>
        /// With FullTreatmentSequence and all
        /// </summary>
        /// <param name="patientFullName"></param>
        /// <returns></returns>
        static NewPatientData DeSerializePatient_Full(string patientFullName)
        {
            string json = File.ReadAllText($"{streamingAssets_PatientFolderPath}{patientFullName}.txt");
            NewPatientData newPatientData = JsonUtility.FromJson<NewPatientData>(json);

            string ts_json = File.ReadAllText($"{streamingAssets_PatientFolderPath}{patientFullName}_treatmentSequence.txt");
            TreatmentSequence ts = DeSerializeTreatmentSequence(ts_json);


            newPatientData.FullTreatmentSequence = ts;
            return newPatientData;
        }
        /// <summary>
        /// Without TreatmentSeuqence
        /// </summary>
        /// <param name="patientFullName"></param>
        /// <returns></returns>
        /// 
        static NewPatientData DeSerializePatient_Full(string json,string ts_json)
        {
          //  string json = File.ReadAllText($"{streamingAssets_PatientFolderPath}{patientFullName}.txt");
            NewPatientData newPatientData = JsonUtility.FromJson<NewPatientData>(json);

           // string ts_json = File.ReadAllText($"{streamingAssets_PatientFolderPath}{patientFullName}_treatmentSequence.txt");
            TreatmentSequence ts = DeSerializeTreatmentSequence(ts_json);


            newPatientData.FullTreatmentSequence = ts;
            return newPatientData;
        }
        static NewPatientData DeSerializePatient_Simple(string patientFullName)
        { 
            string json = File.ReadAllText($"{streamingAssets_PatientFolderPath}{patientFullName}.txt");
          // string json = RequestTest.Instance.GetRows(patientFullName);
            NewPatientData newPatientData = JsonUtility.FromJson<NewPatientData>(json);
            return newPatientData;
        }

        static TreatmentSequence DeSerializeTreatmentSequence(string serializedTreatmentSequence)
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

        //public static List<string> GetExistingPatientNames()
        //{
        //    List<string> toReturn = new List<string>();

        //    //RequestTest.Instance.GetRow


        //    if (!Directory.Exists(streamingAssets_PatientFolderPath))
        //    {
        //        Debug.LogError("Patient folder not found!");
        //        return null;
        //    }
        //    var collection = Directory.GetFiles(streamingAssets_PatientFolderPath, "*.txt");
        //    toReturn = collection.Where(x => !x.Contains("treatmentSequence")).ToList();
        //    for (int i = 0; i < toReturn.Count; i++)
        //    {
        //        toReturn[i] = Path.GetFileName(toReturn[i]);
        //        toReturn[i] = toReturn[i].Substring(0, toReturn[i].Length - 4); //removes ".txt"
        //    }

        //    return toReturn;
        //}
        public static List<string> GetExistingPatientNames(bool isAls)
        {
            List<string> toFilter = new List<string>();
            List<string> toReturn = new List<string>();

            if (!Directory.Exists(streamingAssets_PatientFolderPath))
            {
                Debug.LogError("Patient folder not found!");
                return null;
            }
            var collection = Directory.GetFiles(streamingAssets_PatientFolderPath, "*.txt");
            toFilter = collection.Where(x => !x.Contains("treatmentSequence")).ToList();
            for (int i = 0; i < toFilter.Count; i++)
            {
                string s = Path.GetFileName(toFilter[i]);
                s = s.Substring(0, s.Length - 4); //removes ".txt"
                NewPatientData temp = DeSerializePatient_Simple(s);
                if(temp.isALS == isAls)
                toReturn.Add(s); //removes ".txt"
            }

            return toReturn;
        }
        public static List<string> GetExistingPatientNames(System.Func<NewPatientData, bool> pred)
        {
            List<NewPatientData> toFilter = new List<NewPatientData>();
            List<string> toReturn = new List<string>();


            toFilter = RequestTest.Instance.GetAllPatients_Simple();
            foreach(var temp in toFilter)
            {
                if (pred(temp)) 
                    toReturn.Add($"{temp.Name}_{temp.SureName}"); //TBF IMMEDIATELY!
            }

            return toReturn;
        }

        ///// <summary>
        ///// Returns Patient Names (as filenames to be loaded) - filtering over a list of the NewPatientData loaded from those patients.
        ///// THIS METHOD DOES *NOT* LOAD TREATMENT_SEQUENCE into the NewPatientData.
        ///// 
        ///// To filter on a list of patients WITH treatment sequence, use GetExistingPatientNamesByTreatmentSequencePredicate()
        ///// </summary>
        ///// <param name="pred"></param>
        ///// <returns></returns>
        //public static List<string> GetExistingPatientNames(System.Func<NewPatientData, bool> pred)
        //{
        //    List<string> toReturn = new List<string>();
        //    List<NewPatientData> tempList = new List<NewPatientData>();

        //    if (!Directory.Exists(streamingAssets_PatientFolderPath))
        //    {
        //        Debug.LogError("Patient folder not found!");
        //        return null;
        //    }
        //    var collection = Directory.GetFiles(streamingAssets_PatientFolderPath, "*.txt");
        //    toReturn = collection.Where(x => !x.Contains("treatmentSequence")).ToList();
        //    for (int i = 0; i < toReturn.Count; i++)
        //    {
        //        string s = Path.GetFileName(toReturn[i]);
        //        s = s.Substring(0, s.Length - 4); //removes ".txt"
        //        NewPatientData temp = DeSerializePatient_Simple(s);
        //        //if(temp.isALS == )
        //        tempList.Add(temp);
        //    }
        //    tempList = tempList.Where(pred).ToList();



        //    return toReturn;
        //}
        ///// <summary>
        ///// MORE EXPENSIVE! but can iterate of NewPatientData WITH FullTreatmentSequence
        ///// </summary>
        ///// <param name="pred"></param>
        ///// <returns></returns>
        //public static List<string> GetExistingPatientNamesByTreatmentSequencePredicate(System.Func<NewPatientData, bool> pred)
        //{
        //    List<string> toReturn = new List<string>();

        //    if (!Directory.Exists(streamingAssets_PatientFolderPath))
        //    {
        //        Debug.LogError("Patient folder not found!");
        //        return null;
        //    }
        //    var collection = Directory.GetFiles(streamingAssets_PatientFolderPath, "*.txt");
        //    toReturn = collection.Where(x => !x.Contains("treatmentSequence")).ToList();
        //    for (int i = 0; i < toReturn.Count; i++)
        //    {
        //        toReturn[i] = Path.GetFileName(toReturn[i]);
        //        toReturn[i] = toReturn[i].Substring(0, toReturn[i].Length - 4); //removes ".txt"
        //    }

        //    return toReturn;
        //}

    }

}