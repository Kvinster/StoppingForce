using UnityEngine;
using UnityEngine.Assertions;

namespace SF.Services {
	public static class AudioService {
		static AudioSource _audioSource;

		static AudioSource AudioSource {
			get {
				if ( !_audioSource ) {
					var go = new GameObject("[AudioService]");
					Object.DontDestroyOnLoad(go);
					_audioSource = go.AddComponent<AudioSource>();
				}
				return _audioSource;
			}
		}

		public static void PlaySound(AudioClip clip) {
			Assert.IsTrue(clip);
			AudioSource.PlayOneShot(clip);
		}
	}
}
