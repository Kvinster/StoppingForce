using UnityEngine;

using SF.State;

namespace SF.Tutorial {
	public sealed class SimpleTutorial : MonoBehaviour {
		public float  MinShowTime = 2f;
		public string Id;

		float _showTimer;

		bool _canHide;

		void Start() {
			if ( GameState.Instance.IsTutorialShown(Id) ) {
				Hide();
				return;
			}
			_showTimer = 0f;
		}

		void Update() {
			if ( !_canHide ) {
				_showTimer += Time.deltaTime;
				if ( _showTimer >= MinShowTime ) {
					_canHide = true;
				}
			}
			if ( _canHide && Input.anyKey ) {
				SetShown();
				Hide();
			}
		}

		void SetShown() {
			GameState.Instance.SetTutorialShown(Id);
		}

		void Hide() {
			Destroy(gameObject);
		}
	}
}
