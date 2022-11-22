using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatientCreationSpace;

public interface BasicBlock 
{
    GameObject gameObject();
    AddBlockMaster addBlockMaster();
    void SetAddBlockMaster(AddBlockMaster addBlockMaster);
    void DestroyMe();
    Treatment GetTreatment();
    TreatmentGroup GetTreatmentGroup();
    bool IsInteractable();
    void SetInteractable(bool isInteractable);
   
}
