using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PatientCreationSpace
{

    [System.Serializable]
    public class TreatmentDB<T> where T : Treatment
    {

#if UNITY_EDITOR
        public static readonly string initialPath = "Assets/StreamingAssets/Databases/";
#else
        public static readonly string initialPath = $"{Application.streamingAssetsPath}/Databases/";
#endif

        // Databases all treatments or is good for creating seperate DB's per type
        [SerializeField]
        public List<T> treatments;
        [SerializeField]
        protected List<T> tempTreatments;
        
        public T GetTreatmentByIndex(int index) => treatments[index]; //this is virtual in case 
        public virtual List<T> GetFullList() => treatments;
        public virtual List<string> GetListOfTreatmentNames()
        {
            if (treatments == null || treatments.Count == 0)
            {
                Debug.LogError("no treatments found!");
                return null;
            }

            List<string> toReturn = new List<string>();

            foreach (Treatment item in treatments)
            {
                toReturn.Add(item.TreatmentDisplayNameAsPartOfDatabase());
            }
            return toReturn;
        }


        public List<T> GetTreatmentsWithLinq(System.Func<T, bool> pred)
        {
            return treatments.Where(pred).ToList();
        }

        public void LoadDatabase()
        {
            if (!Directory.Exists(initialPath))
            {
                Debug.LogError("No database folder found");
                return;
            }
            
            string typeString = typeof(T).ToString().Substring(21); // start after "PatientCreationSpace." (21 chars)
            if (!File.Exists($"{initialPath}{typeString}_Database.txt"))
            {
                Debug.LogError("No database file found");
                return;
            }
            string datastring = File.ReadAllText($"{initialPath}{typeString}_Database.txt");
            treatments = JsonUtility.FromJson<TreatmentDB<T>>(datastring).treatments;
        }
        public void SaveDatabase()
        {
            if (!Directory.Exists(initialPath))
            {
                Directory.CreateDirectory(initialPath);
            }

            //if(File.Exists($"{initialPath}/{GetType()}.txt"))
            //{
            //    Debug.LogError($"Overwrite {GetType()}base?");
            //    return;
            //}

            StreamWriter sw = File.CreateText($"{initialPath}/{typeof(T)}_Database.txt");

            string saveString = JsonUtility.ToJson(this);

            sw.Write(saveString);
            sw.Close();
        }

    }

}