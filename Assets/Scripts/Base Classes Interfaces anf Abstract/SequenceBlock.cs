using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The unit for TreatmentSequence.
/// Each Block must be performed sequentially (one after the other).
/// A Block may represnt a single Treatment, or TreatmentGroup
/// Single Treatment blocks make for are stricter sequence. If a procedure requires clear steps one after another - only SingleTreatment blocks will be used.
/// But some treatments are not sequentially consequential - and can happen with no regard to order. Those treatments will be a one TreatmentGroup.
/// </summary>
namespace PatientCreationSpace
{

    [System.Serializable]
    public class SequenceBlock
    {
        //Treatment blocks need:
        //bool isComplete;
        public string typeString;
        public virtual string DisplayStringAsPartOfSequence()
        {
            return "impossible";
        }

        public virtual bool WasPerformed()
        {
            return true;
        }
    }

}