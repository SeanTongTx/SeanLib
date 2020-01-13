
using EditorPlus;
using SeanLib.Core;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SeanLib.CodeTemplate
{
   [CustomSeanLibEditor("EditorPlus/EditorCoroutineDemo")]
    public class EditorCoroutineDemo : SeanLibEditor
    {
        protected override bool UseIMGUI => true;

        float count;
        bool countend;
        public override void OnGUI()
        {
            base.OnGUI();
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("定时器，定时10秒，每0.5秒一次回调");
                    GUILayout.Label(count.ToString());
                    if (GUILayout.Button("Start"))
                    {
                        EditorCoroutine.Start(Timer());
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("等待 定时器 >5");
                    if(GUILayout.Button("Start"))
                    {
                        EditorCoroutine.Start(waitCounting());
                    }
                    GUILayout.Label(!countend ? "等待中.." : "✓");
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("等1帧");
                    if(GUILayout.Button("Start"))
                    {
                        EditorCoroutine.Start(waitForNextFrame());
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            if(GUILayout.Button("整合两个携程的序列"))
            {
                EditorCoroutine.Start(sequnce());
            }
            OnGUIUtility.ScriptField("Script", this.GetType());
        }

        private IEnumerator waitForNextFrame()
        {
            yield return null;
            Debug.Log("等1帧结束");
        }

        private IEnumerator sequnce()
        {
            Debug.Log("序列开始");
            Debug.Log("开始时钟");
            EditorCoroutine.Start(Timer());
            Debug.Log("开始计数");
            yield return EditorCoroutine.Start(waitCounting()).ToYield();
            Debug.Log("计时结束");
            yield return EditorCoroutine.Start(waitForNextFrame()).ToYield();
            Debug.Log("结束");
        }

        private IEnumerator waitCounting()
        {
            countend = false;
            yield return new EditorYields.General(() => { return count > 3; });
            countend = true;
        }
        private IEnumerator Timer()
        {
            count = 0;
            while (count < 10)
            {
                count+=.5f;
                if(this.window)
                this.window.Repaint(); 
                yield return new EditorYields.WaitForSeconds(0.5f);
            }
        }
    }
}