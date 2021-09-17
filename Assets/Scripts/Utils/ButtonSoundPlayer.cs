using UnityEngine;
using UnityEngine.UI;

using SF.Services;

namespace SF.Utils {
	[RequireComponent(typeof(Button))]
	public sealed class ButtonSoundPlayer : MonoBehaviour {
		public AudioClip ClickSound;

		Button _button;

		Button Button {
			get {
				if ( !_button ) {
					_button = GetComponent<Button>();
				}
				return _button;
			}
		}

		void OnEnable() {
			Button.onClick.AddListener(OnClick);
		}

		void OnDisable() {
			Button.onClick.RemoveListener(OnClick);
		}

		void OnClick() {
			AudioService.PlaySound(ClickSound);
		}
	}
}
