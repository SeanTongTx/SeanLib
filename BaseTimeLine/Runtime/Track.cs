using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace TimeLine
{
    //[TrackClipType(typeof(Clip))]
    public abstract class Track : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<Mixer> mixer = ScriptPlayable<Mixer>.Create(graph, inputCount);
            Mixer behaviour = mixer.GetBehaviour();
            SetContext(behaviour,go);
            return mixer;
        }
        public virtual void SetContext(Mixer mixer,GameObject go)
        {
            mixer.context.track = this;
            mixer.context.director = go.GetComponent<PlayableDirector>();
            mixer.context.BindObject = GetBindingObject(mixer.context.director);
            var clips = this.GetClips();
            foreach (var clip in clips)
            {
                Clip c = clip.asset as Clip;
                c.clipInfo = clip;
                c.mixer = mixer; 
            }
        }
        public virtual UnityEngine.Object GetBindingObject(PlayableDirector director)
        {
           return director.GetGenericBinding(this);
        }
    }
}
