using ServiceTools.Reflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class ReferenceObject : MonoBehaviour
{
    public virtual Color color
    {
        get
        {
            return Color.gray;
        }
    }
    public SceneObject Data = new SceneObject() { state = SceneObjectState.binding };
    public virtual T Resolve<T>() where T : UnityEngine.Component
    {
        return transform.GetComponent<T>();
    }
    public virtual UnityEngine.Object Resolve(string TypeName)
    {
        return transform.GetComponent(TypeName);
    }
    public virtual UnityEngine.Object Resolve()
    {
        return gameObject;
    }
    public string GUID
    {
        get
        {
            return Data.GUID;
        }
        set
        {
            Data.GUID = value;
        }
    }
    /// <summary>
    /// 记录场景数据
    /// </summary>
    [ContextMenu("SetSceneData")]
    public virtual void SetSceneData()
    {
        Data.Name = this.gameObject.name;
        Data.Depth = 0;
        StringBuilder sb = new StringBuilder();
        Transform parent = this.transform.parent;
        while (parent != null)
        {
            if (parent.parent != null)
            {
                if(sb.Length>0)
                {
                    sb.Insert(0,"/");
                }
                sb.Insert(0, parent.name);
                Data.Depth++;
               parent = parent.parent;
            }
            else
            {
                break;
            }
        }
        Data.ChildPath = sb.ToString();
        Data.Root = parent ? parent.name : string.Empty ;
        Data.LocalPostion = this.transform.localPosition;
        Data.LocalRotation = this.transform.localRotation;
        Data.LocalScale = this.transform.localScale;
        Data.Postion = this.transform.position;
        Data.Rotation = this.transform.rotation;
        Data.Scale = this.transform.lossyScale;
        Data.Active = this.gameObject.activeSelf;
    }
    /// <summary>
    /// 放置到场景中
    /// </summary>
    public virtual void PuToScene()
    {
        Data.state = SceneObjectState.placed;
        Vector3 lpos = transform.localPosition;
        Quaternion lros = transform.localRotation;
        Vector3 lSca = transform.localScale;
        bool ac = this.gameObject.activeSelf;
        this.transform.SetParent(Data.GetSceneParent());
        if (Data.OverrideTransform)
        {
            lpos = Data.LocalPostion;
            lros = Data.LocalRotation;
            lSca = Data.LocalScale;
            ac = Data.Active;
        }
        this.transform.localPosition = lpos;
        this.transform.localRotation = lros;
        this.transform.localScale = lSca;
        this.transform.gameObject.SetActive(ac);
    }

    private void Awake()
    {
        TryInit();
    }
    public void TryInit()
    {
        if (string.IsNullOrEmpty(Data.GUID))
        {
            Data.GUID = Guid.NewGuid().ToString();
        }
        if (string.IsNullOrEmpty(Data.RefType))
        {
            Data.RefType = GetType().FullName;
        }
        if(ReferenceRoot.Instance)
        {
            ReferenceRoot.Instance.Put(this);
        }
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(this.transform.position, 0.03f);
    }

}
