using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class TreatmentSequence : IBlockCollection
    {
        public List<SequenceBlock> sequenceBlocks;

        public System.Action OnSequenceChange;



        public void Init()
        {
            sequenceBlocks = new List<SequenceBlock>();
        }

        //sequence changing methods
        /// <summary>
        /// adds block to end of list
        /// </summary>
        /// <param name="sequenceBlock"></param>
        public void AddToSequence(SequenceBlock sequenceBlock)
        {
            sequenceBlocks.Add(sequenceBlock);
            OnSequenceChange?.Invoke();
        }
        /// <summary>
        /// add block at index and push rest down
        /// </summary>
        /// <param name="index"></param>
        public void AddToSequence(SequenceBlock sequenceBlock, int index)
        {
            sequenceBlocks.Insert(index, sequenceBlock);
            OnSequenceChange?.Invoke();
        }

        /// <summary>
        /// removes block at index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveFromSequence(int index)
        {
            sequenceBlocks.RemoveAt(index);
            OnSequenceChange?.Invoke();
        }
        /// <summary>
        /// removes block by reference
        /// </summary>
        /// <param name="sequenceBlock"></param>
        public void RemoveFromSequence(SequenceBlock sequenceBlock)
        {
            sequenceBlocks.Remove(sequenceBlock);
            OnSequenceChange?.Invoke();
        }



        public bool IsComplete()
        {
            foreach (var item in sequenceBlocks)
            {
                if (!item.WasPerformed())
                    return false;
            }
            //checks if all parameters were met...
            return true;
        }

        public string AllDisplayStrings()
        {
            if (sequenceBlocks.Count == 0)
                return "";

            string toReturn = "";//= sequenceBlocks[0].DisplayString();
            for (int i = 0; i < sequenceBlocks.Count; i++)
            {
                toReturn += $"\n{sequenceBlocks[i].DisplayStringAsPartOfSequence()}\n";
            }

            return toReturn;
        }
        public List<string> ListAllDisplayStrings()
        {
            if (sequenceBlocks.Count == 0)
                return null;

            List<string> toReturn = new List<string>();// treatments[0].DisplayString();
            for (int i = 0; i < sequenceBlocks.Count; i++)
            {
                toReturn.Add(sequenceBlocks[i].DisplayStringAsPartOfSequence());
            }
            return toReturn;
        }

        public List<SequenceBlock> SequenceBlocks() => sequenceBlocks;


        public void SubToOnListChanged(System.Action func)
        {
            OnSequenceChange += func;
        }

        public void MoveIndex(int index, int movement)
        {
            if (index + movement >= sequenceBlocks.Count || index + movement < 0)
            {
                Debug.LogError("cant move treatment in that direction");
                return;
            }

            SequenceBlock temp = sequenceBlocks[index];
            sequenceBlocks[index] = sequenceBlocks[index + movement];
            sequenceBlocks[index + movement] = temp;
            OnSequenceChange?.Invoke();
        }
        public List<Question> GetQuestions()
        {
            List<Question> toReturn = new List<Question>();

            foreach (var item in sequenceBlocks)
            {
                if (item is Question)
                {
                    Question temp = (Question)item;
                    toReturn.Add(temp);
                }
                else if (item is TreatmentGroup)
                {
                    TreatmentGroup tg = (TreatmentGroup)item;

                    //THIS IS A TREATMENT GROUP WHICH MAY CONTAIN QUESTIONS!
                    List<SequenceBlock> qs = tg.SequenceBlocks().Where(x => x is Question).ToList(); //!!((Question)x) parenthesis are NOT EXTRA (Question)x != null - would still consider x as SequenceBlock !

                    if (qs == null || qs.Count == 0)
                        continue; //no questions found in treatment group, continue on with the FullTreatmentSequence

                    foreach (var tgQuestion in qs)
                    {
                        toReturn.Add(tgQuestion as Question);
                    }

                }
            }
            return toReturn;
        }
    }
}