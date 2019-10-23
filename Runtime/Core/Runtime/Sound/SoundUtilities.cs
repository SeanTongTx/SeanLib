//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeanLib.Core
{
    /// <summary>
    /// 声音工具
    /// </summary>
    public static class SoundUtilities
    {
        /// <summary>
        /// 声音大小管理器
        /// </summary>
        public static IGameVolume volumeCentre;

        static AudioListener mListener;

        static bool mLoaded = false;
        static float mGlobalVolume = 1f;

        static Dictionary<int, AudioSource> _audioSources = new Dictionary<int, AudioSource>();
        static AudioSource GetAudioSource(int index)
        {
            if (!_audioSources.ContainsKey(index))
                _audioSources.Add(index, mListener.gameObject.AddComponent<AudioSource>());
            if (_audioSources[index] == null)
            {
                _audioSources.Remove(index);
                _audioSources.Add(index, mListener.gameObject.AddComponent<AudioSource>());
            }
            return _audioSources[index];
        }

        /// <summary>
        /// Globally accessible volume affecting all sounds played via NGUITools.PlaySound().
        /// </summary>
        static public float soundVolume
        {
            get
            {
                if (!mLoaded)
                {
                    mLoaded = true;
                    mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
                }
                return mGlobalVolume;
            }
            set
            {
                if (mGlobalVolume != value)
                {
                    mLoaded = true;
                    mGlobalVolume = value;
                    PlayerPrefs.SetFloat("Sound", value);
                }
            }
        }

        /// <summary>
        /// Play the specified audio clip.
        /// </summary>

        static public AudioSource PlaySound(AudioClip clip) { return PlaySound(clip, 1f, 1f); }

        /// <summary>
        /// Play the specified audio clip with the specified volume.
        /// </summary>

        static public AudioSource PlaySound(AudioClip clip, float volume) { return PlaySound(clip, volume, 1f); }

        /// <summary>
        /// Play the specified audio clip with the specified volume and pitch.
        /// </summary>

        static public AudioSource PlaySound(AudioClip clip, float volume, float pitch)
        {
            volume *= soundVolume;

            if (clip != null)
            {
                if (mListener == null || !mListener.gameObject.activeSelf==true)
                {
                    mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

                    if (mListener == null)
                    {
                        Camera cam = Camera.main;
                        if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
                        if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
                    }
                }

                if (mListener != null && mListener.enabled && mListener.gameObject.activeSelf == true)
                {
                    AudioSource source = mListener.gameObject.AddComponent<AudioSource>();
                    source.pitch = pitch;
                    source.clip = clip;
                    source.volume = volume;
                    source.Play();
                    return source;
                }
            }
            return null;
        }
        static public void StopSound(AudioSource audioSource)
        {
            if (audioSource!=null)
                audioSource.Stop();
        }
        /// <summary>
        /// Play the specified audio clip.
        /// </summary>

        static public AudioSource PlaySoundOnExists(AudioClip clip, int existsIndex = 0) { return PlaySoundOnExists(clip, 1f, 1f, existsIndex); }

        /// <summary>
        /// Play the specified audio clip with the specified volume.
        /// </summary>

        static public AudioSource PlaySoundOnExists(AudioClip clip, float volume, int existsIndex = 0) { return PlaySoundOnExists(clip, volume, 1f, existsIndex); }

        /// <summary>
        /// Play the specified audio clip with the specified volume and pitch.
        /// </summary>

        static public AudioSource PlaySoundOnExists(AudioClip clip, float volume, float pitch, int existsIndex = 0)
        {
            volume *= soundVolume;

            if (clip != null)
            {
                if (mListener == null || mListener.gameObject.activeSelf == true)
                {
                    mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

                    if (mListener == null)
                    {
                        Camera cam = Camera.main;
                        if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
                        if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
                    }
                }

                if (mListener != null && mListener.enabled && mListener.gameObject.gameObject.activeSelf == true)
                {
                    AudioSource audioSource = GetAudioSource(existsIndex);
                    audioSource.pitch = pitch;
                    audioSource.PlayOneShot(clip, volume);
                    return audioSource;
                }
            }
            return null;
        }
    }
}

