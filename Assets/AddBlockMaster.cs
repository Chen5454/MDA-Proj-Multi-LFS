using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBlockMaster : MonoBehaviour
{
    [SerializeField]
    Transform sequenceParent;

    public List<BasicBlock> basicBlocks;

    private void OnEnable()
    {
        if(basicBlocks == null)
        basicBlocks = new List<BasicBlock>();
    }
    private void OnDisable()
    {
        //destory everything!
        foreach (var item in basicBlocks)
        {
            Destroy(item.gameObject());
        }
        basicBlocks.Clear();
    }
    public void AddBlockToSequence(GameObject prefab)
    {
        GameObject go = Instantiate(prefab, sequenceParent);
        BasicBlock bb = go.GetComponent<BasicBlock>();
        if (bb == null)
        {
            Debug.LogError("כל הכבוד נטע!");
        }
        bb.SetAddBlockMaster(this);
        basicBlocks.Add(bb);
        //or group block if not basic? I dont love it TBF
    }
    public void AddInstantiatedBlockToSequence(BasicBlock bb)
    {
        bb.gameObject().transform.parent = sequenceParent;
        bb.SetAddBlockMaster(this);
        basicBlocks.Add(bb);
        //or group block if not basic? I dont love it TBF
    }

}
