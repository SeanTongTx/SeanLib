using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class DynamicRef
{
    public string DynamicName;
    public string TargetGUID;
}
[ExecuteInEditMode]
[RequireComponent(typeof(ReferenceRoot))]
public class DynamicReference : MonoBehaviour
{
    public List<DynamicRef> refs = new List<DynamicRef>();

    private void OnValidate()
    {
        Sync2Root();
    }
    public void Sync2Root()
    {
        var root = GetComponent<ReferenceRoot>();
        foreach (var DynRef in refs)
        {
            if (DynRef.DynamicName.IsNOTNullOrEmpty())
            {
                root.DynamicNameDic[DynRef.DynamicName] = DynRef.TargetGUID;
            }
        }
    }
}
