using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class Entry : IEqualityComparer
    {
        public string action;
        public string playerID;
        public string patientID;
        public int teamNumber;

        public Entry(string actionName, string playerId, string patientId, int teamNumber)
        {
            action = actionName;
            playerID = playerId;
            patientID = patientId;
            this.teamNumber = teamNumber;
        }

        public new bool Equals(object x, object y)
        {
            return ((Entry)x).action == ((Entry)y).action && ((Entry)x).patientID == ((Entry)y).patientID; //this is kind of meaningless now...
        }

        public int GetHashCode(object obj)
        {
            throw new System.NotImplementedException();
        }
    }

}