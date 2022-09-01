using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action Sequence", menuName = "Action Sequence")]
public class ActionSequence : ScriptableObject
{
    public List<string> actions = new List<string>();
}
