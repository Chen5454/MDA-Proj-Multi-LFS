using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;

public interface BasicBlock 
{
    Treatment GetTreatment();
    TreatmentGroup GetTreatmentGroup();
    bool IsInteractable();
    void SetInteractable(bool isInteractable);
   
}
