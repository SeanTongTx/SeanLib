using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public abstract class FieldDrawer
{
    //父级缓存
    public object owner;
    //字段实例
    public object instance;
    //字段类型
    public Type type;
    //属性列表
    public List<PropertyAttribute> customAttribute=new List<PropertyAttribute>();
    public bool isCollection { get; private set; }
    public string TypeName { get; private set; }
    public string filedName { get; private set; }
    public FieldInfo fieldInfo;
    //public virtual string Title { get { return filedName + "(" + TypeName + ")"; } }
    public virtual string Title { get { return filedName; } }
    public virtual void OnEnbale(string filedName, object instance, Type type, FieldInfo field)
    {
        this.filedName = filedName;
        bool b;
        TypeName = FieldDrawerUtil.TypeString(type, out b);
        isCollection = b;

        this.instance = instance;
        this.type = type;
        this.fieldInfo = field;
        if (fieldInfo!=null)
        {
            object[] atts = fieldInfo.GetCustomAttributes(true);
            customAttribute.Clear();
            foreach (object att in atts)
            {
                if (att is PropertyAttribute)
                {
                    customAttribute.Add((PropertyAttribute)att);
                }
            }
        }
    }
    /// <summary>
    /// return edit value
    /// </summary>
    /// <returns></returns>
    public abstract object OnGUI(params GUILayoutOption[] options);

    public FieldDrawer Clone()
    {
        return this.MemberwiseClone() as FieldDrawer;
    }
}
