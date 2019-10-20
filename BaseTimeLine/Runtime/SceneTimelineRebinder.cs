using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteInEditMode]
[RequireComponent(typeof(PlayableDirector))]
public class SceneTimelineRebinder : MonoBehaviour
{
    [Serializable]
    public class Rebingding
    {
        [InspectorPlus.ReadOnly]
        public UnityEngine.Object Key;
        [InspectorPlus.SceneRefAtt(ShowFieldStr =false,ShowRefType =true, ShowDetail=true)]
        public SceneReference Value;
    }
    [SerializeField,HideInInspector]
    public List<Rebingding> rebinding = new List<Rebingding>();
    [SerializeField]
    public bool PlayAfterBind;
    private PlayableDirector _playableDirector;
    public PlayableDirector Director
    {
        get
        {
            if (!_playableDirector) _playableDirector = GetComponent<PlayableDirector>();
            return _playableDirector;
        }
    }

    private void Start()
    {
        if(!Application.isPlaying)
        {
            Rebind();
        }
    }

    //this Should call in editor
    [InspectorPlus.Button(Editor =true,Title ="索引Timeline轨道")]
    public void ReKey()
    {
        List<SceneTimelineRebinder.Rebingding> newrebingdings = new List<SceneTimelineRebinder.Rebingding>();
        foreach (var item in Director.playableAsset.outputs)
        {
            if (item.sourceObject == null) continue;
            SceneTimelineRebinder.Rebingding bind = rebinding.Find(e => e.Key!=null?e.Key.name == item.sourceObject.name:false);
            if (bind == null)
            {
                newrebingdings.Add(new SceneTimelineRebinder.Rebingding() { Key = item.sourceObject });
            }
            else
            {
                bind.Key = item.sourceObject;
                newrebingdings.Add(bind);
            }
        }
        rebinding = newrebingdings;
    }

    [InspectorPlus.Button(Editor = true,Title ="重绑定场景对象",Help ="自动重绑定，需要完善绑定列表。这个列表应该在编辑器编辑时就确定完整。")]
    public void Rebind()
    {
        foreach (var item in rebinding)
        {
            if(item.Value!=null)
            {
                var SceneValue = item.Value.Resolve();
                if (SceneValue)
                {
                    Director.SetGenericBinding(item.Key, SceneValue);
                }
            }
        }
        if(PlayAfterBind&&Application.isPlaying)
        {
            Director.Play();
        }
    }
    /// <summary>
    /// 理论上 应该在Rebind 自动重绑定之后使用。
    /// 程序手动绑定 轨道对象
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="bindObj"></param>
    public void RebindManually(string trackName, UnityEngine.Object bindObj)
    {
        var track = rebinding.Find(e => e.Key && e.Key.name == trackName);
        Director.SetGenericBinding(track.Key, bindObj);
    }
}
