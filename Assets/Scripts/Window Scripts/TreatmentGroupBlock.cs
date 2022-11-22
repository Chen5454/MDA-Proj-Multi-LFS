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
    AddBlockMaster myAddBlock;

    AddBlockMaster abm;//parent addblock, the addblockmaster in which THIS is a block
    public AddBlockMaster addBlockMaster()
    {
        return abm;
    }

    public void DestroyMe()
    {
        abm.basicBlocks.Remove(this);
        Destroy(gameObject);
    }
    public void SetAddBlockMaster(AddBlockMaster addBlockMaster)
    {
        abm = addBlockMaster;
    }

    public Treatment GetTreatment()
    {
        return null;
    }

    public TreatmentGroup GetTreatmentGroup()
    {
        _treatmentGroup = new TreatmentGroup();
        _treatmentGroup.Init();
        foreach (var item in myAddBlock.basicBlocks)
        {
            _treatmentGroup.AddTreatment(item.GetTreatment() as SequenceBlock);
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

    GameObject BasicBlock.gameObject()
    {
        return gameObject;
    }
}