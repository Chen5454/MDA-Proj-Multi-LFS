using PatientCreationSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatmentGroupBlock : MonoBehaviour, BasicBlock
{
    TreatmentGroup _treatmentGroup;
    bool _isInteractable;

    Transform parent;
    //has 3 buttons that can add treatmentblocks to this parent

    [SerializeField]
    AddBlockMaster addBlockMaster;

    public Treatment GetTreatment()
    {
        return null;
    }

    public TreatmentGroup GetTreatmentGroup()
    {
        _treatmentGroup = new TreatmentGroup();
        _treatmentGroup.Init();
        foreach (var item in addBlockMaster.basicBlocks)
        {
            _treatmentGroup.AddTreatment(item as SequenceBlock);
        }
        return _treatmentGroup;
    }

    public bool IsInteractable()
    {
        return _isInteractable;
    }

    public void SetInteractable(bool isInteractable)
    {
        //all interactables - including all treatments in group - set to isInteractable
        // TBD TBF soon
        //
        _isInteractable = isInteractable;
    }

}
