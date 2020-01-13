
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EditorPlus
{
    public class EditorYields
    {
        /// <summary>
        /// 在编辑器中使用scaled time
        /// </summary>
        public class WaitForSeconds : CustomYieldInstruction
        {
            public float StartWaitTime = -1f;
            private float m_waitTime;
            public float scaledWaitTime => m_waitTime / Time.timeScale;
            public WaitForSeconds(float time)
            {
                StartWaitTime = Time.realtimeSinceStartup;
                m_waitTime = time;
            }
            public override bool keepWaiting
            {
                get
                {
                    bool flag = Time.realtimeSinceStartup < (StartWaitTime + this.scaledWaitTime);
                    if (!flag)
                    {
                        this.StartWaitTime = -1f;
                    }
                    return flag;
                }
            }
        }
        public class General : CustomYieldInstruction
        {
            private Predicate check;
            public General(Predicate predicate)
            {
                this.check = predicate;
            }
            public override bool keepWaiting => !check();
        }
        public class WaitForCoroutine : CustomYieldInstruction
        {
            EditorCoroutine coroutine;

            public WaitForCoroutine(EditorCoroutine coroutine)
            {
                this.coroutine = coroutine;
            }

            public override bool keepWaiting => coroutine.InRoutine;
        }
    }
    public delegate bool Predicate();
    public static class EditorYieldsHelper
    {
        public static IEnumerator ToYield(this Predicate func)
        {
            return new EditorYields.General(func);
        }
        public static IEnumerator ToYield(this EditorCoroutine coroutine)
        {
            return new EditorYields.WaitForCoroutine(coroutine);
        }
    }
}