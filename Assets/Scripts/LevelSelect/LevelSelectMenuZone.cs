using UnityEngine;

using System;

using SF.Gameplay;

namespace SF.LevelSelect {
	[RequireComponent(typeof(Collider2D))]
	public sealed class LevelSelectMenuZone : MonoBehaviour {
		public event Action OnGunEnter;

		void OnTriggerEnter2D(Collider2D other) {
			if ( other.GetComponentInChildren<Gun>() ) {
				OnGunEnter?.Invoke();
			}
		}
	}
}
