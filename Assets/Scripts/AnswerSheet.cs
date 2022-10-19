using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CT and Maternity are the same for ALS and BLS (as in documentation)
/// </summary>
/// 
namespace PatientCreationSpace
{

    public enum DestinationRoom { CT, Maternity, Bypass_ALS, Bypass_BLS };
    public class AnswerSheet : MonoBehaviour
    {
        //held on patients - describes the actions required by the exam
        public string patientID;
        public List<string> correctActions;
        public TreatmentSequence treatmeantSequence;
        public DestinationRoom destinationRoom;

        private void Start()
        {
            FeedbackMaster.Instance.AddPatient(this);
        }


        void SetActions()
        {

            //TAKE THESE FROM THE SCRIPTABLE OBJECTS!!!
            switch (destinationRoom)
            {
                case DestinationRoom.CT:
                    correctActions.Add("age");
                    correctActions.Add("measure_golcuse");
                    correctActions.Add("measure_bloodPressure");
                    correctActions.Add("check_pupils"); //drop down of optional elements - does it already exist in the TagMeeyoon?
                                                        //previous medical events (enums) -> follow up (bool), if yes, any limitations persist?
                                                        //conciouse state
                                                        //etc...
                    break;
                case DestinationRoom.Maternity:
                    break;
                case DestinationRoom.Bypass_ALS:
                    break;
                case DestinationRoom.Bypass_BLS:
                    break;
                default:
                    break;
            }

        }
    }

}