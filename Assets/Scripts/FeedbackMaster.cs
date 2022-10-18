using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public class FeedbackMaster : MonoBehaviour
    {
        //MONO-Singleton
        public static FeedbackMaster Instance;

        public Dictionary<string, List<Entry>> allPerformedActionsPerPatient;
        //public List<Entry> allRequiredActions;

        [SerializeField]
        Dictionary<string, AnswerSheet> answerSheets;



        List<PatientReport> patientReports;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            allPerformedActionsPerPatient = new Dictionary<string, List<Entry>>();
            answerSheets = new Dictionary<string, AnswerSheet>();
            patientReports = new List<PatientReport>();
            //allRequiredActions = new List<Entry>(); //load all required actions from requirements. either from patients or from some LevelInfo class

            //if(answerSheets==null || answerSheets.Count ==0) 
            //{
            //    answerSheets = FindObjectsOfType<AnswerSheet>().ToList();
            //}
        }

        //Recieve player action (Bdikot)
        public void AddEntry(string actionName, string playerID, string patientID, int teamNumber)
        {
            if (!allPerformedActionsPerPatient.ContainsKey(patientID))
            {
                allPerformedActionsPerPatient.Add(patientID, new List<Entry>());
            }
            allPerformedActionsPerPatient[patientID].Add(new Entry(actionName, playerID, patientID, teamNumber));
        }

        [ContextMenu("AddEntryB")]
        public void AddEntryB()
        {
            //parse log
            int teamNumber = 0; //parse from log
            string playerid = "someone"; //parse from log
            string patientid = "B"; //parse from log
            string actionName = "2";
            AddEntry(actionName, playerid, patientid, teamNumber);
            teamNumber = 0; //parse from log
            playerid = "someone"; //parse from log
            patientid = "B"; //parse from log
            actionName = "3";
            AddEntry(actionName, playerid, patientid, teamNumber);
            teamNumber = 0; //parse from log
            playerid = "someone"; //parse from log
            patientid = "A"; //parse from log
            actionName = "2";
            AddEntry(actionName, playerid, patientid, teamNumber);
            teamNumber = 0; //parse from log
            playerid = "someone"; //parse from log
            patientid = "A"; //parse from log
            actionName = "3";
            AddEntry(actionName, playerid, patientid, teamNumber);
            teamNumber = 0; //parse from log
            playerid = "someone"; //parse from log
            patientid = "A"; //parse from log
            actionName = "1";
            AddEntry(actionName, playerid, patientid, teamNumber);
            teamNumber = 0; //parse from log
            playerid = "someone"; //parse from log
            patientid = "D"; //parse from log
            actionName = "1";
            AddEntry(actionName, playerid, patientid, teamNumber);
            teamNumber = 0; //parse from log
            playerid = "someone"; //parse from log
            patientid = "C"; //parse from log
            actionName = "5";
            AddEntry(actionName, playerid, patientid, teamNumber);

        }
        [ContextMenu("SCORE")]

        public void CalculateScore()
        {
            //create sub-lists:
            List<string> treatedPatients = allPerformedActionsPerPatient.Keys.ToList();

            if (treatedPatients.Count() < answerSheets.Keys.Count())
            {
                List<string> s = answerSheets.Keys.Except(treatedPatients).ToList();
                print($"Not all patients treated");
                foreach (var item in s)
                {
                    print($"missing patient:{item}");
                }
            }

            foreach (var patientEntryList_pair in allPerformedActionsPerPatient)
            {
                AnswerSheet answer = answerSheets[patientEntryList_pair.Key];
                List<Entry> entries = patientEntryList_pair.Value;

                //int correctTotal = answer.actions.Count;
                //int runningTotal = 0;

                List<string> performedActions = new List<string>();

                foreach (var entry in entries)
                {
                    performedActions.Add(entry.action);
                }

                List<string> missingRequiredActions = answer.correctActions.Except(performedActions).ToList();
                PatientReport pr = new PatientReport();
                pr.answerSheet = answer;
                pr.treatingTeamNumber = entries[0].teamNumber; //assuming they're all the same TBF
                pr.performedActions = performedActions;
                patientReports.Add(pr);

                if (missingRequiredActions.Count == 0)
                {
                    print($"Patient {patientEntryList_pair.Key} was treated correctly");

                }
                else
                {
                    print($"Patient {patientEntryList_pair.Key} will be suing for malpractice. \n Missing Treatments and Anamnesia:");
                    foreach (var item in missingRequiredActions)
                    {
                        print($"{item} not performed on {patientEntryList_pair.Key}");
                    }
                }
            }
            DisplayPatientReports();
            ////compare required and performed
            //foreach (var item in answerSheets)
            //{
            //    int correctTotal = item.actions.Count;
            //    int runningTotal = 0;
            //    foreach (var action in item.actions)
            //    {
            //        if (allPerformedActionsPerPatient[item.patientID].Where(x => x.action == action).Count() > 0)
            //        {
            //            runningTotal++;
            //        }
            //    }

            //    if (runningTotal == correctTotal)
            //    {
            //        print("corrent total!");
            //    }
            //    else
            //    { 
            //        print("incorrent total!");

            //    }
            //}
        }
        [ContextMenu("Report")]
        public void DisplayPatientReports()
        {
            if (patientReports == null || patientReports.Count == 0)
            {
                Debug.LogError("no patient reports");
                return;
            }

            foreach (var patient in patientReports)
            {
                //print 
                string allActions = "";
                foreach (var item in patient.answerSheet.correctActions)
                {
                    string mark = patient.performedActions.Contains(item) ? "V" : "X";
                    allActions = $"{allActions}\n {item} {mark}";
                }
                print($"{patient.answerSheet.patientID} - {allActions}");
            }
        }
        public void AddPatient(AnswerSheet answerSheet)
        {
            answerSheets.Add(answerSheet.patientID, answerSheet);
        }
    }

}