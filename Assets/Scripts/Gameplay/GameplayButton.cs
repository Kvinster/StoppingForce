using UnityEngine;

using System;
using System.Collections.Generic;

using SF.Utils;

namespace SF.Gameplay {
	public sealed class GameplayButton : GameComponent {
		public ColliderNotifier2D Notifier;
		public Collider2D         NotifierCollider;
		public Collider2D[]       IgnoreColliders;

		readonly HashSet<GameObject> _pressables = new HashSet<GameObject>();

		public bool IsPressed { get; private set; }

		public event Action OnPressed;
		public event Action OnReleased;

		protected override void Init(GameStarter starter) {
			Notifier.OnTriggerEnter += OnNotifierEnter;
			Notifier.OnTriggerExit  += OnNotifierExit;

			foreach ( var ignoreCollider in IgnoreColliders ) {
				Physics2D.IgnoreCollision(NotifierCollider, ignoreCollider, true);
			}

			UpdatePressed();
		}

		void OnNotifierEnter(GameObject go) {
			if ( _pressables.Contains(go) ) {
				return;
			}
			_pressables.Add(go);
			UpdatePressed();
		}

		void OnNotifierExit(GameObject go) {
			if ( !_pressables.Contains(go) ) {
				return;
			}
			_pressables.Remove(go);
			UpdatePressed();
		}

		void UpdatePressed() {
			var oldPressed = IsPressed;
			IsPressed = _pressables.Count > 0;
			if ( IsPressed == oldPressed ) {
				return;
			}
			if ( IsPressed ) {
				OnPressed?.Invoke();
			} else {
				OnReleased?.Invoke();
			}
		}
	}
}
