using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Abstract class that describes ANY patient-interaction that has medical/treatment implications.
/// Including: Questions, Tests, and Medicine. 
/// Give injection, perform CPR, underss patient, ask patient their age)
/// </summary>
namespace PatientCreationSpace
{

    [System.Serializable]
    public abstract class Treatment : SequenceBlock
    {
        [SerializeField]
        protected string id;

        /// <summary>
        /// Only to be used by inheretors
        /// </summary>
        /// <param name="newID">TBF pull from last ID used somehow</param>
        public virtual void Set(string newID, string treatmentType)
        {
            id = newID;
            typeString = treatmentType;
        }
        //identification of question, test or device
        public virtual string ID()
        {
            return id;
        }
        /// <summary>
        /// each treatment should decide which string to expose as its name
        /// </summary>
        /// <returns></returns>
        public virtual string TreatmentDisplayNameAsPartOfDatabase()
        {
            return id;
        }

        //may be either playerDataDelta - or Answer to question
        public abstract object Result();

        public override bool WasPerformed()
        {
            return false;
        }
    }

}