using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatientCreationSpace
{

    public interface IBlockCollection
    {
        List<SequenceBlock> SequenceBlocks();
        void SubToOnListChanged(System.Action func);
        /// <summary>
        /// Move an item up or down, as much as you want
        /// </summary>
        /// <param name="index">the element to move</param>
        /// <param name="movement">amount and direction of movement. Movement is added to index, swapping the [index+movement] element with the [index] element</param>
        void MoveIndex(int index, int movement);

        string AllDisplayStrings();
        List<string> ListAllDisplayStrings();

    }

}