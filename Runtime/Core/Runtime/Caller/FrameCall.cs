using System;
using UnityEngine;

namespace SeanLib.Core
{
    /// <summary>
    /// 和帧有关的调用
    /// </summary>
    public class FrameCall : MonoBehaviour
    {
        /// <summary>
        /// 延迟指定帧数后调用
        /// </summary>
        /// <param name="a">回调</param>
        /// <param name="delayFrame">延迟帧数</param>
        public static FrameCall DelayFrame(Action a, int delayFrame)
        {
            int currFrame = 0;
            FrameCall addComponent = ParasiticComponent.parasiteHost.AddComponent<FrameCall>();
            addComponent.CallAction(() =>
            {
                bool b = ++currFrame < delayFrame;
                if (!b)
                {
                    a();
                }
                return b;
            });
            return addComponent;
        }
        /// <summary>
        /// 下一帧调用
        /// </summary>
        /// <param name="a">回调</param>
        public static FrameCall DelayFrame(Action a)
        {
            FrameCall addComponent = ParasiticComponent.parasiteHost.AddComponent<FrameCall>();
            addComponent.CallAction(() =>
            {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    DebugConsole.Log(e) ;
                }
                return false;
            });
            return addComponent;
        }

        

        /// <summary>
        /// 每一帧都调用，直到返回false
        /// </summary>
        /// <param name="a"></param>
        public static FrameCall Call(Func<bool> a)
        {
            var frameCall = ParasiticComponent.parasiteHost.AddComponent<FrameCall>();
            frameCall.CallAction(a);
            return frameCall;
        }

        private Func<bool> _delayCall;
        private void CallAction(Func<bool> a)
        {
            _delayCall = a;
        }
        /// <summary>
        /// 运行一次
        /// </summary>
        public void Run()
        {
            Update();
        }
        
        void Update()
        {
            if (_delayCall != null)
            {
                if (!_delayCall())
                {
                    _delayCall = null;
                    Destroy(this);
                }
            }
        }
    }
}
