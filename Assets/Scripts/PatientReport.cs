using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class PatientReport
    {
        public int treatingTeamNumber;
        public AnswerSheet answerSheet;
        public List<string> performedActions;
        //public List<string> missingActions;
        [Tooltip("Time in seconds needed to get full points.")]
        public float prefectTime;
        [Tooltip("Time in seconds over which no points will be awarded for time.")]
        public float passableTime;
        public float timePerformed;
    }

}