using System;
using UnityEngine;

namespace SeanLib.Core
{
    /// <summary>
    /// 如果被调用了，则在一段时间内无法调用第二次
    /// </summary>
    public class OnceCall : MonoBehaviour
    {
        /// <summary>
        /// 在一段时间内无法调用第二次
        /// </summary>
        /// <param name="a">回调方法</param>
        /// <param name="delay">间隔时间</param>
        /// <returns></returns>
        public static OnceCall Create(Action a, float delay)
        {
            var onceCall = ParasiticComponent.parasiteHost.AddComponent<OnceCall>();
            onceCall.fun = a;
            onceCall.delay = delay;
            return onceCall;
        }
        /// <summary>
        /// 间隔时间
        /// </summary>
        public float delay = 1;
        /// <summary>
        /// 回调
        /// </summary>
        public Action fun;
        private bool _tag = false;
        public void Call()
        {
            if (_tag == false)
            {
                _tag = true;
                if (fun != null) fun();
                Invoke("reset", delay);
            }
        }

        private void reset()
        {
            _tag = false;
        }
    }


}
