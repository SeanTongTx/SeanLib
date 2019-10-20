using System;
using System.Collections;
using UnityEngine;

namespace SeanLib.Core
{

    public class CoroutineCall : MonoBehaviour
    {
        public Coroutine coroutine;
        /// <summary>
        /// 可以运行一段协同代码
        /// </summary>
        /// <param name="a">协同方法</param>
        public static CoroutineCall Call(Func<IEnumerator> a)
        {
            CoroutineCall coroutineCall = ParasiticComponent.parasiteHost.AddComponent<CoroutineCall>();
            coroutineCall.a = a;
            coroutineCall.coroutine = coroutineCall.StartCoroutine(coroutineCall.CallCoroutine());
            return coroutineCall;
        }

        private Func<IEnumerator> a;
        IEnumerator CallCoroutine()
        {
            yield return StartCoroutine(a());
            coroutine = null;
            Destroy(this);
        }
    }
}
