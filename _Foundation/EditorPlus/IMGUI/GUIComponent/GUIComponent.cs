using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        public static class Grid
        {
            public class GridContainer<T>
            {
                public enum Operation
                {
                    none,
                    shear,
                    copy
                }
                public Operation oper = Operation.none;
                public List<T> selected = new List<T>();
                public static List<T> Clipboard = new List<T>();
                public static List<T> CopyClipboard = new List<T>();
                public Func<T, int, T> onGui;
                public Func<T, int, bool> isShow;
                public Func<T> onCreate;
                public int maxColumn = 5;
                public string add = "+";
                public string del = "x";
                public Action<GenericMenu> OnShowMenu;
                public bool Selectable;
                public void OnEnable(Func<T, int, T> onGui, Func<T> onCreate, int maxColumn = 5, string add = "+", string del = "x")
                {
                    this.onGui = onGui;
                    this.onCreate = onCreate;
                    this.maxColumn = maxColumn;
                    this.add = add;
                    this.del = del;
                }
                public void OnGUI(List<T> list)
                {
                    int indexMax = list.Count - 1;
                    int row = (indexMax + maxColumn) / maxColumn;
                    int index = 0;
                    List<int> deletes = new List<int>();
                    for (int i = 0; i < row; i++)
                    {
                        int colCount = (list.Count - index) < maxColumn ? (list.Count - index) : maxColumn;
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < colCount; j++)
                        {
                            if (isShow != null && !isShow(list[index], index))
                            {
                                index++;
                                continue;
                            }
                            list[index] = onGui(list[index], index);
                            var rect = GUILayoutUtility.GetLastRect();
                            if (Selectable&&Event.current.type == EventType.MouseDown)
                            {
                                if (Event.current.button == 0)
                                {
                                    if (rect.Contains(Event.current.mousePosition))
                                    {
                                        Event.current.Use();
                                        selected.Add(list[index]);
                                    }
                                    else
                                    {
                                        if (!Event.current.control)
                                        {
                                            selected.Clear();
                                        }
                                    }
                                }
                                else
                                {
                                    if (selected.Count >= 1 && rect.Contains(Event.current.mousePosition))
                                    {
                                        Event.current.Use();
                                        var menu = new GenericMenu();
                                        if (OnShowMenu != null)
                                        {
                                            OnShowMenu(menu);
                                        }
                                        menu.ShowAsContext();
                                    }
                                }
                            }
                            if (Event.current.type == EventType.KeyDown)
                            {
                                if (Event.current.keyCode == KeyCode.Escape)
                                {
                                    oper = Operation.none;
                                    selected.Clear();
                                }
                            }
                            if (selected.Contains(list[index]))
                            {
                                EditorGUI.DrawRect(rect, new Color(0, 0, 1, 0.1f));
                            }
                            GUILayout.BeginVertical(GUILayout.Width(del.Length * 16));
                            if (GUILayout.Button(del, EditorStyles.label))
                            {
                                deletes.Add(index);
                            }
                            GUILayout.EndVertical();
                            index++;
                        }
                        GUILayout.EndHorizontal();
                    }

                    foreach (var delete in deletes)
                    {
                        list.RemoveAt(delete);
                    }

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button(add, GUILayout.Width(add.Length * 16)))
                    {
                        list.Add(onCreate());
                    }
                    GUILayout.EndHorizontal();
                }
            }
            public static void GridList<T>(List<T> list, Func<T, int, T> onGui, Func<T> onCreate, int maxColumn = 5, string add = "+", string del = "x")
            {
                try
                {
                    int indexMax = list.Count - 1;
                    int row = (indexMax + maxColumn) / maxColumn;
                    int index = 0;
                    List<int> deletes = new List<int>();
                    for (int i = 0; i < row; i++)
                    {
                        int colCount = (list.Count - index) < maxColumn ? (list.Count - index) : maxColumn;
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < colCount; j++)
                        {
                            list[index] = onGui(list[index], index);
                            GUILayout.BeginVertical(GUILayout.Width(del.Length * 16));
                            if (GUILayout.Button(del, EditorStyles.label))
                            {
                                deletes.Add(index);
                            }
                            GUILayout.EndVertical();
                            index++;
                        }
                        GUILayout.EndHorizontal();
                    }

                    foreach (var delete in deletes)
                    {
                        list.RemoveAt(delete);
                    }
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button(add, GUILayout.Width(add.Length * 16)))
                    {
                        list.Add(onCreate());
                    }
                    GUILayout.EndHorizontal();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
            }
        }
        /*

         */
    }
}