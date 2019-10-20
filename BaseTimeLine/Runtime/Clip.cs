using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace TimeLine
{
    [Serializable]
    public class Clip :PlayableAsset, ITimelineClipAsset, IPropertyPreview
    {
        [Header("Sequnence")]
        [InspectorPlus.Line]
        public bool Await;

        //Cache
        [HideInInspector, NonSerialized]
        public TimelineClip clipInfo;
        [HideInInspector, NonSerialized]
        public Mixer mixer;
        [HideInInspector, NonSerialized]
        public Behaviour behaviour;


        public virtual ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.All;
            }
        }
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var scriptPlayable = ScriptPlayable<Behaviour>.Create(graph);
            Behaviour behaviour = scriptPlayable.GetBehaviour();
            SetContext(behaviour);
            return scriptPlayable;
        }
        public virtual void SetContext(Behaviour behaviour)
        {
            this.behaviour = behaviour;
            behaviour.context.behaviour = behaviour;
            behaviour.context.clip = this;
            behaviour.context.mixer = mixer;
            behaviour.context.track = mixer.context.track;
            behaviour.context.director = mixer.context.director;
            behaviour.context.BindObject = mixer.context.BindObject;
            behaviour.SetContext(this);
        }


        public virtual void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
#endif
        }
    }
}
