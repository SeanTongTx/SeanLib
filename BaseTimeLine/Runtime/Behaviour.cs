
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimeLine
{
    /// <summary>
    /// handle clip runing alone
    /// </summary>
    public class Behaviour : PlayableBehaviour
    {
        public enum InternalState
        {
            Create,
            Start,
            Play,
            Pause,
            Stop,
            Destroy
        }
        public class Context
        {
            public InternalState state = InternalState.Create;
            public Track track;
            public Mixer mixer;
            public Clip clip;
            public Behaviour behaviour;
            public PlayableDirector director;
            public UnityEngine.Object BindObject;
            #region FrameData
            /// <summary>
            /// this countting is not real frameindex
            /// </summary>
            public int frameCount;
            public float weight;
            public double localTime;
            public T Clip<T>()where T :Clip
            {
                return clip as T;
            }
            #endregion
        }
        public Context context = new Context();

        public virtual void SetContext(Clip clip)
        {

        }
        #region Sequnence
        public virtual bool Await { get { return context.clip.Await; } }
        #endregion

        //Start->Pause Create->Pause
        //Play->Pause
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (context.state == InternalState.Play)
            {
                PauseOnPlaying(playable, info);
            }
            else if (context.state == InternalState.Create || context.state == InternalState.Start)
            {
                PauseOnStart(playable, info);
            }
            Event.Instace.OnClipPause.Dispatch(this.context);
            context.state = InternalState.Pause;
        }
        public virtual void PauseOnStart(Playable playable, FrameData info)
        {
        }
        public virtual void PauseOnPlaying(Playable playable, FrameData info)
        {
            if(Await&&IsLate)
            {
                Event.Instace.OnAwait.Dispatch(this.context);
            }
        }
        public override void OnGraphStart(Playable playable)
        {
            context.state = InternalState.Start;
        }
        public override void OnGraphStop(Playable playable)
        {
            context.state = InternalState.Stop;
        }
        public override void OnPlayableCreate(Playable playable)
        {
            context.state = InternalState.Create;
        }
        public override void OnPlayableDestroy(Playable playable)
        {
            context.state = InternalState.Destroy;
        }
        public virtual void OnSecoundFrame(Playable playable, FrameData info, object playerData)
        {
        }
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Event.Instace.OnClipPlay.Dispatch(this.context);
            context.state = InternalState.Play;
            context.frameCount = 0;
        }
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            context.frameCount++;
            context.weight = info.weight;
            if (context.frameCount == 1)
            {
                context.mixer.SetMixContext(context);
            }
            context.localTime = playable.GetTime();
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            context.BindObject = playerData as UnityEngine.Object;
            if (context.frameCount == 2)
            {
                OnSecoundFrame(playable, info, playerData);
                return;
            }
        }
        public bool IsEarly
        {
            get { return context.director == null ? true:context.director.time < context.clip.clipInfo.start; }
        }
        public bool IsLate
        {
            get { return context.director==null?false:context.director.time > context.clip.clipInfo.end; }
        }
        /// <summary>
        /// return 0-1 time
        /// </summary>
        public float Time01
        {
            get { return (float)(context.localTime / context.clip.clipInfo.duration); }
        }
        public virtual bool IsTimeOut()
        {
            return IsLate || IsEarly;
        }
    }
}
