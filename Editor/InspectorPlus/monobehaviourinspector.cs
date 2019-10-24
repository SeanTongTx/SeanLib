
using EditorPlus;
using SeanLib.Core;
using ServiceTools.Reflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

using Object = UnityEngine.Object;
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourInspector : BaseInspector<MonoBehaviour>
{
    public override void OnInspectorGUI()
    {
        DrawHelp();
        base.OnInspectorGUI();
        DrawButton();
    }
    void DrawHelp()
    {
        Type type = target.GetType();
        var helpBox = type.GetCustomAttribute<InspectorPlus.HelpBox>();
        if (helpBox != null)
        {
            EditorGUILayout.HelpBox(helpBox.Msg, (MessageType)helpBox.MessageType);
        }
    }
    void DrawButton()
    {
        Type type = target.GetType();
        MethodInfo[] ms = type.GetMethods();
        foreach (var item in ms)
        {
            InspectorPlus.Button att_button = ReflecTool.GetAttribute<InspectorPlus.Button>(item);
            if (att_button != null)
            {
                if (att_button.Playing && Application.isPlaying)
                {
                }
                else if (att_button.Editor && !Application.isPlaying)
                {
                }
                else
                {
                    OnGUIUtility.Vision.GUIEnabled(false);
                }
                Button(item, att_button);

                OnGUIUtility.Vision.GUIEnabled(true);
            }
        }
    }
   protected void Button(MethodInfo item, InspectorPlus.Button att_button)
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label(new GUIContent(att_button.Title, att_button.Help));
            if (GUILayout.Button(item.Name))
            {
                foreach (var tgt in targets)
                {
                    if (tgt.GetType() == target.GetType())
                        item.Invoke(target, null);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    /*
    public Dictionary<string, ReorderableList> RecorderLists = new Dictionary<string, ReorderableList>();
    List<int> deleteList = new List<int>();
    private ReorderableList currentList;
    public void OnEnable()
    {
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            if (property.propertyType == SerializedPropertyType.Generic)
            {
                InspectorPlus.Orderable att = PropertyDrawerTools.GetAttribute<InspectorPlus.Orderable>(property);
                if (att != null)
                {
                    ReorderableList list = new ReorderableList(
                        serializedObject,
                        serializedObject.FindProperty(property.propertyPath),
                        true,
                        true,
                        true,
                        true);
                    list.drawElementCallback = DrawListElement;
                    list.drawHeaderCallback = DrawHeader;
                    list.drawFooterCallback = DrawFooter;
                    list.onRemoveCallback = Remove;
                    list.elementHeightCallback = ElementHeight;
                    RecorderLists[property.propertyPath] = list;
                }
            }
            enterChildren = false;
        }
    }
    private void DrawFooter(Rect rect)
    {
        float xMax = rect.xMax;
        float num = xMax - 8f;
        if (currentList.displayAdd)
        {
            num -= 25f;
        }
        if (currentList.displayRemove)
        {
            num -= 25f;
        }
        Rect r0 = new Rect(rect.x, rect.y, 25f, 13f);
        Rect p0 = new Rect(rect.x, rect.y - 4f, 25f, 13f);
        rect = new Rect(num, rect.y, xMax - num, rect.height);
        Rect rect2 = new Rect(num + 4f, rect.y - 3f, 25f, 13f);
        Rect rect3 = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);

        if (Event.current.type == EventType.Repaint)
        {
            ReorderableList.defaultBehaviours.footerBackground.Draw(rect, false, false, false, false);
            ReorderableList.defaultBehaviours.footerBackground.Draw(r0, false, false, false, false);
        }
        if (GUI.Button(p0, "c", ReorderableList.defaultBehaviours.preButton))
        {
            IList list = PropertyDrawerTools.GetPropertyInstance<IList>(currentList.serializedProperty);
            if (list != null)
            {
                list.Clear();
            }
        }
        if (currentList.displayAdd)
        {
            if (GUI.Button(
                rect2,
                (currentList.onAddDropdownCallback == null) ? ReorderableList.defaultBehaviours.iconToolbarPlus : ReorderableList.defaultBehaviours.iconToolbarPlusMore,
                ReorderableList.defaultBehaviours.preButton))
            {
                if (currentList.onAddDropdownCallback != null)
                {
                    currentList.onAddDropdownCallback(rect2, currentList);
                }
                else if (currentList.onAddCallback != null)
                {
                    currentList.onAddCallback(currentList);
                }
                else
                {
                    ReorderableList.defaultBehaviours.DoAddButton(currentList);
                }
                if (currentList.onChangedCallback != null)
                {
                    currentList.onChangedCallback(currentList);
                }
            }
        }
        if (currentList.displayRemove)
        {
            using (new EditorGUI.DisabledScope(currentList.index < 0 || currentList.index >= currentList.count || (currentList.onCanRemoveCallback != null && !currentList.onCanRemoveCallback(currentList))))
            {
                if (GUI.Button(rect3, ReorderableList.defaultBehaviours.iconToolbarMinus, ReorderableList.defaultBehaviours.preButton))
                {
                    Remove(currentList);
                    if (currentList.onChangedCallback != null)
                    {
                        currentList.onChangedCallback(currentList);
                    }
                }
            }
        }
    }
    private void Remove(ReorderableList list)
    {
        IList l = PropertyDrawerTools.GetPropertyInstance<IList>(currentList.serializedProperty);
        if (l != null)
        {
            l.RemoveAt(list.index);
        }
    }
    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, currentList.serializedProperty.name);
        var eventType = Event.current.type;
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                // Show a copy icon on the drag
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        IList list = PropertyDrawerTools.GetPropertyInstance<IList>(currentList.serializedProperty);
                        if (list != null)
                        {
                            Type t = list.GetType().GetGenericArguments()[0];
                            foreach (Object reference in DragAndDrop.objectReferences)
                            {
                                if (reference is GameObject)
                                {
                                    if (t == typeof(GameObject))
                                    {
                                        list.Add(reference);
                                    }
                                    else
                                    {
                                        Component c = (reference as GameObject).GetComponent(t);
                                        if (c)
                                        {
                                            list.Add(c);
                                        }
                                    }
                                }
                                else if (reference.GetType() == t || reference.GetType().IsSubclassOf(t))
                                {
                                    list.Add(reference);
                                }
                            }
                        }
                    }
                }
                Event.current.Use();
            }
        }
    }
    private float ElementHeight(int index)
    {
        SerializedProperty itemData = serializedObject.FindProperty(currentList.serializedProperty.propertyPath).GetArrayElementAtIndex(index);
        return EditorGUI.GetPropertyHeight(itemData);
    }
    private void DrawListElement(Rect rect, int index, bool isactive, bool isfocused)
    {
        SerializedProperty itemData = serializedObject.FindProperty(currentList.serializedProperty.propertyPath).GetArrayElementAtIndex(index);
        Rect r0 = Rect.zero;
        Rect r1 = OnGUIUtility.Layout.Divide.Divide2Horizontal(rect, out r0, 20);
        Rect r2 = OnGUIUtility.Layout.Divide.Divide2Horizontal(r1, out r1, r1.width - 20);
        r2.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(r1, itemData);
        if (GUI.Button(r2, "X"))
        {
            deleteList.Add(index);
        }
    }*/
}