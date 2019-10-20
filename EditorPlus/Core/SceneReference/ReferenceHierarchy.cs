using SeanLib.Core;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace EditorPlus
{
    public class ReferenceHierarchy : EditorWindow
    {
        [MenuItem("Window/ReferenceWindow &#2")]
        public static void ShowWindow()
        {
            ReferenceHierarchy w = GetWindow<ReferenceHierarchy>();
            w.Show();
        }

        [MenuItem("GameObject/RefObject/ReferenceObject &#r", false, 10)]
        public static void CreateReferenceObject()
        {
            CreateRefObject<ReferenceObject>();
        }
        public static void CreateRefObject<T>() where T : ReferenceObject
        {
            var go = new GameObject("ReferenceObject");
            ReferenceRoot.AddReference<T>(go, null);
            if (Selection.activeGameObject)
                go.transform.SetParent(Selection.activeGameObject.transform);
            Selection.activeGameObject = go;
        }
        public static void CreateReferenceRoot()
        {
            var root = new GameObject("ReferenceRoot", typeof(ReferenceRoot));
        }

        private void OnEnable()
        {
            search = new OnGUIUtility.Search();
            RefreshData();
        }
        private void OnDisable()
        {

        }
        OnGUIUtility.Search search;
        public Dictionary<ReferenceObject, ReferenceObject> AllDic = new Dictionary<ReferenceObject, ReferenceObject>();
        public Dictionary<string, ReferenceObject> GUIDdic = new Dictionary<string, ReferenceObject>();
        public List<SceneReference> References = new List<SceneReference>();
        Vector2 v;
        bool Missing = false;
        public OnGUIUtility.TabGroup ShowMode = new OnGUIUtility.TabGroup(new string[] { "场景引用", "资源引用", "动态引用" });

        List<UnityEngine.Object> TestAssets=new List<UnityEngine.Object>();
        public void OnGUI()
        {
            if (!ReferenceRoot.Instance)
            {
                EditorGUILayout.HelpBox("Scene need RefrenceRoot", MessageType.Error);
                if (GUILayout.Button("Create Root"))
                {
                    CreateReferenceRoot();
                }
                return;
            }
            EditorGUILayout.BeginHorizontal();
            {
                ShowMode.OnGui(EditorStyles.toolbarButton);
                search.OnToolbarGUI();
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {
                    RefreshData();
                }
                if (Event.current.type == EventType.Layout && Event.current.keyCode == KeyCode.R)
                {
                    RefreshData();
                }
                if (Event.current.type == EventType.Layout && Missing)
                {
                    RefreshData();
                    Missing = false;
                }
            }
            EditorGUILayout.EndHorizontal();

            v = EditorGUILayout.BeginScrollView(v);
            {
                if (ShowMode.IsEnable(0))
                {
                    foreach (var objPair in AllDic)
                    {
                        if (objPair.Key == null)
                        {
                            Missing = true;
                            break;
                        }
                        if(filtRefObj(objPair.Key))
                        {
                            bool hasConflit = objPair.Value;
                            OnGUIUtility.Vision.BeginBackGroundColor(hasConflit ? Color.red : Color.white);
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(objPair.Key.Data.GUID, GUILayout.MaxWidth(100));
                                if (GUILayout.Button(objPair.Key.gameObject.name, OnGUIUtility.Styles.Title))
                                {
                                    Selection.activeGameObject = objPair.Key.gameObject;
                                }
                                if (hasConflit)
                                {
                                    if (GUILayout.Button("New", GUILayout.MaxWidth(64)))
                                    {
                                        NewGUID(objPair.Key);
                                        return;
                                    }
                                    if (GUILayout.Button(objPair.Value.gameObject.name, OnGUIUtility.Styles.Title))
                                    {
                                        Selection.activeGameObject = objPair.Value.gameObject;
                                    }
                                    if (GUILayout.Button("New", GUILayout.MaxWidth(64)))
                                    {
                                        NewGUID(objPair.Value);
                                        return;
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            OnGUIUtility.Vision.EndBackGroundColor();
                        }
                    }  
                }
                else if (ShowMode.IsEnable(1))
                {
                    FieldDrawerUtil.ObjectField("测试资源", TestAssets);
                    foreach (var reference in References)
                    {
                        if (filtReference(reference))
                        {
                            var refobj = ReferenceRoot.Instance.Get(reference.Identity);
                            Color color = Color.white;
                            if (!refobj) color = OnGUIUtility.Colors.red;
                            if (reference.Dynamic) color = OnGUIUtility.Colors.blue;
                            OnGUIUtility.Vision.BeginBackGroundColor(color);
                            EditorGUILayout.BeginHorizontal();
                            {
                                var title = reference.Identity;
                                if (reference.Dynamic) title = "(" +reference.DyamicName +")" + reference.Identity;
                                GUILayout.Label(title, GUILayout.MaxWidth(150));
                                if (refobj)
                                {
                                    if (GUILayout.Button(refobj.gameObject.name, OnGUIUtility.Styles.Title))
                                    {
                                        Selection.activeGameObject = refobj.gameObject;
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button("Null", OnGUIUtility.Styles.Title))
                                    {
                                        ObjectPopupWindow.Show(reference);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            OnGUIUtility.Vision.EndBackGroundColor();
                        }
                    }
                }
                else if (ShowMode.IsEnable(2))
                {
                    OnGUIUtility.Vision.BeginBackGroundColor(Color.blue);
                    foreach (var item in ReferenceRoot.Instance.DynamicNameDic)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.SelectableLabel(item.Key, GUILayout.MaxWidth(100));
                            EditorGUILayout.SelectableLabel(item.Value, GUILayout.MaxWidth(300));
                            var refObj = ReferenceRoot.Instance.GetDynamic(item.Key);
                            if (refObj)
                            {
                                if (GUILayout.Button(refObj.gameObject.name, OnGUIUtility.Styles.Title))
                                {
                                    Selection.activeGameObject = refObj.gameObject;
                                }
                            }
                            else
                            {
                                GUILayout.Button("Null", OnGUIUtility.Styles.Title);
                            }

                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    OnGUIUtility.Vision.EndBackGroundColor();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        bool filtRefObj(ReferenceObject refobj)
        {
            if (search.Current.IsNullOrEmpty()) return true;
            foreach (var kvFilter in search.filter.filters)
            {
                if (kvFilter.Key == "t"&& refobj.GetType().FullName.ToLower().Contains(kvFilter.Value.ToLower()))
                {
                    if (search.filter.value.IsNullOrEmpty()) return true;
                    else
                    {
                        return refobj.Data.GUID.ToLower().Contains(search.filter.value.ToLower());
                    }
                }
                else if(kvFilter.Key=="n"&& refobj.name.ToLower().Contains(kvFilter.Value.ToLower()))
                {
                    if (search.filter.value.IsNullOrEmpty()) return true;
                    else
                    {
                        return refobj.Data.GUID.ToLower().Contains(search.filter.value.ToLower());
                    }
                }
            }
            return search.GeneralValid(refobj.Data.GUID);
        }
        bool filtReference(SceneReference reference)
        {
            foreach (var kvFilter in search.filter.filters)
            {
                if(kvFilter.Key=="t" &&kvFilter.Value=="dynamic"&& reference.Dynamic)
                {
                    return reference.Identity.ToLower().Contains(search.filter.value);
                }
            }
            return search.GeneralValid(reference.Identity);
        }

        private void NewGUID(ReferenceObject refobj)
        {
            Undo.RecordObject(refobj, "RefObj");
            refobj.Data.GUID = GUIDHelper.NewGUID();
            PrefabUtility.RecordPrefabInstancePropertyModifications(refobj);
            RefreshData();
        }
        public void RefreshData()
        {
            //刷新object
            var references = GameObjectUtilities.FindSceneAllGameObjects<ReferenceObject>();
            GUIDdic.Clear();
            AllDic.Clear();
            References.Clear();
            foreach (var refobj in references)
            {
                ReferenceObject conflitObj = null;
                if (GUIDdic.ContainsKey(refobj.Data.GUID))
                {
                    conflitObj = GUIDdic[refobj.Data.GUID];
                }
                else
                {
                    GUIDdic[refobj.Data.GUID] = refobj;
                }
                AllDic[refobj] = conflitObj;
            }

            Regex r = new Regex(@"GUID:\s*(\S*)\s*DyamicName:\s*(\S*)\s*Dynamic:\s*(\S*)\s*TypeName:", RegexOptions.IgnoreCase);
            //刷新引用
            foreach (var asset in TestAssets)
            {
                if (asset)
                {
                    var filepath = PathTools.Asset2File(AssetDatabase.GetAssetPath(asset));
                    string allFileContent = FileTools.ReadAllText(filepath);
                    MatchCollection ms = r.Matches(allFileContent);
                    for (int i = 0; i < ms.Count; i++)
                    {
                        Match m = ms[i];
                        String guid1 = m.Groups[1].ToString();
                        String dynaimcName = m.Groups[2].ToString();
                        String dynaimc = m.Groups[3].ToString();
                        if (guid1.IsNOTNullOrEmpty())
                        {
                            bool dyn = dynaimc=="1"?true:false;
                            References.Add(new SceneReference() { Identity = guid1,Dynamic= dyn ,DyamicName=dynaimcName});
                        }
                    }
                }
            }

        }
    }

}