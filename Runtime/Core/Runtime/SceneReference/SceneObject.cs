using SeanLib.Core;
using ServiceTools.Reflect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneObjectState
{
    //存储在数据中
    store,
    //编辑器中绑定
    binding,
    //运行时放置到场景中
    placed
}

[Serializable]
public class SceneObject
{
    [Header("Hierarchy")]
    [InspectorPlus.Line]
    [InspectorPlus.ReadOnly]
    public string Name;
    [InspectorPlus.ReadOnly]
    public string ChildPath = string.Empty;
    [InspectorPlus.ReadOnly]
    public string Root = string.Empty;
    [InspectorPlus.ReadOnly]
    public int Depth =0;
    [InspectorPlus.ReadOnly]
    public SceneObjectState state;

    [InspectorPlus.ReadOnly]
    public string RefType = string.Empty;
    [Header("AssetData")]
    [InspectorPlus.Line]
    public string AssetPath;
    [InspectorPlus.GUID]
    public string GUID = string.Empty;
    [Header("Transform")]
    [InspectorPlus.Line]
    public Vector3 LocalPostion;
    public Quaternion LocalRotation;
    public Vector3 LocalScale;
    public Vector3 Postion;
    public Quaternion Rotation;
    public Vector3 Scale;
    public bool Active = true;
    [Header("Control")]
    [InspectorPlus.Line]
    public bool OverrideTransform;
    [Tooltip("运行时绑定：这个场景对象需要在运行时根据逻辑动态绑定")]
    public bool RuntimeBind;

    /// <summary>
    /// 数据绑定到Gameobject
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public ReferenceObject BindInstance(GameObject go)
    {
        ReferenceObject component = ReferenceRoot.AddReference(AssemblyTool.FindTypesInCurrentDomainByName(RefType), go, null);
        this.state = SceneObjectState.binding;
        component.Data = this;
        return component;
    }
    /// <summary>
    /// 通过数据获取场景中的Gameobject
    /// </summary>
    /// <returns></returns>
    public GameObject GetSceneInstance()
    {
        var parent = GetSceneParent();
        if (parent == null) return null;
        else
        {
            var instance = parent.transform.Find(Name);
            return instance == null ? null : instance.gameObject;
        }
    }
    /// <summary>
    /// 查找根节点
    /// </summary>
    /// <returns></returns>
    public Transform GetSceneParent()
    {
        if (string.IsNullOrEmpty(this.Root)) return null;
        Scene scene = SceneManager.GetActiveScene();
        List<GameObject> Roots = new List<GameObject>(scene.GetRootGameObjects());
        var root = Roots.Find(e => e.name == Root);
        if (root == null) return null;
        Transform target = root.transform;
        if (string.IsNullOrEmpty(ChildPath)) return target;
        var result= target.Find(ChildPath);
        if(!result)
        {
            Debug.LogError(Root+"/"+ ChildPath +"/"+Name+ " :Cant find scenepath");
        }
        return result;
    }
}