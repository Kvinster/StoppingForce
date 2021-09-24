using UnityEngine;
using UnityEngine.Assertions;

using System.Collections.Generic;

namespace SF.Services {
	public static class AudioService {
		static GameObject  _gameObject;
		static AudioSource _oneShotAudioSource;

		static readonly List<AudioSource>               AudioSourcePool  = new List<AudioSource>();
		static readonly Dictionary<object, AudioSource> KeyToAudioSource = new Dictionary<object, AudioSource>();

		static GameObject GameObject {
			get {
				if ( !_gameObject ) {
					_gameObject = new GameObject("[AudioService]");
					Object.DontDestroyOnLoad(_gameObject);
				}
				return _gameObject;
			}
		}

		static AudioSource OneShotAudioSource {
			get {
				if ( !_oneShotAudioSource ) {
					_oneShotAudioSource = GameObject.AddComponent<AudioSource>();
				}
				return _oneShotAudioSource;
			}
		}

		public static void PlaySound(AudioClip clip, float volumeScale = 1f) {
			Assert.IsTrue(clip);
			OneShotAudioSource.PlayOneShot(clip, volumeScale);
		}

		public static void PlayInPool(object key, AudioClip clip, bool loop = false, float volumeScale = 1f) {
			Assert.IsTrue(clip);
			Assert.IsNotNull(key);
			Assert.IsFalse(KeyToAudioSource.ContainsKey(key));
			var audioSource = GetAudioSourceFromPool();
			KeyToAudioSource.Add(key, audioSource);
			audioSource.clip   = clip;
			audioSource.loop   = loop;
			audioSource.volume = volumeScale;
			audioSource.Play();
		}

		public static void TryStopInPool(object key) {
			if ( !KeyToAudioSource.ContainsKey(key) ) {
				return;
			}
			StopInPool(key);
		}

		public static void StopInPool(object key) {
			if ( !KeyToAudioSource.ContainsKey(key) ) {
				Debug.LogWarningFormat("AudioService.StopInPool: no key '{0}' in pool", key);
				return;
			}
			var audioSource = KeyToAudioSource[key];
			KeyToAudioSource.Remove(key);
			if ( !audioSource ) { // can happen when the game is closing
				return;
			}
			audioSource.Stop();
			audioSource.clip = null;
			AudioSourcePool.Add(audioSource);
		}

		static AudioSource GetAudioSourceFromPool() {
			AudioSource audioSource;
			if ( AudioSourcePool.Count > 0 ) {
				audioSource = AudioSourcePool[0];
				AudioSourcePool.RemoveAt(0);
			} else {
				audioSource = GameObject.AddComponent<AudioSource>();
			}
			return audioSource;
		}
	}
}
