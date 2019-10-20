
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
namespace BaseTimeLine
{
    [TrackClipType(typeof(RefControlClip))]
    public class RefControlTrack : TimeLine.Track
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<RefControlMixer> mixer = ScriptPlayable<RefControlMixer>.Create(graph, inputCount);
            RefControlMixer behaviour = mixer.GetBehaviour();
            SetContext(behaviour, go);
            return mixer;
        }
    }
}