
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
using SeanLib.Core;
using System.Collections.Generic;
namespace BaseTimeLine
{
    [Serializable]
    public class RefControlClip : TimeLine.Clip
    {
        public override ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;

            }
        }

        [Serializable]
        public class ControlItem
        {
            [InspectorPlus.SceneRefAtt(ShowFieldStr = true, ShowRefType = true)]
            public SceneReference Reference = new SceneReference();
            public SceneReference Postion = new SceneReference();
            [InspectorPlus.Range(0, 1.1f)]
            public RangeFloat LifeTime = new RangeFloat(0, 1.1f);
            ///controllers
            [Tooltip("开启")]
            public bool On;
            [Tooltip("是否设置为目标的子节点")]
            public bool SetAsChild;
            [NonSerialized]
            public bool apply;
        }
        public List<ControlItem> Items = new List<ControlItem>();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var scriptPlayable = ScriptPlayable<RefControlBehaviour>.Create(graph);
            RefControlBehaviour behaviour = scriptPlayable.GetBehaviour();
            SetContext(behaviour);

            return scriptPlayable;
        }
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            base.GatherProperties(director, driver);
            foreach (var item in Items)
            {
                item.Reference.Resolve();
            }
#endif
        }
    }
}