using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineTemplate
{
    public class TrackTemplate : CodeTemplate
    {
        public override string FilePath
        {
            get
            {
                return "NEW_TRACK_NAME.cs";
            }
        }

        public override string Template
        {
            get
            {
                return @"
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
[TrackClipType(typeof(NEW_CLIP_NAME))]
public class NEW_TRACK_NAME : BASE_TRACK_NAME
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        ScriptPlayable<NEW_MIXER_NAME> mixer = ScriptPlayable<NEW_MIXER_NAME>.Create(graph, inputCount);
        NEW_MIXER_NAME behaviour = mixer.GetBehaviour();
        SetContext(behaviour, go);
        return mixer;
    }
}";
            }
        }
        string[] keys = new string[] { "NEW_CLIP_NAME", "NEW_TRACK_NAME", "BASE_TRACK_NAME", "NEW_MIXER_NAME" };

        public TrackTemplate()
        {
        }

        public TrackTemplate(string TemplateName) : base(TemplateName)
        {
        }

        public override string[] KeyWords
        {
            get
            {
                return keys;
            }
        }
    }
    public class MixerTemplate : CodeTemplate
    {
        public override string FilePath
        {
            get
            {
                return "NEW_MIXER_NAME.cs";
            }
        }

        public override string Template
        {
            get
            {
                return @"
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
public class NEW_MIXER_NAME : BASE_MIXER_NAME
{
}";
            }
        }
        string[] keys = new string[] { "NEW_MIXER_NAME", "BASE_MIXER_NAME" };

        public MixerTemplate()
        {
        }

        public MixerTemplate(string TemplateName) : base(TemplateName)
        {
        }

        public override string[] KeyWords
        {
            get
            {
                return keys;
            }
        }
    }
    public class ClipTemplate : CodeTemplate
    {
        public override string FilePath
        {
            get
            {
                return  "NEW_CLIP_NAME.cs";
            }
        }

        public override string Template
        {
            get
            {
                return @"
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
[Serializable]
public class NEW_CLIP_NAME:BASECLIPNAME
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var scriptPlayable = ScriptPlayable<NEW_BEHAVIOURS_NAME>.Create(graph);
        NEW_BEHAVIOURS_NAME behaviour = scriptPlayable.GetBehaviour();
        SetContext(behaviour);
        return scriptPlayable;
    }
}";
            }
        }
        string[] keys = new string[] { "NEW_CLIP_NAME", "BASECLIPNAME","NEW_BEHAVIOURS_NAME" };

        public ClipTemplate()
        {
        }

        public ClipTemplate(string TemplateName) : base(TemplateName)
        {
        }

        public override string[] KeyWords
        {
            get
            {
                return keys;
            }
        }
    }
    public class BehaviourTemplate : CodeTemplate
    {
        public override string FilePath
        {
            get
            {
                return "NEW_BEHAVIOURS_NAME.cs";
            }
        }

        public override string Template
        {
            get
            {
                return @"
using UnityEngine;
using TimeLine;
using UnityEngine.Timeline;
using System;
using UnityEngine.Playables;
public class NEW_BEHAVIOURS_NAME :BASE_BEHAVIOUR_NAME
{
    public NEW_CLIP_NAME Clip { get { return this.context.clip as NEW_CLIP_NAME; } }
    public NEW_MIXER_NAME Mixer { get { return this.context.mixer as NEW_MIXER_NAME; } }
    public NEW_TRACK_NAME Track { get { return this.context.track as NEW_TRACK_NAME; } }
}";
            }
        }
        string[] keys = new string[] { "NEW_BEHAVIOURS_NAME", "BASE_BEHAVIOUR_NAME", "NEW_CLIP_NAME",
            "NEW_MIXER_NAME","NEW_MIXER_NAME","NEW_TRACK_NAME" };

        public BehaviourTemplate()
        {
        }

        public BehaviourTemplate(string TemplateName) : base(TemplateName)
        {
        }

        public override string[] KeyWords
        {
            get
            {
                return keys;
            }
        }
    }
}
