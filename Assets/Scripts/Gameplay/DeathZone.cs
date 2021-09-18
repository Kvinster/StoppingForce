using UnityEngine;

using System.Collections.Generic;

using SF.Common;
using SF.Managers;

namespace SF.Gameplay {
	public sealed class DeathZone : GameComponent {
		public float Dps = 100f;

		readonly List<BaseBox> _boxes = new List<BaseBox>();

		PauseManager _pauseManager;

		protected override void Init(GameStarter starter) {
			_pauseManager = starter.PauseManager;
		}

		void Update() {
			if ( _pauseManager.IsPaused ) {
				return;
			}
			for ( var i = _boxes.Count - 1; i >= 0; i-- ) {
				var box = _boxes[i];
				box.TakeDamage(Dps * Time.deltaTime);
				if ( !box ) {
					_boxes.RemoveAt(i);
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			var box = other.GetComponentInChildren<BaseBox>();
			if ( !box || _boxes.Contains(box) ) {
				return;
			}
			_boxes.Add(box);
		}

		void OnTriggerExit2D(Collider2D other) {
			var box = other.GetComponentInChildren<BaseBox>();
			if ( !box ) {
				return;
			}
			_boxes.Remove(box);
		}
	}
}
