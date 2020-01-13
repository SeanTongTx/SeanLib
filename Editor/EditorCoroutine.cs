using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace EditorPlus
{
    public class EditorCoroutine
    {
        public static EditorCoroutine Start(IEnumerator _routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_routine);
            coroutine.Start();
            return coroutine;
        }

        private IEnumerator routine;
        public bool InRoutine => routine != null;
        protected EditorCoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }

        protected virtual void Start()
        {
            //Debug.Log("start");
            EditorApplication.update += update;
        }

        public virtual void Stop()
        {
            //Debug.Log("stop");
            EditorApplication.update -= update;
            routine = null;
        }

        protected virtual void update()
        {
            /* NOTE: no need to try/catch MoveNext,
             * if an IEnumerator throws its next iteration returns false.
             * Also, Unity probably catches when calling EditorApplication.update.
             */
            //Debug.Log("update");
            CustomYieldInstruction yield = routine.Current as CustomYieldInstruction;
            if (yield!=null)
            {
                if (yield.keepWaiting)
                {
                    return;
                }
                else
                {
                    if (!routine.MoveNext())
                    {
                        Stop();
                    }
                }
            }
            else
            {
                if (!routine.MoveNext())
                {
                    Stop();
                }
            }
        }
    }
}