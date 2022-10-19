using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    [System.Serializable]
    public class TreatmentGroup : SequenceBlock, IBlockCollection // a group of actions that may be performed in NO particular order
    {
        //List<Treatment> treatments; // => new List<Treatment>() with tests and questions

        [SerializeField]
        List<SequenceBlock> treatments; //Could be a list of sequence blocks...

        public System.Action OnSequenceChange;

        public void Init()
        {
            treatments = new List<SequenceBlock>();
            typeString = GetType().ToString().Substring(21); //Ignores the initial namespace and period ("PatientCreationSpace.") - 21 chars
        }

        public void AddTreatment(SequenceBlock t)
        {
            treatments.Add(t);
            OnSequenceChange?.Invoke();
        }
        public void RemoveTreatment(SequenceBlock t)
        {
            treatments.Remove(t);
            OnSequenceChange?.Invoke();
        }
        public void RemoveTreatment(int index)
        {
            treatments.RemoveAt(index);
            OnSequenceChange?.Invoke();
        }

        public override bool WasPerformed()
        {
            foreach (var item in treatments)
            {
                if (!item.WasPerformed())
                    return false;
            }
            return true;
        }
        public string AllDisplayStrings() => DisplayStringAsPartOfSequence();
        public override string DisplayStringAsPartOfSequence()
        {
            if (treatments.Count == 0)
                return "";

            string toReturn = "";// treatments[0].DisplayString();
            for (int i = 0; i < treatments.Count; i++)
            {
                toReturn += $"{treatments[i].DisplayStringAsPartOfSequence()}\n";
            }
            return toReturn;
        }
        public List<string> ListAllDisplayStrings()
        {
            if (treatments.Count == 0)
                return null;

            List<string> toReturn = new List<string>();// treatments[0].DisplayString();
            for (int i = 0; i < treatments.Count; i++)
            {
                toReturn.Add(treatments[i].DisplayStringAsPartOfSequence());
            }
            return toReturn;
        }

        public List<SequenceBlock> SequenceBlocks()
        {
            return treatments;
        }

        //public void SubToOnListChanged(Action func)
        //{
        //    OnSequenceChange += func;
        //}

        public void MoveIndex(int index, int movement)
        {
            if (index + movement >= treatments.Count || index + movement < 0)
            {
                Debug.LogError("cant move treatment in that direction");
                return;
            }
            SequenceBlock temp = treatments[index];
            treatments[index] = treatments[index + movement];
            treatments[index + movement] = temp;
            OnSequenceChange?.Invoke();
        }

        public void SubToOnListChanged(System.Action func)
        {
            OnSequenceChange += func;
        }
    }

}