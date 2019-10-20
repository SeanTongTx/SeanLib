using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestAssetDatabase
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestAssetDB()
        {
          var assets=  AssetDatabase.FindAssets("",new string[] { "Packages/" });

            Debug.Log("AssetDatabase.FindAssets:-----------");
            foreach (var item in assets)
            {
                Debug.Log(AssetDatabase.GUIDToAssetPath(item));
            }
        }
        [Test]
        public void TestLoadAsset()
        {
          var obj=  AssetDatabase.LoadAssetAtPath("Packages/com.seanlib.protocolcore/Editor/ProtocolDoc.cs", typeof(UnityEngine.Object));
            Debug.Log(obj);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestPackages()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            var request = Client.List();
            yield return new WaitUntil(() => { return request.IsCompleted; });
           var collection= request.Result;
            foreach (var item in collection)
            {
                Debug.Log(item.name+"   |   "+item.resolvedPath+"   |   "+ item.assetPath);
            }
        }
    }
}
