using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PrefabTools
{
    /// <summary>
    /// 判断场景中GO是否是 Prefab的实例
    /// </summary>
    /// <param name="sceneObject"></param>
    /// <returns></returns>
    public static bool IsPrefabInstance(UnityEngine.GameObject sceneObject)
    {
        UnityEngine.Object obj = PrefabUtility.GetPrefabInstanceHandle(sceneObject);
        if (obj == null) return false;
        if (sceneObject.transform.parent != null)
        {
            UnityEngine.Object objparent = PrefabUtility.GetPrefabInstanceHandle(sceneObject.transform.parent.gameObject);
            return obj != objparent;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// 获得prefab资源目录
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetPrefabAssetPath(GameObject go)
    {
        string AssetPath = string.Empty;
        AssetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(go));
        return AssetPath;
    }

    public static GameObject PutPrefabIntoScene(GameObject prefabObject)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;
        go.name = prefabObject.name;
        return go;
    }
    public static GameObject PutPrefabIntoScene(GameObject prefabObject,Transform parant,bool stay=false)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;
        go.name = prefabObject.name;
        go.transform.SetParent(parant, stay);
        return go;
    }
}