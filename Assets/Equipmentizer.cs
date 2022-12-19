using System.Collections.Generic;
using UnityEngine;

public class Equipmentizer : MonoBehaviour
{
    public SkinnedMeshRenderer TargetMeshRenderer;

    void Start()
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in TargetMeshRenderer.bones)
            boneMap[bone.gameObject.name] = bone;


        SkinnedMeshRenderer myRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        Transform[] newBones = new Transform[myRenderer.bones.Length];
        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            GameObject bone = myRenderer.bones[i].gameObject;
            if (!boneMap.TryGetValue(bone.name, out newBones[i]))
            {
                Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
                //break
                continue;
            }
        }
        myRenderer.bones = newBones;

    }
}