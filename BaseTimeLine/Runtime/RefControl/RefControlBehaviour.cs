
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
namespace BaseTimeLine
{
    public class RefControlBehaviour : TimeLine.Behaviour
    {
        public RefControlClip Clip { get { return this.context.clip as RefControlClip; } }
        public RefControlMixer Mixer { get { return this.context.mixer as RefControlMixer; } }
        public RefControlTrack Track { get { return this.context.track as RefControlTrack; } }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            foreach (var item in Clip.Items)
            {
                item.apply = false;
            }
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            float time01 = Time01;
            if (context.weight > 0)
            {
                foreach (var item in Clip.Items)
                {
                    if (time01 >= item.LifeTime.start && time01 <= item.LifeTime.end)
                    {
                        if (!item.apply)
                        {
                            ControlObject(item, item.On);
                            item.apply = true;
                        }
                    }
                    else
                    {
                        if (item.apply)
                        {
                            ControlObject(item, !item.On);
                            item.apply = false;
                        }
                    }
                }
            }
        }
        public override void PauseOnPlaying(Playable playable, FrameData info)
        {
            base.PauseOnPlaying(playable, info);

            foreach (var item in Clip.Items)
            {
                if (item.apply && item.LifeTime.end <= 1)
                {
                    ControlObject(item, !item.On);
                    item.apply = false;
                }
            }
        }

        private void ControlObject(RefControlClip.ControlItem item, bool On)
        {
            var refObj = item.Reference.Resolve();
            if (refObj)
            {
                item.apply = true;
                var go = refObj as GameObject;
                var pos = item.Postion.Resolve();
                if (go)
                {
                    go.SetActive(On);
                    if (pos)
                    {
                        go.transform.position = (pos as GameObject).transform.position;
                        go.transform.rotation = (pos as GameObject).transform.rotation;
                        if (Application.isPlaying&&item.SetAsChild)
                        {
                            go.transform.SetParentLocalScale((pos as GameObject).transform);
                        }
                    }
                }
                else
                {
                    var component = (refObj as Component);
                    component.gameObject.SetActive(On);
                    if (component is ParticleSystem)
                    {
                        if (On) { (component as ParticleSystem).Play(); }
                        else (component as ParticleSystem).Stop();
                    }
                    else if (component is PlayableDirector)
                    {
                        if (On) { (component as PlayableDirector).Play(); }
                        else (component as PlayableDirector).Stop();
                    }
                    if (pos)
                    {
                        component.transform.position = (pos as GameObject).transform.position;
                        component.transform.rotation = (pos as GameObject).transform.rotation;
                        if (Application.isPlaying && item.SetAsChild)
                        {
                            component.transform.SetParentLocalScale((pos as GameObject).transform);
                        }
                    }
                }

            }
        }
    }
}