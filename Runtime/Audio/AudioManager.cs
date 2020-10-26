using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{

	/// <summary>
	/// 音效过度种类
	/// </summary>
	public enum MusicTransition
	{
		/// <summary>
		/// 马上播放 
		/// </summary>
		Swift,
		/// <summary>
		/// 渐入和渐出
		/// </summary>
		LinearFade,
		/// <summary>
		/// 平滑过渡到下一段
		/// </summary>
		CrossFade
	}

	/// <summary>
	/// 背景音乐
	/// </summary>
	[Serializable]
	public struct BackgroundMusic
	{
		
		public AudioClip CurrentClip;
		
		public AudioClip NextClip;
		
		public MusicTransition MusicTransition;
		/// <summary>
		/// 转换持续时间
		/// </summary>
		public float TransitionDuration;
	}
	

	
	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : MonoBehaviour
	{
		#region Inspector Variables

		[Header("背景音乐属性")]

		[Tooltip("是否静音")]
		[SerializeField] bool _musicOn = true;

		[Tooltip("音量")]
		[Range(0, 1)]
		[SerializeField] float _musicVolume = 1f;

		[Tooltip("使用此音量在初始化时")]
		[SerializeField] bool _useMusicVolOnStart = false;
		
		[SerializeField] AudioMixerGroup _musicMixerGroup = null;

		[Tooltip("music mixer Group Name")]
		[SerializeField] string _volumeOfMusicMixer = string.Empty;

		[Space(3)]

		[Header("音效属性")]

		[Tooltip("SoundFX 是否打开")]
		[SerializeField] bool _soundFxOn = true;

		[Tooltip("The sound effects volume")]
		[Range(0, 1)]
		[SerializeField] float _soundFxVolume = 1f;

		[Tooltip("Use the current sound effect volume settings on initialisation start")]
		[SerializeField] bool _useSfxVolOnStart = false;

		[SerializeField] AudioMixerGroup _soundFxMixerGroup = null;

		[Tooltip("The exposed volume parameter name of the sound effects mixer group")]
		[SerializeField] string _volumeOfSFXMixer = string.Empty;

		[Space(3)]

		[SerializeField] List<AudioClip> _playlist = new List<AudioClip>();
		// TOGO :: Try a Reorderable list for future implementation of the playlist
		#endregion

		#region Singleton Pattern

		private static AudioManager instance;
		//防止多线程同时创建
		private static object key = new object();
		private static bool alive = true;
		
		public static AudioManager Instance
		{
			
			get
			{
				if (!alive)
				{
					Debug.LogWarning(typeof(AudioManager) + "' is already destroyed on application quit.");
					return null;
				}

				
				if (instance == null)
				{
					instance = FindObjectOfType<AudioManager>();

					
					if (instance == null)
					{
						lock (key)
						{
							instance = new GameObject().AddComponent<AudioManager>();
						}
					}
				}

				return instance;
			}
		}
		
		private AudioManager() { }

		#endregion

		#region Public Static Getters

		
		public AudioClip CurrentMusicClip => backgroundMusic.CurrentClip;

		
		public List<SoundEffect> SoundFxPool => sfxPool;

		
		public List<AudioClip> Playlist => _playlist;

		
		public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;

		
		public float MusicVolume
		{
			get => _musicVolume;
			set => SetBGMVolume (value);
		}

		
		public float SoundVolume
		{
			get => _soundFxVolume;
			set => SetSFXVolume (value);
		}

		
		public bool IsMusicOn
		{
			get => _musicOn;
			set => ToggleBGMMute (value);
		}

		
		public bool IsSoundOn
		{
			get => _soundFxOn;
			set => ToggleSFXMute (value);
		}

		
		public bool IsMasterMute
		{
			get => !_musicOn && !_soundFxOn;
			set => ToggleMute(value);
		}

		#endregion

		#region Private Static Variables

		
		List<SoundEffect> sfxPool = new List<SoundEffect>();
		
		static BackgroundMusic backgroundMusic;
		
		static AudioSource musicSource = null, crossfadeSource = null;

		private static float currentMusicVol = 0;
		private static float currentSfxVol = 0;
		private static float musicVolCap = 0;
		private static float savedPitch = 1f;
		
		
		static bool musicOn = false, sfxOn = false;
		// 过度时间
		static float transitionTime;

		// Player Prefabs store keys
		static readonly string BgMusicVolKey = "BGMVol";
		static readonly string SoundFxVolKey = "SFXVol";
		static readonly string BgMusicMuteKey = "BGMMute";
		static readonly string SoundFxMuteKey = "SFXMute";

		#endregion

		#region Initialisation Functions

	
		void OnDestroy()
		{
			StopAllCoroutines();
			SaveAllPreferences();
		}
		
		void OnApplicationExit()
		{
			alive = false;
		}
		
		void Initialise()
		{
			gameObject.name = "AudioManager";

			_musicOn = LoadBGMMuteStatus();
			_musicVolume = _useMusicVolOnStart ? _musicVolume : LoadBGMVolume();
			_soundFxOn = LoadSFXMuteStatus();
			_soundFxVolume = _useSfxVolOnStart ? _soundFxVolume : LoadSFXVolume();

			
			if (musicSource == null)
			{
				musicSource = gameObject.GetComponent<AudioSource>();
				musicSource = musicSource ? musicSource : gameObject.AddComponent<AudioSource>();
			}

			musicSource = ConfigureAudioSource(musicSource);
			DontDestroyOnLoad(this.gameObject);
		}

		
		void Awake()
		{
			if (instance == null)
			{
				instance = this;
				Initialise();
			}
			else if (instance != this)
			{
			
				Destroy(this.gameObject);
			}
		}

	
		void Start()
		{
			if (musicSource != null)
			{
				//todo 放到Awake
				StartCoroutine(OnUpdate());
			}
		}

	
		AudioSource ConfigureAudioSource(AudioSource audioSource)
		{
			audioSource.outputAudioMixerGroup = _musicMixerGroup;
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = 0;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.loop = true;
			audioSource.volume = LoadBGMVolume();
			audioSource.mute = !_musicOn;

			return audioSource;
		}

		#endregion

		#region Update Functions

		
		private void ManageSoundEffects()
		{
			for (int i = sfxPool.Count - 1; i >= 0; i--)
			{
				SoundEffect sfx = sfxPool[i];
				if (sfx.Source.isPlaying && !float.IsPositiveInfinity(sfx.Time))
				{
					
					sfx.Time -= Time.deltaTime;
					sfxPool[i] = sfx;
				}
				
                if (sfxPool[i].Time <= 0.0001f || HasPossiblyFinished(sfxPool[i]))
				{
					sfxPool[i].Source.Stop();
					sfxPool[i].Callback?.Invoke();
					Destroy(sfxPool[i].gameObject);
					sfxPool.RemoveAt(i);
					break;
				}
			}
		}
		
        bool HasPossiblyFinished(SoundEffect soundEffect)
        {
            return !soundEffect.Source.isPlaying && FloatEquals(soundEffect.PlaybackPosition, 0) && soundEffect.Time <= 0.09f;
        }

        /// <summary>
        /// 比较Float
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        bool FloatEquals(float num1, float num2, float threshold = 0.0001f)
        {
            return Math.Abs(num1 - num2) < threshold;
        }

		/// <summary>
		/// Music 是否被修改
		/// </summary>
		private bool IsMusicAltered()
		{
            bool flag = musicOn != _musicOn || musicOn != !musicSource.mute || !FloatEquals(currentMusicVol, _musicVolume);

            
			if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer.Trim()))
			{
				float vol;
				_musicMixerGroup.audioMixer.GetFloat(_volumeOfMusicMixer, out vol);
				vol = NormaliseVolume(vol);

                return flag || !FloatEquals(currentMusicVol, vol);
			}

			return flag;
		}

		
		private bool IsSoundFxAltered()
		{
            bool flag = _soundFxOn != sfxOn || !FloatEquals(currentSfxVol, _soundFxVolume);

			if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer.Trim()))
			{
				float vol;
				_soundFxMixerGroup.audioMixer.GetFloat(_volumeOfSFXMixer, out vol);
				vol = NormaliseVolume(vol);

                return flag || !FloatEquals(currentSfxVol, vol);
			}

			return flag;
		}

	
		/// <summary>
		/// 音乐渐入和渐出
		/// </summary>
		private void CrossFadeBackgroundMusic()
		{
			if (backgroundMusic.MusicTransition == MusicTransition.CrossFade)
			{
				//名字是否一样
				if (musicSource.clip.name != backgroundMusic.NextClip.name)
				{
					transitionTime -= Time.deltaTime;

					musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);
					crossfadeSource.volume = Mathf.Clamp01(musicVolCap - musicSource.volume);
					crossfadeSource.mute = musicSource.mute;

					if (musicSource.volume <= 0.00f)
					{
						SetBGMVolume(musicVolCap);
						PlayBackgroundMusic(backgroundMusic.NextClip, crossfadeSource.time, crossfadeSource.pitch);
					}
				}
			}
		}
		
		private void FadeOutFadeInBackgroundMusic()
		{
			if (backgroundMusic.MusicTransition == MusicTransition.LinearFade)
			{
				if (musicSource.clip.name == backgroundMusic.NextClip.name)
				{
					transitionTime += Time.deltaTime;
					musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime / backgroundMusic.TransitionDuration);

					if (musicSource.volume >= musicVolCap)
					{
						SetBGMVolume(musicVolCap);
						PlayBackgroundMusic(backgroundMusic.NextClip, musicSource.time, savedPitch);
					}
				}
				else
				{
					transitionTime -= Time.deltaTime;
					musicSource.volume = Mathf.Lerp(0, musicVolCap, transitionTime/backgroundMusic.TransitionDuration);
					if (musicSource.volume <= 0.00f)
					{
						musicSource.volume = transitionTime = 0;
						PlayMusicFromSource(ref musicSource, backgroundMusic.NextClip, 0, musicSource.pitch);
					}
				}
			}
		}

		/// <summary>
		/// Update
		/// </summary>
		IEnumerator OnUpdate()
		{
			while (alive)
			{
				ManageSoundEffects();

				
				if (IsMusicAltered())
				{
					ToggleBGMMute(!musicOn);

                    if (!FloatEquals(currentMusicVol, _musicVolume))
					{
						currentMusicVol = _musicVolume;
					}

					if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer))
					{
						float vol;
						_musicMixerGroup.audioMixer.GetFloat(_volumeOfMusicMixer, out vol);
						vol = NormaliseVolume(vol);
						currentMusicVol = vol;
					}

					SetBGMVolume(currentMusicVol);
				}

			
				if (IsSoundFxAltered())
				{
					ToggleSFXMute(!sfxOn);

                    if (!FloatEquals(currentSfxVol,_soundFxVolume))
					{
						currentSfxVol = _soundFxVolume;
					}

					if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer))
					{
						float vol;
						_soundFxMixerGroup.audioMixer.GetFloat(_volumeOfSFXMixer, out vol);
						vol = NormaliseVolume(vol);
						currentSfxVol = vol;
					}

					SetSFXVolume(currentSfxVol);
				}

				if (crossfadeSource != null)
				{
					CrossFadeBackgroundMusic();

					yield return null;
				}
				else
				{
					if (backgroundMusic.NextClip != null)
					{
						FadeOutFadeInBackgroundMusic();

						yield return null;
					}
				}

				yield return new WaitForEndOfFrame();
			}
		}

		#endregion

		#region Background Music Functions

		
		private void PlayMusicFromSource(ref AudioSource audio_source, AudioClip clip, float playback_position, float pitch)
		{
			try
			{
				audio_source.clip = clip;
				audio_source.time = playback_position;
				audio_source.pitch = pitch = Mathf.Clamp (pitch, -3f, 3f);
				audio_source.Play();
			}
			catch (NullReferenceException nre)
			{
				Debug.LogError(nre.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		
		private void PlayBackgroundMusic(AudioClip clip, float playback_position, float pitch)
		{
			PlayMusicFromSource(ref musicSource, clip, playback_position, pitch);
			backgroundMusic.NextClip = null;
			backgroundMusic.CurrentClip = clip;
			if (crossfadeSource != null)
			{
				Destroy(crossfadeSource);
				crossfadeSource = null;
			}
		}

		#region Public Background Music API 

		
		public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration, float volume, float pitch, float playback_position = 0)
		{
		
			if (clip == null || backgroundMusic.CurrentClip == clip)
			{
				return;
			}
			
			if (backgroundMusic.CurrentClip == null || transition_duration <= 0)
			{
				transition = MusicTransition.Swift;
			} 
			
			if (transition == MusicTransition.Swift)
			{
				PlayBackgroundMusic(clip, playback_position, pitch);
				SetBGMVolume(volume);
			}
			else
			{
				
				if (backgroundMusic.NextClip != null)
				{
					Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
					return;
				}

				// Save the transition effect to be handled by the internal manager
				backgroundMusic.MusicTransition = transition;
				// set the duration for the tramsition
				transitionTime = backgroundMusic.TransitionDuration = transition_duration;
				// Register the music volume limit or cap when transitioning
				musicVolCap = _musicVolume;
				// Set the next audio data clip to transition to
				backgroundMusic.NextClip = clip;
				// Inititalise the crossfade audio source if transition is a cross fade
				if (backgroundMusic.MusicTransition == MusicTransition.CrossFade)
				{
					// Stop!!! Still performing a crossfade transition
					if (crossfadeSource != null)
					{
						Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
						return;
					}

					// Initialise an AudioSource to the crossfade source
					crossfadeSource = ConfigureAudioSource(gameObject.AddComponent<AudioSource>());
					// The crossfade volume increases as the music volume decreases, so get its relative volume
					crossfadeSource.volume = Mathf.Clamp01(musicVolCap - currentMusicVol);
					crossfadeSource.priority = 0;
					// Start playing the clip from the cross fade source
					PlayMusicFromSource(ref crossfadeSource, backgroundMusic.NextClip, 0, pitch);
				}
			}
		}

		/// <summary>
		/// Plays a background music. 
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip">The audio data to play</param>
		/// <param name="transition">How should the music change from the current to the next.</param>
		/// <param name="transition_duration">Time in secs it takes to transition.</param>
		/// <param name="volume">Playback volume.</param>
		public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration, float volume)
		{
			PlayBGM(clip, transition, transition_duration, volume, 1f);
		}

		/// <summary>
		/// Plays a background music.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip">The audio data to play</param>
		/// <param name="transition">How should the music change from the current to the next.</param>
		/// <param name="transition_duration">Time in secs it takes to transition.</param>
		public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration)
		{
			PlayBGM(clip, transition, transition_duration, _musicVolume, 1f);
		}

		/// <summary>
		/// Plays a background music.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip">The audio data to play</param>
		/// <param name="transition">How should the music change from the current to the next. Use MusicTransition to specify type </param>
		public void PlayBGM(AudioClip clip, MusicTransition transition)
		{
			PlayBGM(clip, transition, 1f, _musicVolume, 1f);
		}

		/// <summary>
		/// Plays a background music using the swift the transition mode.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip">The audio data to play</param>
		public void PlayBGM(AudioClip clip)
		{
			PlayBGM(clip, MusicTransition.Swift, 1f, _musicVolume, 1f);
		}

		/// <summary>
		/// Plays a background music. 
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip_path">Path name of the target clip from the Resources folder</param>
		/// <param name="transition">How should the music change from the current to the next.</param>
		/// <param name="transition_duration">Time in secs it takes to transition.</param>
		/// <param name="volume">Playback volume.</param>
		/// <param name="pitch">Pitch level of the clip.</param>
		/// <param name="playback_position">Play position of the clip.</param>
		public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration, float volume, float pitch, float playback_position = 0)
		{
			PlayBGM (LoadClip(clip_path), transition, transition_duration, volume, pitch, playback_position);
		}

		/// <summary>
		/// Plays a background music. 
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip_path">Path name of the target clip from the Resources folder</param>
		/// <param name="transition">How should the music change from the current to the next.</param>
		/// <param name="transition_duration">Time in secs it takes to transition.</param>
		/// <param name="volume">Playback volume.</param>
		public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration, float volume)
		{
			PlayBGM (LoadClip(clip_path), transition, transition_duration, volume, 1f);
		}

		/// <summary>
		/// Plays a background music.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip_path">Path name of the target clip from the Resources folder</param>
		/// <param name="transition">How should the music change from the current to the next.</param>
		/// <param name="transition_duration">Time in secs it takes to transition.</param>
		public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration)
		{
			PlayBGM(LoadClip(clip_path), transition, transition_duration, _musicVolume, 1f);
		}

		/// <summary>
		/// Plays a background music.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip_path">Path name of the target clip from the Resources folder</param>
		/// <param name="transition">How should the music change from the current to the next. Use MusicTransition to specify type </param>
		public void PlayBGM(string clip_path, MusicTransition transition)
		{
			PlayBGM(LoadClip(clip_path), transition, 1f, _musicVolume, 1f);
		}

		/// <summary>
		/// Plays a background music using the swift the transition mode.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="clip_path">Path name of the target clip from the Resources folder</param>
		public void PlayBGM(string clip_path)
		{
			PlayBGM(LoadClip(clip_path), MusicTransition.Swift, 1f, _musicVolume, 1f);
		}

		/// <summary>
		/// Stops the playing background music
		/// </summary>
		public void StopBGM()
		{
			if (musicSource.isPlaying)
			{
				musicSource.Stop();
			}
		}

		/// <summary>
		/// Pauses the playing background music
		/// </summary>
		public void PauseBGM()
		{
			if (musicSource.isPlaying)
			{
				musicSource.Pause();
			}
		}

		/// <summary>
		/// Resumes the playing background music
		/// </summary>
		public void ResumeBGM()
		{
			if (!musicSource.isPlaying)
			{
				musicSource.UnPause();
			}
		}

		#endregion

		#endregion

		#region Sound Effect Functions

		/// <summary>
		/// Inner function used to play all resulting sound effects.
		/// Initialises some particular properties for the sound effect.
		/// </summary>
		/// <param name="audio_clip">The audio data to play</param>
		/// <param name="location">World location of the audio clip.</param>
		/// <returns>Newly created gameobject with sound effect and audio source attached</returns>
		private GameObject CreateSoundFx(AudioClip audio_clip, Vector2 location)
		{
			// Create a temporary game object to host our audio source
			GameObject host = new GameObject("TempAudio");
			// Set the temp audio's world position
			host.transform.position = location;
			// Parent it to the AudioManager until further notice
			host.transform.SetParent(transform);
			// Specity a tag for future use
			host.AddComponent<SoundEffect>();

			// Add an audio source to that host
			AudioSource audioSource = host.AddComponent<AudioSource>() as AudioSource;
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = 0;
			audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
			// Set the mixer group for the sound effect if one exists
			audioSource.outputAudioMixerGroup = _soundFxMixerGroup;
			// Set that audio source clip to the one in paramaters
			audioSource.clip = audio_clip;
			// Set the mute value
			audioSource.mute = !_soundFxOn;

			return host;
		}

		#region Public Sound Effect API 

		/// <summary>
		/// Returns the index of a sound effect in pool if one exists.
		/// </summary>
		/// <param name="name">The name of the sound effect.</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <returns>Index of sound effect or -1 is none exists</returns>
		public int IndexOfSoundFxPool(string name, bool singleton = false)
		{
			int index = 0;
			while (index < sfxPool.Count)
			{
				if (sfxPool[index].Name == name && singleton == sfxPool[index].Singleton)
				{
					return index;
				}

				index++;
			}

			return -1;
		}

		/// <summary>
		/// Plays a sound effect for a duration of time at a given location in world space and calls the specified callback function after the time is over.
		/// </summary>
		/// <returns>An audiosource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="location">World location of the clip</param>
		/// <param name="duration">The length in time the clip should play</param>
		/// <param name="volume">Playback volume.</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <param name="pitch">Pitch level of the clip.</param>
		/// <param name="callback">Action callback to be invoked after the sound has finished.</param>
		public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
		{
			if (duration <= 0 || clip == null) 
			{
				return null;
			}

			int index = IndexOfSoundFxPool(clip.name, true);

			if (index >= 0)
			{
				// Reset the duration if it exists in the pool
				SoundEffect singletonSFx = sfxPool[index];
				singletonSFx.Duration = singletonSFx.Time = duration;
				sfxPool[index] = singletonSFx;

				return sfxPool[index].Source;
			}

			GameObject host = null;
			AudioSource source = null;

			host = CreateSoundFx(clip, location);
			source = host.GetComponent<AudioSource>();
			source.loop = duration > clip.length;
			source.volume = _soundFxVolume * volume;
			source.pitch = pitch;

			// Create a new repeat sound
			SoundEffect sfx = host.GetComponent<SoundEffect>();
			sfx.Singleton = singleton;
			sfx.Source = source;
			sfx.OriginalVolume = volume;
			sfx.Duration = sfx.Time = duration;
			sfx.Callback = callback;

			// Add it to the list
			sfxPool.Add(sfx);

			//Destroy (host, duration);
			//FireCallback (callback, duration);

			// Start playing the sound
			source.Play();

			return source;
		}

		/// <summary>
		/// Plays a sound effect for a duration of time at a given location in world space and calls the specified callback function after the time is over
		/// </summary>
		/// <returns>An audiosource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="location">World location of the clip</param>
		/// <param name="duration">The length in time the clip should play</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <param name="callback">Action callback to be invoked after the sound has finished.</param>
		public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, bool singleton = false, Action callback = null)
		{
			return PlaySFX(clip, location, duration, _soundFxVolume, singleton, 1f, callback);
		}

		/// <summary>
		/// Plays a sound effect for a duration of time and calls the specified callback function after the time is over
		/// </summary>
		/// <returns>An audiosource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="duration">The length in time the clip should play</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <param name="callback">Action callback to be invoked after the sound has finished.</param>
		public AudioSource PlaySFX(AudioClip clip, float duration, bool singleton = false, Action callback = null)
		{
			return PlaySFX(clip, Vector2.zero, duration, _soundFxVolume, singleton, 1f, callback);
		}

		/// <summary>
		/// Repeats a sound effect for a specified amount of times at a given location in world space and calls the specified callback function after the sound is over.
		/// </summary>
		/// <returns>An audiosource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="location">World location of the clip</param>
		/// <param name="repeat">How many times in successions you want the clip to play. To loop forever, set as a negative number</param>
		/// <param name="volume">Playback volume.</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <param name="pitch">Pitch level of the clip.</param>
		/// <param name="callback">Action callback to be invoked after the sound has finished.</param>
		public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
		{
			if (clip == null) 
			{
				return null;
			}

			if (repeat != 0)
			{
				int index = IndexOfSoundFxPool(clip.name, true);

				if (index >= 0)
				{
					// Reset the duration if it exists in the pool
					SoundEffect singletonSFx = sfxPool[index];
					singletonSFx.Duration = singletonSFx.Time = repeat > 0 ? clip.length * repeat : float.PositiveInfinity;
					sfxPool[index] = singletonSFx;

					return sfxPool[index].Source;
				}

				GameObject host = CreateSoundFx(clip, location);
				AudioSource source = host.GetComponent<AudioSource>();
				source.loop = repeat != 0;
				source.volume = _soundFxVolume * volume;
				source.pitch = pitch;

				// Create a new repeat sound
				SoundEffect sfx = host.GetComponent<SoundEffect>();
				sfx.Singleton = singleton;
				sfx.Source = source;
				sfx.OriginalVolume = volume;
				sfx.Duration = sfx.Time = repeat > 0 ? clip.length * repeat : float.PositiveInfinity;
				sfx.Callback = callback;

				// Add it to the list
				sfxPool.Add(sfx);

				// Start playing the sound
				source.Play();

				return source;
			}

			// Play one shot if repat length is less than or equal to 1
			return PlayOneShot(clip, location, volume, pitch, callback);
		}

		/// <summary>
		/// Repeats a sound effect for a specified amount of times at a given location in world space and calls the specified callback function after the sound is over.
		/// </summary>
		/// <returns>An audiosource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="location">World location of the clip</param>
		/// <param name="repeat">How many times in successions you want the clip to play. To loop forever, set as a negative number</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <param name="callback">Action callback to be invoked after the sound has finished.</param>
		public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, bool singleton = false, Action callback = null)
		{
			return RepeatSFX(clip, location, repeat, _soundFxVolume, singleton, 1f, callback);
		}

		/// <summary>
		/// Repeats a sound effect for a specified amount of times at a given location in world space and calls the specified callback function after the sound is over.
		/// </summary>
		/// <returns>An audiosource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="repeat">How many times in successions you want the clip to play. To loop forever, set as a negative number</param>
		/// <param name="singleton">Is the sound effect a singleton.</param>
		/// <param name="callback">Action callback to be invoked after the sound has finished.</param>
		public AudioSource RepeatSFX(AudioClip clip, int repeat, bool singleton = false, Action callback = null)
		{
			return RepeatSFX(clip, Vector2.zero, repeat, _soundFxVolume, singleton, 1f, callback);
		}

		/// <summary>
		/// Plays a sound effect once at a location in world space and calls the specified callback function after the sound is over
		/// </summary>
		/// <returns>An AudioSource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="location">World location of the clip</param>
		/// <param name="volume">Playback volume.</param>
		/// <param name="pitch">Pitch level of the clip.</param>
		/// <param name="callback">Action callback to be invoked after clip has finished playing</param>
		public AudioSource PlayOneShot(AudioClip clip, Vector2 location, float volume, float pitch = 1f, Action callback = null)
		{
			if (clip == null) 
			{
				return null;
			}

			GameObject host = CreateSoundFx(clip, location);
			AudioSource source = host.GetComponent<AudioSource>();
			source.loop = false;
			source.volume = _soundFxVolume * volume;
			source.pitch = pitch;

			// Create a new repeat sound
			SoundEffect sfx = host.GetComponent<SoundEffect>();
			sfx.Singleton = false;
			sfx.Source = source;
			sfx.OriginalVolume = volume;
			sfx.Duration = sfx.Time = clip.length;
			sfx.Callback = callback;

			// Add it to the list
			sfxPool.Add(sfx);

			source.Play();

			return source;
		}

		/// <summary>
		/// Plays a sound effect once at a location in world space
		/// </summary>
		/// <returns>An AudioSource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="location">World location of the clip</param>
		/// <param name="callback">Action callback to be invoked after clip has finished playing</param>
		public AudioSource PlayOneShot(AudioClip clip, Vector2 location, Action callback = null)
		{
			return PlayOneShot(clip, location, _soundFxVolume, 1f, callback);
		}


		/// <summary>
		/// Plays a sound effect once and calls the specified callback function after the sound is over
		/// </summary>
		/// <returns>An AudioSource</returns>
		/// <param name="clip">The audio data to play</param>
		/// <param name="callback">Action callback to be invoked after clip has finished playing</param>
		public AudioSource PlayOneShot(AudioClip clip, Action callback = null)
		{
			return PlayOneShot(clip, Vector2.zero, _soundFxVolume, 1f, callback);
		}

		/// <summary>
		/// Pauses all the sound effects in the game
		/// </summary>
		public void PauseAllSFX()
		{
			// Loop through all sound effects with the SoundEffectTag and update their properties
			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				if (sfx.Source.isPlaying) sfx.Source.Pause();
			}
		}

		/// <summary>
		/// Resumes all the sound effect in the game
		/// </summary>
		public void ResumeAllSFX()
		{
			// Loop through all sound effects with the SoundEffectTag and update their properties
			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				if (!sfx.Source.isPlaying) sfx.Source.UnPause();
			}
		}

		/// <summary>
		/// Stops all the sound effects in the game
		/// </summary>
		public void StopAllSFX()
		{
			// Loop through all sound effects with the SoundEffectTag and update their properties
			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				if (sfx.Source) 
				{
					sfx.Source.Stop();
					Destroy(sfx.gameObject);
				}
			}

			sfxPool.Clear();
		}

		#endregion

		#endregion

		#region Setter Functions

		/// <summary>
		/// Loads an AudioClip from the Resources folder
		/// </summary>
		/// <param name="path">Path name of the target clip from the Resources folder</param>
		/// <param name="add_to_playlist">Option to add loaded clip into the playlist for future reference</param>
		/// <returns>The Audioclip from the resource folder</returns>
		public AudioClip LoadClip(string path, bool add_to_playlist = false)
		{
			AudioClip clip = Resources.Load(path) as AudioClip;
			if (clip == null)
			{
				Debug.LogError (string.Format ("AudioClip '{0}' not found at location {1}", path, System.IO.Path.Combine (Application.dataPath, "/Resources/"+path)));
				return null;
			}

			if (add_to_playlist)
			{
				AddToPlaylist(clip);
			}

			return clip;
		}

		/// <summary>
		/// Loads an AudioClip from the specified url path.
		/// </summary>
		/// <param name="path">The url path of the audio clip to download. For example: 'http://www.my-server.com/audio.ogg'</param>
		/// <param name="audio_type">The type of audio encoding for the downloaded clip. See AudioType</param>
		/// <param name="add_to_playlist">Option to add loaded clip into the playlist for future reference</param>
		/// <param name="callback">Action callback to be invoked after clip has finished loading</param>
		public void LoadClip(string path, AudioType audio_type, bool add_to_playlist, Action<AudioClip> callback)
		{
			StartCoroutine(LoadAudioClipFromUrl(path, audio_type, (downloadedContent) =>
				{
					if (downloadedContent != null && add_to_playlist)
					{
						AddToPlaylist(downloadedContent);
					}

					callback.Invoke(downloadedContent);
				}));
		}

		/// <summary>
		/// Loads the audio clip from URL.
		/// </summary>
		/// <returns>The audio clip from URL.</returns>
		/// <param name="audio_url">Audio URL.</param>
		/// <param name="audio_type">Audio type.</param>
		/// <param name="callback">Callback.</param>
		IEnumerator LoadAudioClipFromUrl(string audio_url, AudioType audio_type, Action<AudioClip> callback)
		{
			using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(audio_url, audio_type))
			{
                yield return www.SendWebRequest();

				if (www.isNetworkError)
				{
                    Debug.Log(string.Format("Error downloading audio clip at {0} : {1}", audio_url, www.error));
				}

				callback.Invoke(UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www));
			}
		}

		/// <summary>
		/// Toggles the mater mute that controls both background music and sound effect mute.
		/// </summary>
		/// <param name="flag">New toggle state of the mute controller.</param>
		private void ToggleMute(bool flag)
		{
			ToggleBGMMute(flag);
			ToggleSFXMute(flag);
		}

		/// <summary>
		/// Toggles the background music mute.
		/// </summary>
		/// <param name="flag">New toggle state of the background music controller.</param>
		private void ToggleBGMMute(bool flag)
		{
			musicOn = _musicOn = flag;
			musicSource.mute = !musicOn;
		}

		/// <summary>
		/// Toggles the sound effect mute.
		/// </summary>
		/// <param name="flag">New toggle state of the sound effect controller.</param>
		private void ToggleSFXMute(bool flag)
		{
			sfxOn = _soundFxOn = flag;

			// Loop through all sound effects with the SoundEffectTag and update their properties
			foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
			{
				sfx.Source.mute = !sfxOn;
			}

			//sfxOn = _soundFxOn;
		}

		/// <summary>
		/// Sets the background music volume.
		/// </summary>
		/// <param name="volume">New volume of the background music.</param>
		private void SetBGMVolume(float volume)
		{
			try
			{
				// Restrict the values to a range of [0 - 1] to suit the AudioManager
				volume = Mathf.Clamp01(volume);
				// Assign vol to all music volume variables
				musicSource.volume = currentMusicVol = _musicVolume = volume;

				// Is the AudioManager using a master mixer
				if (_musicMixerGroup != null && !string.IsNullOrEmpty(_volumeOfMusicMixer.Trim()))
				{
					// Get the equivalent mixer volume, always [-80db ... 20db]
					float mixerVol = -80f + (volume * 100f);
					// Set the volume of the background music group
					_musicMixerGroup.audioMixer.SetFloat(_volumeOfMusicMixer, mixerVol);
				}
			}
			catch (NullReferenceException nre)
			{
				Debug.LogError(nre.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		/// <summary>
		/// Sets the volume of the sound effects.
		/// </summary>
		/// <param name="volume">New volume for all the sound effects.</param>
		private void SetSFXVolume(float volume)
		{
			try
			{
				// Restrict the values to a range of [0 - 1] to suit the AudioManager
				volume = Mathf.Clamp01(volume);
				// Update the volume controllers of the sound effects
				currentSfxVol = _soundFxVolume = volume;

				// Loop through all sound effects with the SoundEffectTag and update their properties
				foreach (SoundEffect sfx in FindObjectsOfType<SoundEffect>())
				{
					sfx.Source.volume = _soundFxVolume * sfx.OriginalVolume;
					sfx.Source.mute = !_soundFxOn;
				}

				// Is the AudioManager using a master mixer
				if (_soundFxMixerGroup != null && !string.IsNullOrEmpty(_volumeOfSFXMixer.Trim()))
				{
					// Get the equivalent mixer volume, always [-80db ... 20db]
					float mixerVol = -80f + (volume * 100f);
					// Set the volume of the sound effect group
					_soundFxMixerGroup.audioMixer.SetFloat(_volumeOfSFXMixer, mixerVol);
				}
			}
			catch (NullReferenceException nre)
			{
				Debug.LogError(nre.Message);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		/// <summary>
		/// Normalises the volume so that it can be in a range of [0 - 1] to suit the music source volume and the AudioManager volume
		/// </summary>
		/// <returns>The normalised volume between the range of zero and one.</returns>
		/// <param name="vol">Vol.</param>
		private float NormaliseVolume(float vol)
		{
			vol += 80f;
			vol /= 100f;
			return vol;
		}

		#endregion

		#region Player Prefs Functions

		private float LoadBGMVolume()
		{
			return PlayerPrefs.HasKey(BgMusicVolKey) ? PlayerPrefs.GetFloat(BgMusicVolKey) : _musicVolume;
		}

		private float LoadSFXVolume()
		{
			return PlayerPrefs.HasKey(SoundFxVolKey) ? PlayerPrefs.GetFloat(SoundFxVolKey) : _soundFxVolume;
		}

		/// <summary>
		/// Converts the integer value to a boolean representative value
		/// </summary>
		private bool ToBool(int integer)
		{
			return integer == 0 ? false : true;
		}

		private bool LoadBGMMuteStatus()
		{
			return PlayerPrefs.HasKey(BgMusicMuteKey) ? ToBool(PlayerPrefs.GetInt(BgMusicMuteKey)) : _musicOn;
		}

		/// <summary>
		/// Get the mute or disabled status of the sound effect from disk
		/// </summary>
		/// <returns>Returns the value of the sound effect mute key from the saved preferences if it exists or the defaut value if it does not</returns>
		private bool LoadSFXMuteStatus()
		{
			return PlayerPrefs.HasKey(SoundFxMuteKey) ? ToBool(PlayerPrefs.GetInt(SoundFxMuteKey)) : _soundFxOn;
		}

		#region Public Player Prefs API

		/// <summary>
		/// Stores the volume and the mute status of the background music to disk.
		/// Note that all preferences would automatically get saved when this script gets destroyed 
		/// </summary>
		public void SaveBGMPreferences()
		{
			PlayerPrefs.SetInt(BgMusicMuteKey, _musicOn ? 1 : 0);
			PlayerPrefs.SetFloat(BgMusicVolKey, _musicVolume);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Stores the volume and the mute status of the sound effect to disk.
		/// Note that all preferences would automatically get saved when this script gets destroyed
		/// </summary>
		public void SaveSFXPreferences()
		{
			PlayerPrefs.SetInt(SoundFxMuteKey, _soundFxOn ? 1 : 0);
			PlayerPrefs.SetFloat(SoundFxVolKey, _soundFxVolume);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Removes all key and value pertaining to sound options from disk
		/// </summary>
		public void ClearAllPreferences()
		{
			PlayerPrefs.DeleteKey(BgMusicVolKey);
			PlayerPrefs.DeleteKey(SoundFxVolKey);
			PlayerPrefs.DeleteKey(BgMusicMuteKey);
			PlayerPrefs.DeleteKey(SoundFxMuteKey);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Writes all modified sound options or preferences to disk
		/// </summary>
		public void SaveAllPreferences()
		{
			PlayerPrefs.SetFloat(SoundFxVolKey, _soundFxVolume);
			PlayerPrefs.SetFloat(BgMusicVolKey, _musicVolume);
			PlayerPrefs.SetInt(SoundFxMuteKey, _soundFxOn ? 1 : 0);
			PlayerPrefs.SetInt(BgMusicMuteKey, _musicOn ? 1 : 0);
			PlayerPrefs.Save();
		}

		#endregion

		#endregion

		#region Playlist Functions

		/// <summary>
		/// Clear the list of audio clips
		/// </summary>
		public void EmptyPlaylist()
		{
			_playlist.Clear();
		}

		/// <summary>
		/// Add a sound clip to list of audio clips
		/// </summary>
		/// <param name="clip">Sound clip data</param>
		public void AddToPlaylist(AudioClip clip)
		{
			if (clip != null)
			{
				_playlist.Add(clip);
			}
		}

		/// <summary>
		/// Add a sound clip to asset list pool
		/// </summary>
		/// <param name="clip">Sound clip data</param>
		public void RemoveFromPlaylist(AudioClip clip)
		{
			if (clip != null && GetClipFromPlaylist(clip.name))
			{
				_playlist.Remove (clip);
				_playlist.Sort((x,y)=> x.name.CompareTo(y.name));
			}
		}

		/// <summary>
		/// Gets the AudioClip reference from the name supplied 
		/// </summary>
		/// <param name="clip_name">The name of the clip in the asset list pool </param>
		/// <returns>The AudioClip from the pool or null if no matching name can be found</returns>
		public AudioClip GetClipFromPlaylist(string clip_name)
		{
			// Search for each sound assets in the asset list pool 
			for(int i = 0; i < _playlist.Count; i++)
			{
				// Check if name is a match
				if (clip_name == _playlist[i].name)
				{
					return _playlist[i];
				}
			}

			Debug.LogWarning(clip_name +" does not exist in the playlist.");
			return null;
		}

		/// <summary>
		/// Load all sound clips from the Resources folder path into the asset list pool
		/// </summary>
		/// <param name="path">Pathname of the target folder. When using the empty string (i.e, ""), the function will load the entire audio clip content(s) of the resource folder</param>
		/// <param name="overwrite">Overwrites the current content(s) of the playlist.</param>
		public void LoadPlaylist(string path, bool overwrite)
		{
			// Get all clips from resource path
			AudioClip[] clips = Resources.LoadAll<AudioClip>(path);

			// Overwrite the current pool with the new one
			if (clips != null && clips.Length > 0 && overwrite)
			{
				_playlist.Clear();
			}

			// Add every loaded sound resource to the asset list pool
			for (int i = 0; i < clips.Length; i++)
			{
				_playlist.Add(clips[i]);
			}
		}

		#endregion
	}
}