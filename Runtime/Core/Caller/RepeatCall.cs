using System;
using UnityEngine;

namespace SeanLib.Core
{
    /// <summary>
    /// 定时调用
    /// </summary>
    public class RepeatCall : MonoBehaviour
    {
        /// <summary>
        /// 重复调用，直到被调用方法返回false
        /// </summary>
        /// <param name="a">被调用方法</param>
        /// <param name="delay">第一次调用延时时间</param>
        /// <param name="repateRate">开始调用后的间隔</param>
        public static RepeatCall Call(Func<bool> a, float delay, float repateRate)
        {
            var addComponent = ParasiticComponent.parasiteHost.AddComponent<RepeatCall>();
            addComponent.CallAction(a, delay, repateRate);
            return addComponent;
        }

        private Func<bool> _delayCall;
        /// <summary>
        /// 开始执行重复调用，直到被调用方法返回false
        /// </summary>
        /// <param name="a">被调用方法</param>
        /// <param name="delay">第一次调用延时时间</param>
        /// <param name="repateRate">开始调用后的间隔</param>
        public void CallAction(Func<bool> a, float delay, float repateRate)
        {
            _delayCall = a;
            InvokeRepeating("CallBack", delay, repateRate);
        }
        /// <summary>
        /// 停止销毁
        /// </summary>
        public void Stop()
        {
            CancelInvoke("CallBack");
            Destroy(this);
        }

        private void CallBack()
        {
            if (_delayCall==null || !_delayCall())
            {
                CancelInvoke("CallBack");
                Destroy(this);
            }
        }

    }
}
