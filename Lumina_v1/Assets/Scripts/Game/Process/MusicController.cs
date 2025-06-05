using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Menu;
using NoteEditor.Utility;


namespace Game.Process
{
    /// <summary>
    /// 管理音乐的播放、切换、暂停等
    /// </summary>
    public class MusicController : SingletonMonoBehaviour<MusicController>
    {
        [SerializeField] private AudioSource music;

        [SerializeField] private int samples;

        private float volume;

        public Text  musicTime;

        void Start()
        {
            volume = PlayerSettings.Instance.musicVolume;
            music.volume = volume;
        }

        public bool IsPlaying()
        {
            return music.isPlaying;
        }

        public void PlayMusic()
        {
            music.Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            music.clip = clip;
            music.Play();
        }

        public void SetMusic(AudioClip clip)
        {
            
            music.clip = clip;
            music.time = 0;
            
        }

        public AudioClip GetMusic()
        {
            return music.clip;
        }

        public int GetSamples()
        {
            return music.timeSamples;
        }

        public float GetTime()
        {
            return music.time;
        }

        void Update()
        {
            samples = GetSamples();

            ShowMusicTime();
        }

        public float SampleToTime(float s)
        {
            if(music.clip == null) return 0;

            return ConvertUtils.SamplesToTime(s, music.clip.frequency);
        }

        public float TimeToSample(float time)
        {
            if (music.clip == null) return 0;

            return time * music.clip.frequency;
        }
       
        private void ShowMusicTime()
        {
            if (music == null || music.clip == null)
                return;

            // 计算剩余时间（总长度 - 当前播放时间）
            float remainingTime = music.clip.length - music.time;

            musicTime.text = "Time: "+$"{(int)remainingTime / 60:00}:{(int)remainingTime % 60:00}";
        }
        
    }
}


