using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimeLine
{
    /// <summary>
    /// handle mulitibehaviour mix
    /// this is singleton instance for one track
    /// </summary>
    /// Editor行为
    /// 开preview(选择director对象时自动打开) [m]create-create-[m]start-[m]play-[m]prepare-start-play-prepare
    /// 关preview(取消director对象时自动关闭) [m]pause-pause-[m]stop-[m]destroy-stop-destroy
    /// 
    /// 设置Timeline 时间点
    /// [m]prepare-prepare-[m]pf-pf
    /// 
    ///  播放
    ///  如果之前停止过-即stop
    /// [m]start-[m]play-start-play-"[m]prepare-prepare-[m]process-process"
    /// 否则
    /// "[m]prepare-prepare-[m]process-process"
    /// 停止
    /// [m]pause-[m]stop-pause-stop
    /// 
    /// 编辑器没有暂停按钮 只有start 和Stop
    /// 每次显示timeline实例就start 然后pause到游标位置。之后拖动都是持续pause状态只设置时间点
    /// 通过播放(space) 从pause状态 到stop
    /// 之后无论是拖动还是播放都会再次start
    /// 
    /// --internal state
    /// create-start-pause-stop-destory
    /// 
    public class Mixer : Behaviour
    {
        public Context clipA = null;
        public Context clipB = null;
        public int lastInputCount = 0;
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (clipA != null)
            {
                clipA = clipA.behaviour.IsTimeOut() ? null : clipA;
            }
            if (clipB != null)
            {
                clipB = clipB.behaviour.IsTimeOut() ? null : clipB;
            }
            if(clipA==null&&clipB!=null)
            {
                clipA = clipB;
                clipB = null;
            }
        }
        public override bool IsTimeOut()
        {
            return false;
        }
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            context.state = InternalState.Pause;
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            context.BindObject = playerData as UnityEngine.Object;
            if (clipA == null)
            {
                if(lastInputCount!=0)
                {
                    InputChanged(lastInputCount, 0);
                }
                ProcessZero(playable, info, playerData);
                lastInputCount = 0;
            }
            else if (clipB == null)
            {
                if (lastInputCount != 1)
                {
                    InputChanged(lastInputCount, 1);
                }
                ProcessOne(playable, info, playerData);
                lastInputCount = 1;
            }
            else if (clipB != null)
            {
                if (lastInputCount != 2)
                {
                    InputChanged(lastInputCount, 2);
                }
                ProcessTwo(playable, info, playerData);
                lastInputCount = 2;
            }
        }
        public virtual void ProcessZero(Playable playable, FrameData info, object playerData)
        {
        }
        public virtual void ProcessOne(Playable playable, FrameData info, object playerData)
        {
        }
        public virtual void ProcessTwo(Playable playable, FrameData info, object playerData)
        {
        }
        public virtual void InputChanged(int from,int to)
        {
        }
        public void SetMixContext(Context context)
        {

            if (clipA == null)
            {
                clipA = context;
            }
            else if (clipB == null)
            {
                clipB = context;
            }
            //解决3clip合并帧
            if (clipA != null && clipB != null)
            {
                if (clipA != context && clipB != context)
                {
                    clipA = clipB;
                    clipB = context;
                }
            }
        }
        public Context MaxWeight()
        {
            if (clipA != null)
            {
                if (clipB == null) return clipA;
                else
                {
                    return clipA.weight > clipB.weight ? clipA : clipB;
                }
            }
            else if (clipB != null)
            {
                return clipB;
            }
            else return null;
           
        }
    }
}
