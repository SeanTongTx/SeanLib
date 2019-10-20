using UnityEngine;

namespace SeanLib.Core
{
    public class LoadPath
    {
        /// <summary>
        /// streamingAssets路径
        /// </summary>
        public static string streamingAssetsPath;
        /// <summary>
        /// persistentDataPath路径
        /// </summary>
        public static string saveLoadPath;

        public static string readLoadPath;
        static LoadPath()
        {
            saveLoadPath = Application.persistentDataPath + "/";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			streamingAssetsPath = "file://" + Application.streamingAssetsPath+ "/";
            readLoadPath = "file://" +Application.persistentDataPath+"/";
#elif UNITY_IPHONE || UNITY_IOS
			streamingAssetsPath = "file://"+ Application.streamingAssetsPath+ "/";
            readLoadPath ="file://"+ Application.persistentDataPath + "/";
#elif UNITY_ANDROID
            readLoadPath =  "jar:file://"  + Application.persistentDataPath + "/";
        streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets/";
#endif
        }
        /// <summary>
        /// 因为win平台下www读取需要使用反斜杠 所以要对路径进行替换
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string replace(string FilePath)
        { 
#if UNITY_EDITOR|| UNITY_STANDALONE_WIN
            FilePath= FilePath.Replace("/", @"\");
            return  FilePath;
#else
            return FilePath;
#endif
        }
        public static string replaceToUnity(string FilePath)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            FilePath = FilePath.Replace(@"\", "/");
            return FilePath;
#else
            return FilePath;
#endif
        }
    }
}