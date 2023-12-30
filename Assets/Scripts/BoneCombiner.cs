using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCombiner 
{
    public readonly Dictionary<int, Transform> _RootBoneDictonary = new Dictionary<int, Transform>();
    private readonly Transform[] _boneTransforms = new Transform[67];

    private readonly Transform _transform;

    public BoneCombiner(GameObject rootObj)
    {
        _transform = rootObj.transform;
        TraverseHierarchy(_transform);
    }

    public Transform AddLimb(GameObject boneObj, List<string> boneName)
    {
        Transform limb = ProcessBoneObject(boneObj.GetComponentInChildren<SkinnedMeshRenderer>(), boneName);
        limb.SetParent(_transform); 
        return limb;
    }

    private Transform ProcessBoneObject(SkinnedMeshRenderer renderer, IReadOnlyList<string> boneNames)
    {
        var boneObject = new GameObject().transform;
        var meshRenderer = boneObject.gameObject.AddComponent<SkinnedMeshRenderer>();
        //var bones = renderer.bones;
        for (int i = 0; i < boneNames.Count; i++)
        {
            _boneTransforms[i] = _RootBoneDictonary[boneNames[i].GetHashCode()];
        }
        meshRenderer.bones = _boneTransforms;
        meshRenderer.sharedMesh = renderer.sharedMesh;
        meshRenderer.materials = renderer.sharedMaterials;
        return boneObject;
    }

    private void TraverseHierarchy(IEnumerable transform)
    {
        foreach (Transform child in transform)
        {
            _RootBoneDictonary.Add(child.name.GetHashCode(), child);
            TraverseHierarchy(child);
        }
    }
}
