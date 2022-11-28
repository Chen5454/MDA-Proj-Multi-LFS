using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddBlockMaster : MonoBehaviour
{
    [SerializeField]
    Transform sequenceParent;

    public List<BasicBlock> basicBlocks;
    [SerializeField]
    ScrollRect scrollRect;

    private void OnEnable()
    {
        if (basicBlocks == null)
            basicBlocks = new List<BasicBlock>();
        else
        {
            foreach (var item in basicBlocks)
            {
                Destroy(item.gameObject());
            }
            basicBlocks.Clear();
        }

    }
    private void OnDisable()
    {
        //destory everything!
        
        //basicBlocks.Clear();
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
        bb.gameObject().transform.SetParent(sequenceParent);
        bb.SetAddBlockMaster(this);
        basicBlocks.Add(bb);
        //or group block if not basic? I dont love it TBF
    }

}
