using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public class BlockCollectionDisplayer : MonoBehaviour //Shows both TreatmentGroups and Sequences
    {
        //TreatmentSequence treatmeantSequence;
        IBlockCollection treatmeantSequence => collectionEditor.blockCollection; //shouldnt be able to set()
        BlockCollectionEditor collectionEditor;
        //List<SequenceBlock> sequenceBlocks;
        //TBF - make these SequenceBlockDisplayer instead of just one text box with several lines or several textBoxes
        [SerializeField]
        TMPro.TMP_Text textBox;

        [SerializeField]
        Transform displayerParent;

        [SerializeField]
        GameObject blockDisplayerPrefab;

        List<BlockDisplayer> blockDisplayers;

        //Deprecated!
        //public void Set(IBlockCollection blockCollection)
        //{
        //    blockDisplayers = new List<BlockDisplayer>();
        //    treatmeantSequence = blockCollection;
        //    //sequenceBlocks = blockCollection.SequenceBlocks();
        //    blockCollection.SubToOnListChanged(Display);
        //    //blockCollection.OnListChanged() += Display;

        //}
        public void Set(BlockCollectionEditor blockCollectionEditor)
        {
            if (blockDisplayers != null && blockDisplayers.Count > 0)
            {
                for (int i = 0; i < blockDisplayers.Count; i++)
                {
                    Destroy(blockDisplayers[i].gameObject);
                }
            }
            blockDisplayers = new List<BlockDisplayer>();
            //treatmeantSequence = blockCollectionEditor.blockCollection;
            collectionEditor = blockCollectionEditor;
            treatmeantSequence.SubToOnListChanged(Display);
            Display();
        }

        public void Display()
        {
            //textBox.text = treatmeantSequence.AllDisplayStrings(); //this should go over members and reuse/instantiate a displayer for them?
            //textBox.text = treatmeantSequence.AllDisplayStrings(); //this should go over members and reuse/instantiate a displayer for them?

            List<string> strings = treatmeantSequence.ListAllDisplayStrings();
            if (strings == null || strings.Count == 0)
            {
                foreach (BlockDisplayer bd in blockDisplayers)
                {
                    Destroy(bd.gameObject);
                }
                blockDisplayers.Clear();
                return;
            }
            for (int i = 0; i < strings.Count; i++)
            {
                if (i >= blockDisplayers.Count)
                {
                    BlockDisplayer newBlock = Instantiate(blockDisplayerPrefab, displayerParent).GetComponent<BlockDisplayer>();
                    blockDisplayers.Add(newBlock);
                    newBlock.SetMe(strings[i], i, collectionEditor);
                }
                else
                {
                    blockDisplayers[i].SetMe(strings[i], i, collectionEditor);
                }
            }
            if (blockDisplayers.Count > strings.Count)
            {
                for (int i = blockDisplayers.Count - 1; i >= strings.Count; i--)
                {
                    Destroy(blockDisplayers[i].gameObject);
                    blockDisplayers.RemoveAt(i);
                }
            }
        }
    }

}