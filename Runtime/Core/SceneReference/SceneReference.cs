using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneReference
{
    /// <summary>
    /// 不要直接控制GUID
    /// 避免缓存错误
    /// </summary>
    [InspectorPlus.ReadOnly,SerializeField]
    public string GUID=string.Empty;
    /// <summary>
    /// Identity 控制GUID
    /// </summary>
    public string Identity
    {
        get
        {
            return GUID;
        }
        set
        {
            if(value!=GUID)
            {
                ClearCache();
            }
            GUID = value;
        }
    }
    public string DyamicName = string.Empty;
    public bool Dynamic = false;
    [InspectorPlus.ReadOnly]
    /// <summary>
    /// 不要直接控制TypeName
    /// 避免缓存错误
    /// </summary>
    public string TypeName = string.Empty;
    public string Type { get { return TypeName; } set
        {
            if (value != TypeName)
            {
                ClearTypeCache();
            }
            TypeName = value;
        } }
    //Cache 
    [HideInInspector, NonSerialized]
    public ReferenceObject RefObj;
    [HideInInspector, NonSerialized]
    public UnityEngine.Object ResolvedObj;
    public void ClearCache()
    {
        RefObj = null;
        ClearTypeCache();
    }
    public void ClearTypeCache()
    {
        ResolvedObj = null;
    }


    public virtual T Resolve<T>() where T : UnityEngine.Component
    {
        if (!ReferenceRoot.Instance) return null;

#if UNITY_EDITOR
        if (ResolvedObj)
            RefObj = ReferenceRoot.Instance.Get(this);
#endif

        if (!Dynamic&&ResolvedObj) return ResolvedObj as T;
        RefObj = ReferenceRoot.Instance.Get(this);
        if (!RefObj)
        {
            ResolvedObj = null;
            return null;
        }
        ResolvedObj = RefObj.Resolve<T>();
        return ResolvedObj as T;
    }
    public virtual UnityEngine.Object Resolve()
    {
        if (!ReferenceRoot.Instance) return null;

#if UNITY_EDITOR
        if(ResolvedObj)
        RefObj = ReferenceRoot.Instance.Get(this);
#endif

        if (!Dynamic && ResolvedObj) return ResolvedObj;
        RefObj = ReferenceRoot.Instance.Get(this);
        if (!RefObj) return null;

        if (string.IsNullOrEmpty(Type))
        {
            ResolvedObj= RefObj.Resolve();
        }
        else
        {
            ResolvedObj = RefObj.Resolve(Type);
        }
        return ResolvedObj;
    }
    public virtual SceneReference Clone()
    {
        return this.MemberwiseClone() as SceneReference;
    }
}

