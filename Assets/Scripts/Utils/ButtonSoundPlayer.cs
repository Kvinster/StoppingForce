using UnityEngine;
using UnityEngine.UI;

namespace SF.Utils {
	[RequireComponent(typeof(Button))]
	public sealed class ButtonSoundPlayer : MonoBehaviour {
		public AudioClip ClickSound;

		Button      _button;
		AudioSource _audioSource;

		Button Button {
			get {
				if ( !_button ) {
					_button = GetComponent<Button>();
				}
				return _button;
			}
		}

		AudioSource AudioSource {
			get {
				if ( !_audioSource ) {
					_audioSource = gameObject.AddComponent<AudioSource>();
				}
				return _audioSource;
			}
		}

		void OnEnable() {
			Button.onClick.AddListener(OnClick);
		}

		void OnDisable() {
			Button.onClick.RemoveListener(OnClick);
		}

		void OnClick() {
			AudioSource.PlayOneShot(ClickSound);
		}
	}
}
