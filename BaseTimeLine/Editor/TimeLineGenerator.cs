using EditorPlus;
using ServiceTools.Reflect;
using System;
using System.Collections;
using System.Collections.Generic;
using TimeLine;
using UnityEditor;
using UnityEngine;

[CustomSeanLibEditor("BaseTimeline/TimeLineGenerator")]
public class TimeLineGenerator : CodeGenerator
{
    TimeLineTemplate.TrackTemplate TrackTemplate = new TimeLineTemplate.TrackTemplate("Track");
    TimeLineTemplate.MixerTemplate MixerTemplate = new TimeLineTemplate.MixerTemplate("Mixer");
    TimeLineTemplate.BehaviourTemplate BehaviourTemplate = new TimeLineTemplate.BehaviourTemplate("Behaviour");
    TimeLineTemplate.ClipTemplate ClipTemplate = new TimeLineTemplate.ClipTemplate("Clip");


    bool ExistTrack;
    List<Type> trackTypes;
    List<Type> mixerTypes;
    List<Type> clipTypes;
    List<Type> behaviourTypes;
    OnGUIUtility.FadeGroup trackGroup = new OnGUIUtility.FadeGroup();
    OnGUIUtility.FadeGroup mixerGroup = new OnGUIUtility.FadeGroup();
    OnGUIUtility.FadeGroup clipGroup = new OnGUIUtility.FadeGroup();
    OnGUIUtility.FadeGroup behaviourGroup = new OnGUIUtility.FadeGroup();
    OnGUIUtility.Select<Type> Select_ExistTrack = new OnGUIUtility.Select<Type>();
    OnGUIUtility.Select<Type> Select_BaseTrack = new OnGUIUtility.Select<Type>();

    OnGUIUtility.Select<Type> Select_ExistMixer = new OnGUIUtility.Select<Type>();
    OnGUIUtility.Select<Type> Select_BaseMixer = new OnGUIUtility.Select<Type>();
    OnGUIUtility.Select<Type> Select_BaseClip = new OnGUIUtility.Select<Type>();
    OnGUIUtility.Select<Type> Select_BaseBehaviour = new OnGUIUtility.Select<Type>();

    public override void OnEnable(SeanLibWindow drawer)
    {
        this.templates.Add(TrackTemplate);
        this.templates.Add(MixerTemplate);
        this.templates.Add(BehaviourTemplate);
        this.templates.Add(ClipTemplate);
        base.OnEnable(drawer);
        trackTypes = AssemblyTool.FindTypesInCurrentDomainWhere((type) =>
        {
            return type == typeof(Track) || type.IsSubclassOf(typeof(Track));
        });
        mixerTypes = AssemblyTool.FindTypesInCurrentDomainWhere((type) =>
        {
            return type == typeof(Mixer) || type.IsSubclassOf(typeof(Mixer));
        });
        clipTypes = AssemblyTool.FindTypesInCurrentDomainWhere((type) =>
        {
            return type == typeof(Clip) || type.IsSubclassOf(typeof(Clip));
        });
        behaviourTypes = AssemblyTool.FindTypesInCurrentDomainWhere((type) =>
        {
            return type == typeof(TimeLine.Behaviour) || type.IsSubclassOf(typeof(TimeLine.Behaviour));
        });
        trackGroup.OnEnable(window.Repaint, false);
        mixerGroup.OnEnable(window.Repaint, false);
        clipGroup.OnEnable(window.Repaint, false);
        behaviourGroup.OnEnable(window.Repaint, false);
    }
    public override void OnDraw()
    {
        ExistTrack = EditorGUILayout.ToggleLeft("Use ExistTrack", ExistTrack);
        if (trackGroup.OnGuiBegin("Track", styles.Title))
        {
            OnGUIUtility.Layout.IndentBegin();
            if (ExistTrack)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(this.KeyValues["NEW_TRACK_NAME"]);
                if (Select_ExistTrack.OnGUI(trackTypes))
                {
                    this.KeyValues["NEW_TRACK_NAME"] = Select_ExistTrack.t.FullName;
                }
                EditorGUILayout.EndHorizontal();
                KeyValues["BASE_TRACK_NAME"] = "1";
            }
            else
            {
                this.KeyValues["NEW_TRACK_NAME"] = EditorGUILayout.TextField("Track", this.KeyValues["NEW_TRACK_NAME"]);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("BaseTrack", KeyValues["BASE_TRACK_NAME"]);
                if (Select_BaseTrack.OnGUI(trackTypes))
                {
                    KeyValues["BASE_TRACK_NAME"] = Select_BaseTrack.t.FullName;
                }
                EditorGUILayout.EndHorizontal();
            }
            OnGUIUtility.Layout.IndentEnd();
        }
        trackGroup.OnGuiEnd();
        if (mixerGroup.OnGuiBegin("Mixer", styles.Title))
        {
            if (ExistTrack)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(this.KeyValues["NEW_MIXER_NAME"]);
                if (Select_ExistMixer.OnGUI(mixerTypes))
                {
                    this.KeyValues["NEW_MIXER_NAME"] = Select_ExistMixer.t.FullName;
                }
                EditorGUILayout.EndHorizontal();
                KeyValues["BASE_MIXER_NAME"] = "1";
            }
            else
            {
                KeyValues["NEW_MIXER_NAME"] = EditorGUILayout.TextField("Mixer", KeyValues["NEW_MIXER_NAME"]);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("BaseMixer", KeyValues["BASE_MIXER_NAME"]);
                if (Select_BaseMixer.OnGUI(mixerTypes))
                {
                    KeyValues["BASE_MIXER_NAME"] = Select_BaseMixer.t.FullName;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        mixerGroup.OnGuiEnd();
        if (clipGroup.OnGuiBegin("Clip", styles.Title))
        {
            KeyValues["NEW_CLIP_NAME"] = EditorGUILayout.TextField("Clip", KeyValues["NEW_CLIP_NAME"]);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BaseClip", KeyValues["BASECLIPNAME"]);
            if (Select_BaseClip.OnGUI(clipTypes))
            {
                KeyValues["BASECLIPNAME"] = Select_BaseClip.t.FullName;
            }
            EditorGUILayout.EndHorizontal();
        }
        clipGroup.OnGuiEnd();
        if (behaviourGroup.OnGuiBegin("Behaviour", styles.Title))
        {
            KeyValues["NEW_BEHAVIOURS_NAME"] = EditorGUILayout.TextField("Behaviour", KeyValues["NEW_BEHAVIOURS_NAME"]);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BaseClip", KeyValues["BASE_BEHAVIOUR_NAME"]);
            if (Select_BaseBehaviour.OnGUI(behaviourTypes))
            {
                KeyValues["BASE_BEHAVIOUR_NAME"] = Select_BaseBehaviour.t.FullName;
            }
            EditorGUILayout.EndHorizontal();
        }
        behaviourGroup.OnGuiEnd();
    }
    public override void OnGenerate()
    {
        if(ExistTrack)
        {
            templates.Remove(TrackTemplate);
            templates.Remove(MixerTemplate);
        }
        base.OnGenerate();
    }
    public override void OnDisable()
    {
        trackGroup.OnDisable(window.Repaint);
        mixerGroup.OnDisable(window.Repaint);
        clipGroup.OnDisable(window.Repaint);
        behaviourGroup.OnDisable(window.Repaint);
        base.OnDisable();
    }
}
