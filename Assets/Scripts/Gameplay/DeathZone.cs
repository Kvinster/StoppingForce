using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using SF.Managers;

namespace SF.Gameplay {
	public sealed class DeathZone : GameComponent {
		sealed class BoxTimer {
			public readonly Box   Box;
			public          float Timer;

			public BoxTimer(Box box) {
				Box   = box;
				Timer = 0f;
			}
		}

		public float DeathTime = 3f;

		readonly List<BoxTimer> _boxTimers = new List<BoxTimer>();

		PauseManager _pauseManager;

		protected override void Init(GameStarter starter) {
			_pauseManager = starter.PauseManager;
		}

		void Update() {
			if ( _pauseManager.IsPaused ) {
				return;
			}
			for ( var i = _boxTimers.Count - 1; i >= 0; i-- ) {
				var boxTimer = _boxTimers[i];
				boxTimer.Timer += Time.deltaTime;
				if ( boxTimer.Timer >= DeathTime ) {
					_boxTimers.RemoveAt(i);
					boxTimer.Box.Die();
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			var box = other.GetComponentInChildren<Box>();
			if ( !box || _boxTimers.Any(x => x.Box == box) ) {
				return;
			}
			_boxTimers.Add(new BoxTimer(box));
		}

		void OnTriggerExit2D(Collider2D other) {
			var box = other.GetComponentInChildren<Box>();
			if ( !box ) {
				return;
			}
			for ( var i = _boxTimers.Count - 1; i >= 0; --i ) {
				var boxTimer = _boxTimers[i];
				if ( boxTimer.Box == box ) {
					_boxTimers.RemoveAt(i);
				}
			}
		}
	}
}
