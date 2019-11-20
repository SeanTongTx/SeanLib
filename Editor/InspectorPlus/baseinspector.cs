using SeanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class BaseInspector<T> : UnityEditor.Editor where T : UnityEngine.Object
{
    protected T Target { get { return target as T; } }
    /// <summary>
    /// 找到字段 Property 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expr"></param>
    /// <returns></returns>
    protected SerializedProperty FindProperty<TValue>(Expression<Func<T, TValue>> expr)
    {
        return serializedObject.FindProperty(FieldPath(expr));
    }

    /// <summary>
    /// 找到字段Property后 将其从默认绘制中剔除
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expr"></param>
    /// <returns></returns>
    protected SerializedProperty FindAndExcludeProperty<TValue>(Expression<Func<T, TValue>> expr)
    {
        SerializedProperty p = FindProperty(expr);
        if (p!=null)
        {
            ExcludeProperty(p.name);
        }
        return p;
    }
    protected SerializedProperty FindAndExcludeProperty(string filedPath)
    {
        SerializedProperty p = serializedObject.FindProperty(filedPath);
        if (p != null)
        {
            ExcludeProperty(p.name);
        }
        return p;
    }

    protected string FieldPath<TValue>(Expression<Func<T, TValue>> expr)
    {
        return ReflectionHelpers.GetFieldPath(expr);
    }
    protected virtual void BeginInspector()
    {
        mAdditionalExcluded = null;
        serializedObject.Update();
    }
    public override void OnInspectorGUI()
    {
        BeginInspector();
        var script = FindAndExcludeProperty("m_Script");
        if (script!=null)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(script);
            }
        }
        DrawRemainingPropertiesInInspector();
    }
    protected void DrawRemainingPropertiesInInspector()
    {
        EditorGUI.BeginChangeCheck();
        DrawPropertiesExcluding(serializedObject, GetExcludedPropertiesInInspector().ToArray());
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
    #region ExcludeProperty
    //从默认 inspector绘制中剔除
    List<string> mAdditionalExcluded;
    protected void ExcludeProperty(string propertyName)
    {
        if (mAdditionalExcluded == null)
            mAdditionalExcluded = new List<string>();
        mAdditionalExcluded.Add(propertyName);
    }

    protected virtual List<string> GetExcludedPropertiesInInspector()
    {
        var excluded = new List<string>() { };
        if (mAdditionalExcluded != null)
            excluded.AddRange(mAdditionalExcluded);
        return excluded;
    }
    //绘制Property 并冲默认绘制中剔除
    protected virtual void DrawPropertyInInspector(SerializedProperty p)
    {
        List<string> excluded = GetExcludedPropertiesInInspector();
        if (!excluded.Contains(p.name))
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(p);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            ExcludeProperty(p.name);
        }
    }
    #endregion
}

public abstract class InternalInspector<T>: BaseInspector<T> where T : UnityEngine.Object
{
    public abstract string InternalClassFullName { get; }
    /// <summary>
    /// 内部类编辑器 实例
    /// </summary>
    protected Editor m_ObjectInspector;
    protected MethodInfo m_OnHeaderGUI;
    protected void OnEnable()
    {
        //获取 编辑器 是内部类
        System.Type ObjectorInspectorType = typeof(Editor).Assembly.GetType(InternalClassFullName);
        m_OnHeaderGUI = ObjectorInspectorType.GetMethod("OnHeaderGUI",
           BindingFlags.NonPublic | BindingFlags.Instance);

        m_ObjectInspector = Editor.CreateEditor(Target, ObjectorInspectorType);
    }
    protected void OnDisable()
    {
        if (m_ObjectInspector&&!m_ObjectInspector.Equals(null))
        {
            DestroyImmediate(m_ObjectInspector);
        }
        m_ObjectInspector = null;
    }

    protected override void OnHeaderGUI()
    {
        if (m_OnHeaderGUI != null)
        {
            m_OnHeaderGUI.Invoke(m_ObjectInspector, null);
        }
    }
    public override void OnInspectorGUI()
    {
        m_ObjectInspector.OnInspectorGUI();
    }

    public override bool HasPreviewGUI()
    {
        return m_ObjectInspector.HasPreviewGUI();
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        m_ObjectInspector.OnPreviewGUI(r, background);
    }
    public override string GetInfoString()
    {
        return m_ObjectInspector.GetInfoString();
    }
    public override GUIContent GetPreviewTitle()
    {
        return m_ObjectInspector.GetPreviewTitle();
    }
    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        m_ObjectInspector.OnInteractivePreviewGUI(r, background);
    }

    public override void OnPreviewSettings()
    {
        m_ObjectInspector.OnPreviewSettings();
    }

    public override void ReloadPreviewInstances()
    {
        m_ObjectInspector.ReloadPreviewInstances();
    }
    public override bool RequiresConstantRepaint()
    {
        return m_ObjectInspector.RequiresConstantRepaint();
    }

    public override bool UseDefaultMargins()
    {
        return m_ObjectInspector.UseDefaultMargins();
    }
}
