using UnityEngine;

using SF.Gameplay;
using SF.LevelSelect;

namespace SF.Tutorial {
	public sealed class SimpleTutorial : BaseTutorial {
		public float MinShowTime = 2f;

		float _showTimer;

		bool _canHide;

		protected override void InitInternal(GameStarter _) {
			_showTimer = 0f;
		}

		protected override void InitInternal(LevelSelectStarter starter) {
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
	}
}
