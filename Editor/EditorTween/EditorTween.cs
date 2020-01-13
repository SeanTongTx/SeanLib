
using EditorPlus;
using System;
using System.Collections;
using UnityEngine;
namespace EditorPlus
{
    public class EditorTween
    {
        public class TweenHandle : EditorCoroutine
        {
            public Action OnStop;
            public static TweenHandle Start(IEnumerator _routine,Action onStop=null)
            {
                TweenHandle coroutine = new TweenHandle(_routine);
                coroutine.OnStop = onStop;
                coroutine.Start();
                return coroutine;
            }
            protected TweenHandle(IEnumerator _routine) : base(_routine)
            {
            }
            public override void Stop()
            {
                OnStop?.Invoke();
                base.Stop();
            }
        }
        public static TweenHandle Tween(Action<float> evaluate, float time, string curveName)
        {
           return TweenHandle.Start(StartTween(evaluate, EditorTweenCurve.Get(curveName), time));
        }
        public static TweenHandle Tween(Action<float> evaluate, float time, EditorTweenCurve.BuiltinCurve curve)
        {
           return TweenHandle.Start(StartTween(evaluate, EditorTweenCurve.Get(curve), time));
        }
        private static IEnumerator StartTween(Action<float> evaluate, EditorTweenCurve.TweenCurve curve, float time)
        {
            var waitTime = new EditorYields.WaitForSeconds(time);
            while (true)
            {
                if (waitTime.keepWaiting)
                {
                    var timer = Time.realtimeSinceStartup - waitTime.StartWaitTime;
                    evaluate(curve.curve.Evaluate(timer / time));
                    yield return null;
                }
                else
                {
                    evaluate(curve.curve.Evaluate(1));
                    yield break;
                }
            }
        }
    }
}