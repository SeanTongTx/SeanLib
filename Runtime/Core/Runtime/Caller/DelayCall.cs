using System;
using UnityEngine;

namespace SeanLib.Core
{
    /// <summary>
    /// 延时调用
    /// </summary>
    public class DelayCall : MonoBehaviour
    {
        /// <summary>
        /// 延时调用
        /// </summary>
        /// <param name="a">回调</param>
        /// <param name="delay">调用延时</param>
        /// <returns></returns>
        public static DelayCall Call(Action a, float delay = 1)
        {
            var addComponent = ParasiticComponent.parasiteHost.AddComponent<DelayCall>();
            addComponent.delay = delay;
            addComponent.fun = a;
            addComponent.StartDelayCall();
            return addComponent;
        }
        /// <summary>
        /// 延时
        /// </summary>
        public float delay = 1;
        /// <summary>
        /// 调用完成后自动销毁
        /// </summary>
        public bool destroyAfterCall = true;
        /// <summary>
        /// 回调
        /// </summary>
        public Action fun;

        private bool _started=false;

        /// <summary>
        /// 停止这个调用
        /// </summary>
        public void Stop()
        {
            CancelInvoke("CallBack");
        }
        /// <summary>
        /// 执行回调
        /// </summary>
        private  void StartDelayCall()
        {
            if (!_started)
            {
                Invoke("CallBack", delay);
                _started = false;
            }
        }
        /// <summary>
        /// 回调
        /// </summary>
        private void CallBack()
        {
            if (fun != null) fun();
            _started = true;
            if (destroyAfterCall) Destroy(this);
        }
    }
}
