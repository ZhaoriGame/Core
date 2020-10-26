using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.Audio
{
    
	[Serializable]
	public class SoundEffect : MonoBehaviour
	{
		
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float originalVolume;
        [SerializeField] private float duration;
        [SerializeField] private float playbackPosition;
        [SerializeField] private float time;
        [SerializeField] private Action callback;
        [SerializeField] private bool singleton;
        
		public string Name => audioSource.clip.name;

		public float Length => audioSource.clip.length;
		
		public float PlaybackPosition => audioSource.time;
		
		public AudioSource Source
		{
			get => audioSource;
			set => audioSource = value;
		}
		
		/// <summary>
		/// 初始的声音
		/// </summary>
		public float OriginalVolume
		{
			get => originalVolume;
			set => originalVolume = value;
		}

		/// <summary>
		/// 持续时间
		/// </summary>
		public float Duration
		{
			get => duration;
			set => duration = value;
		}

		/// <summary>
		/// 播放的剩余时间
		/// </summary>
		/// <value>The duration.</value>
		public float Time
		{
			get => time;
			set => time = value;
		}

		/// <summary>
		/// 剩余时间
		/// </summary>
		/// <value>The normalised time.</value>
		public float NormalisedTime => Time / Duration;

		/// <summary>
		/// 播放结束回调
		/// </summary>
		/// <value>The callback.</value>
		public Action Callback
		{
			get => callback;
			set => callback = value;
		}

		/// <summary>
		/// 声音是否唯一（同时存在）
		/// </summary>
		public bool Singleton
		{
			get => singleton;
			set => singleton = value;
		}
	}
}


