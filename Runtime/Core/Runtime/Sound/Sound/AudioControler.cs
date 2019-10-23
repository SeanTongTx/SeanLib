using System;
using System.Collections.Generic;


using UnityEngine;
using System.Collections;
namespace SeanLib.Core
{
    public class AudioControler : MonoBehaviour
    {
        public static AudioControler Instance;

        public AudioListener Listener;
        public void Awake()
        {
            Instance = this;
            Listener = Listener ?? this.GetComponent<AudioListener>() ?? this.GetComponentInChildren<AudioListener>() ?? FindObjectOfType<AudioListener>() ?? gameObject.AddComponent<AudioListener>();
            NextBGM = NextBGM != null ? this.NextBGM : Listener.gameObject.AddComponent<AudioSource>();
            this.CurrentBGM = this.CurrentBGM != null ? this.CurrentBGM : Listener.gameObject.AddComponent<AudioSource>();
        }
        /// <summary>
        /// 音量数值参考记录
        /// </summary>
        [SerializeField]
        public IGameVolume GameVolume
        {
            get
            {
                return this.GetComponent<IGameVolume>();
            }
        }
        public Dictionary<AudioClip, AudioSource> AudioSourcesPool = new Dictionary<AudioClip, AudioSource>();
        public List<AudioSource> EffectPool = new List<AudioSource>();
        public AudioSource CurrentBGM = null;

        public AudioSource NextBGM = null;
        public AudioSource PlayEffectSound(AudioClip Clip)
        {
            if (Clip == null) return null;
            if (!AudioSourcesPool.ContainsKey(Clip))
            {
                AudioSource AS = Listener.gameObject.AddComponent<AudioSource>();
                AudioSourcesPool[Clip] = AS;
            }
            AudioSource source = AudioSourcesPool[Clip];
            if (!source.isPlaying)
            {
                source.clip = Clip;
                source.loop = false;
                source.Play();
                return source;
            }
            else
            {
                return source;
            }
        }
        public AudioSource PlayEffect(AudioClip clip)
        {
            AudioSource AS = Listener.gameObject.AddComponent<AudioSource>();
            EffectPool.Add(AS);
            AS.clip = clip;
            AS.volume = GameVolume.effectSound;
            AS.Play();
            return AS;
        }
        public void StopAllEffectSound()
        {
            foreach (var audioSource in AudioSourcesPool.Values)
            {
                audioSource.Stop();
            }
        }
        public void StopEffectSound(AudioClip Clip)
        {
            if (Clip == null) return;
            if (!AudioSourcesPool.ContainsKey(Clip)) return;
            AudioSourcesPool[Clip].Stop();
        }
        public void MuteBGM(bool mute)
        {
            if (CurrentBGM && CurrentBGM.isPlaying)
            {
                if (mute)
                {
                    state = AudioState.MuteTrue;
                }
                else
                {
                    state = AudioState.MuteFalse;
                }
            }
        }
        public enum AudioState
        {
            Normal, ChangingBGM, MuteTrue, MuteFalse
        }

        public AudioState state = AudioState.Normal;
        public void PlayBGM(AudioClip Clip, bool loop = true)
        {
            if (CurrentBGM.isPlaying)
            {
                state = AudioState.ChangingBGM;
                NextBGM.clip = Clip;
                NextBGM.volume = 0;
                NextBGM.loop = loop;
                NextBGM.Play();
            }
            else
            {
                CurrentBGM.clip = Clip;
                CurrentBGM.volume = GameVolume.backgroundSound;
                CurrentBGM.loop = loop;
                CurrentBGM.Play();
            }
        }

        public bool BindCamera;
        public void Update()
        {
            if (BindCamera)
            {
                if (Camera.main && Listener.transform.position != Camera.main.transform.position)
                    Listener.transform.position = Camera.main.transform.position;
            }
            if (CurrentBGM)
            {
                switch (state)
                {
                    case AudioState.ChangingBGM:
                        CurrentBGM.volume = Mathf.Max(0, CurrentBGM.volume - Time.deltaTime / 2);
                        if (CurrentBGM.volume == 0)
                        {
                            NextBGM.volume = Mathf.Min(GameVolume.backgroundSound, NextBGM.volume + Time.deltaTime / 2);
                        }
                        if (CurrentBGM.volume == 0 && NextBGM.volume == GameVolume.backgroundSound)
                        {
                            AudioSource t = NextBGM;
                            NextBGM = CurrentBGM;
                            CurrentBGM = t;
                            NextBGM.Stop();
                            state = AudioState.Normal;
                        }
                        break;
                    case AudioState.MuteTrue:
                        CurrentBGM.volume = Mathf.Max(0, CurrentBGM.volume - Time.deltaTime / 2);
                        break;
                    case AudioState.MuteFalse:
                        {
                            CurrentBGM.volume = Mathf.Min(GameVolume.backgroundSound, CurrentBGM.volume + Time.deltaTime / 2);
                            if (CurrentBGM.volume == GameVolume.backgroundSound)
                            {
                                state = AudioState.Normal;
                            }
                        }
                        break;
                    default:
                        {
                            if (CurrentBGM.volume != GameVolume.backgroundSound)
                                CurrentBGM.volume = GameVolume.backgroundSound; break;
                        }
                }
            }
            if (AudioSourcesPool.Count > 0)
            {
                List<AudioClip> markDelete = new List<AudioClip>();
                foreach (KeyValuePair<AudioClip, AudioSource> kv in AudioSourcesPool)
                {
                    if (!kv.Value.isPlaying)
                    {
                        markDelete.Add(kv.Key);
                    }
                    else
                    {
                        kv.Value.volume = GameVolume.effectSound;
                    }
                }
                foreach (AudioClip audioClip in markDelete)
                {
                    Destroy(AudioSourcesPool[audioClip]);
                    AudioSourcesPool.Remove(audioClip);
                }
            }
            if (EffectPool.Count > 0)
            {
                EffectPool.RemoveAll(e => e == null);
                List<AudioSource> markDelete2 = new List<AudioSource>();
                foreach (var AS in EffectPool)
                {
                    if (!AS.isPlaying)
                    {
                        markDelete2.Add(AS);
                    }
                }
                foreach (AudioSource AS in markDelete2)
                {
                    DestroyImmediate(AS);
                }
            }
        }

    }
}