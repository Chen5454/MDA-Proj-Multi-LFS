using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public class Databases : MonoBehaviour
    {
        public static Databases Instance;


        public TreatmentDB<Question> questionDB;
        public TreatmentDB<Test> testDB;
        public TreatmentDB<Medicine> medicineDB;

        public List<Question> temp_questions;
        public List<Test> temp_tests;
        public List<Medicine> temp_meds;

        private void Awake()
        {
            if(Instance != null && Instance!=this)
            {
                Destroy(gameObject);
                Debug.LogError("Kill the spare! databases");
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            LoadAllDBs();
        }



#if UNITY_EDITOR
        [ContextMenu("Save All DBs")]
        public void SaveAllDBs()
        {
            questionDB = new TreatmentDB<Question>();
            questionDB.treatments = temp_questions;
            questionDB.SaveDatabase();
            testDB = new TreatmentDB<Test>();
            testDB.treatments = temp_tests;
            testDB.SaveDatabase();
            medicineDB = new TreatmentDB<Medicine>();
            medicineDB.treatments = temp_meds;
            medicineDB.SaveDatabase();
            //medicineDB.SaveDatabase();
        }
#endif

        [ContextMenu("Load All DBs")]
        public void LoadAllDBs()
        {
            questionDB = new TreatmentDB<Question>();
            testDB = new TreatmentDB<Test>();
            medicineDB = new TreatmentDB<Medicine>();



            questionDB.LoadDatabase();
            testDB.LoadDatabase();
            medicineDB.LoadDatabase();
            temp_questions = questionDB.treatments;
            temp_tests = testDB.treatments;
            temp_meds = medicineDB.treatments;
        }

    }

}