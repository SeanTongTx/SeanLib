using SeanLib.Core;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class ReferenceRoot : MonoBehaviour
{
    private static ReferenceRoot _Instance;
    public static ReferenceRoot Instance
    {
        get
        {
            if(_Instance==null)
            {
                _Instance = GameObject.FindObjectOfType<ReferenceRoot>();
            }
            return _Instance;
        }
    }
    [InspectorPlus.ReadOnly]
    public List<ReferenceObject> Refrences = new List<ReferenceObject>();
    public Dictionary<string, ReferenceObject> dic = new Dictionary<string, ReferenceObject>();
    public Dictionary<string, string> DynamicNameDic = new Dictionary<string, string>();
    [InspectorPlus.Button(Editor =true)]
    public void CollectRefrence()
    {
        Refrences = GameObjectUtilities.FindSceneAllGameObjects<ReferenceObject>();
        dic.Clear();
        foreach (var refobj in Refrences)
        {
            if (dic.ContainsKey(refobj.Data.GUID))
            {
                LogConflict(dic[refobj.Data.GUID], refobj);
            }
            else
            {
                dic.Add(refobj.Data.GUID, refobj);
            }
        }
        DynamicReference dynamicRef = GetComponent<DynamicReference>();
        if(dynamicRef)
        {
            dynamicRef.Sync2Root();
        }
    }
    private void LogConflict(ReferenceObject arg1, ReferenceObject arg2)
    {
        Debug.LogError("GUID 冲突[" + arg1.Data.GUID+"]"+ arg1.gameObject + ":" + arg2.gameObject);
    }
    private void OnEnable()
    {
        if (!_Instance)
        {
            _Instance = this;
            CollectRefrence();
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.hierarchyChanged += hierarchyChanged;
        }
#endif
    }
    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.hierarchyChanged -= hierarchyChanged;
        }
#endif
        Refrences.Clear();
    }
    public bool Put(ReferenceObject refObj)
    {
        return Put(refObj, LogConflict);
    }
    public bool Put(ReferenceObject refObj, Action<ReferenceObject, ReferenceObject> ConflictHandle)
    {
        ReferenceObject obj = null;
       if(dic.TryGetValue(refObj.Data.GUID, out obj))
        {
            if(!obj)
            {
                dic[refObj.Data.GUID] = refObj;
                return true;
            }
            else if(obj!=refObj)
            {
                ConflictHandle?.Invoke(obj, refObj);
            }
            return false;
        }
        else
        {
            Refrences.Add(refObj);
            dic[refObj.Data.GUID] = refObj;
            return true;
        }
    }
    public ReferenceObject Get(SceneReference reference)
    {
        var result = Get(reference.Identity);
        if (result) return result;
        if(reference.Dynamic)
        {
            return GetDynamic(reference.DyamicName);
        }
        return null;
    }
    public ReferenceObject GetDynamic(string DynamicName)
    {
        var dynamicGUID = string.Empty;
        if (DynamicNameDic.TryGetValue(DynamicName, out dynamicGUID))
        {
            return Get(dynamicGUID);
        }
        return null;
    }
    public ReferenceObject Get(string GUID)
    {
        if(dic.Count!=Refrences.Count)
        {
            CollectRefrence();
        }
        ReferenceObject obj = null;
        dic.TryGetValue(GUID, out obj);
        return obj;
    }
    public ReferenceObject Find<T>(string name)
    {
        return Refrences.Find(e => (e.name == name && e is T));
    }
    /// <summary>
    /// Get Exist RefObject or Create new one
    /// if this gameobject already have a RefObject but not T this will not effect
    /// </summary>
    /// <typeparam name="T">RefObjType</typeparam>
    /// <param name="go"></param>
    /// <param name="ComponentType"></param>
    /// <param name="sceneRef"></param>
    /// <returns></returns>
    public static T AddReference<T>(GameObject go, Type ComponentType, SceneReference sceneRef = null) where T: ReferenceObject
    {
        return AddReference(typeof(T), go,  ComponentType, sceneRef) as T;
    }
    public static ReferenceObject AddReference(Type RefObjType, GameObject go, Type ComponentType, SceneReference sceneRef = null)
    {
       var refObj = go.GetComponent(RefObjType) as ReferenceObject;
        if (refObj == null)
        {
            refObj = go.AddComponent(RefObjType) as ReferenceObject;
            refObj.TryInit();
            if (Instance)
            {
                Instance.Refrences.Add(refObj);
            }
        }
        if (sceneRef != null)
        {
            if (string.IsNullOrEmpty(refObj.Data.GUID))
            {
                refObj.Data.GUID = Guid.NewGuid().ToString();
            }
            sceneRef.Identity = refObj.Data.GUID;
            sceneRef.RefObj = refObj;
            if (ComponentType != null)
            {
                sceneRef.Type = ComponentType.Name;
            }
        }
        return refObj;
    }
    public static ReferenceObject AddReference(GameObject go,Type type, SceneReference sceneRef = null)
    {
        return AddReference<ReferenceObject>(go,type,sceneRef);
    }
    public static ReferenceObject AddReference(GameObject go, SceneReference sceneRef = null)
    {
        return AddReference<ReferenceObject>(go, null, sceneRef);
    }

    public static string AddMissingReferenceGUID(GameObject go, SceneReference sceneRef = null)
    {
        return AddReference<ReferenceObject>(go, null, sceneRef).Data.GUID;
    }



    public void SetDynamicName(string dynamicName,string TargetGUI)
    {
        DynamicNameDic[dynamicName] = TargetGUI;
    }
    public void SetDynamicName(DynamicRef dynamicReference)
    {
        SetDynamicName(dynamicReference.DynamicName, dynamicReference.TargetGUID);
    }

    #region Editor

#if UNITY_EDITOR
    //场景中变化时
    private void hierarchyChanged()
    {
        this.CollectRefrence();
    }
#endif
    #endregion
}
