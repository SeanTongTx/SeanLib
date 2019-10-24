
namespace SeanLib.TortoiseSVN.Editor
{
#if EDITORPLUS
    using EditorPlus;
    using ServiceTools.Reflect;
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;
#if EDITORPLUS
    [CustomSeanLibEditor("Other/TortoiseSVN", order = 100)]
    public class TortoiseSVNMenu : SeanLibEditor
#else
    public class TortoiseSVNMenu : EditorWindow
#endif
    {

        [MenuItem("Assets/TortoiseSVN/Setting", false, 119)]
        //[MenuItem("Tools/SVNSetting")]
        private static void Setting()
        {
#if EDITORPLUS
            var tortoiseSvnMenu = EditorWindow.GetWindow<SeanLibManager>();
            var item = tortoiseSvnMenu.SeachIndex("Other/TortoiseSVN");
            if (item != null)
            {
                tortoiseSvnMenu.SelectIndex(item.id);
            }
#else
            var tortoiseSvnMenu = EditorWindow.GetWindow<TortoiseSVNMenu>();
#endif
        }

        [MenuItem("Assets/TortoiseSVN/Commit", false, 111)]
        private static void SVNCommit()
        {
            TortoiseSVNCommit(GetSelectionPath());
        }

        [MenuItem("Assets/TortoiseSVN/Update", false, 110)]
        private static void SVNUpdate()
        {
            TortoiseSVNUpdate(GetSelectionPath());
        }
        [MenuItem("Assets/TortoiseSVN/UpdateAll", false, 112)]
        private static void UpdateAll()
        {
            List<string> paths = SubSVNSetting.Paths;
            TortoiseSVNUpdate(paths.ToArray());
        }
        [MenuItem("Assets/TortoiseSVN/CommitAll", false, 113)]
        private static void CommitAll()
        {
            List<string> paths = SubSVNSetting.Paths;
            foreach (string path in paths)
            {
                TortoiseSVNCommit(path);
            }
        }


        public static void TortoiseSVNUpdate(params string[] path)
        {
            RunCmd(TortoiseSVNSetting.TortoiseProcPath, string.Format("/command:update  /path:\"{0}\"", path));
            AssetDatabase.Refresh();
        }

        public static void TortoiseSVNCommit(params string[] path)
        {
            RunCmd(TortoiseSVNSetting.TortoiseProcPath, string.Format("/command:commit  /path:\"{0}\"", path));
            AssetDatabase.Refresh();
        }



        public static string[] GetSelectionPath()
        {
            string[] path = new string[Selection.objects.Length];
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                var o = Selection.objects[i];
                var assetPath = AssetDatabase.GetAssetPath(o);
                path[i] = assetPath;
            }
            return path;
        }

        private static string BuildPath(string[] selectionPath)
        {
            StringBuilder pathBuilder = new StringBuilder();
            var dataPath = Application.dataPath;
            dataPath = Path.GetDirectoryName(dataPath);
            for (int i = 0; i < selectionPath.Length; i++)
            {
                var path = selectionPath[i];
                var directoryName = Path.GetDirectoryName(path);
                if (directoryName.StartsWith(dataPath))
                {
                    pathBuilder.Append(path);
                }
                else
                {
                    pathBuilder.Append(string.Format("{0}/{1}", dataPath, path));
                }
                if (i < selectionPath.Length - 1)
                {
                    pathBuilder.Append("*");
                }
            }
            return pathBuilder.ToString();
        }

        private static string GetFullPath(Object o)
        {
            var assetPath = AssetDatabase.GetAssetPath(o);
            var fullPath = string.Format("{0}{1}", Application.dataPath, assetPath.Substring(6, assetPath.Length - 6));
            return fullPath;
        }

        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        static bool RunCmd(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    myPro.WaitForExit();
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        private static List<ITortoiseSVNSettingEditor> settingEditors = new List<ITortoiseSVNSettingEditor>() { new TortoiseSVNSetting(), new SubSVNSetting() };

        private static Vector2 scrollView;
#if EDITORPLUS
        public override void OnGUI()
#else
        public void OnGUI()
#endif
        {
            scrollView = GUILayout.BeginScrollView(scrollView);
            for (int i = 0; i < settingEditors.Count; i++)
            {
                var tortoiseSvnSettingEditor = settingEditors[i];
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                tortoiseSvnSettingEditor.ShowTortoiseSVNSetting();
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            }
            GUILayout.EndScrollView();
        }
    }

    public class SubSVNSetting : ITortoiseSVNSettingEditor
    {
        private static List<string> _Paths;

        public static List<string> Paths
        {
            get
            {
                if (_Paths == null)
                {
                    _Paths = new List<string>();
                    string path = Application.dataPath;
                    AllDir(path);
#if EDITORPLUS
                    var PackStorageDir = EditorUserSettings.GetConfigValue(HomePage.packageKey);
                    var LocalLibDir = EditorUserSettings.GetConfigValue(HomePage.localKey);
                    AllDir(LocalLibDir);
                    AllDir(PackStorageDir);
#endif
                    _Paths.Add(Application.dataPath);
                }
                return _Paths;
            }
        }
        public void ShowTortoiseSVNSetting()
        {
            foreach (string path in Paths)
            {
                GUILayout.Label(path);
            }
            if (GUILayout.Button("扫描"))
            {
                _Paths = null;
            }
        }

        private static void AllDir(string dirRoot)
        {
            List<string> dirs =new List<string>(Directory.GetDirectories(dirRoot));

            string directoryName = "";
            foreach (var dir in dirs)
            {
                directoryName = Path.GetFileName(dir);
                if (directoryName == ".svn")
                {
                    _Paths.Add(dirRoot);
                }
                else
                {
                    AllDir(dir);
                }
            }
        }
        public int GetTortoiseSVNSettingOrder()
        {
            return 2;
        }
    }
    public class TortoiseSVNSetting : ITortoiseSVNSettingEditor
    {
        public static string TortoiseProcPath
        {
            get
            {
                if (!EditorPrefs.HasKey(TortoiseProcPathKey))
                {
                    EditorPrefs.SetString(TortoiseProcPathKey, "TortoiseProc.exe");
                }
                return EditorPrefs.GetString(TortoiseProcPathKey);
            }
            set
            {
                EditorPrefs.SetString(TortoiseProcPathKey, value);
            }
        }

        private static string TortoiseProcPathKey
        {
            get
            {
                var key = string.Format("{0}TortoiseProc.exe 目录", Application.dataPath);
                return key;
            }
        }

        public void ShowTortoiseSVNSetting()
        {
            GUILayout.Label("TortoiseProc.exe 目录");

            TortoiseProcPath = EditorGUILayout.TextField(TortoiseProcPath);
        }

        public int GetTortoiseSVNSettingOrder()
        {
            return 1;
        }

    }
}
