using UnityEngine;

using SF.Common;
using SF.Utils;

namespace SF.Gameplay {
	[RequireComponent(typeof(BaseBox))]
	[RequireComponent(typeof(Collider2D))]
	public sealed class BoxSoundsPlayer : MonoBehaviour {
		public BaseBox           Box;
		public RandomSoundPlayer DeathSoundPlayer;
		public RandomSoundPlayer CollisionSoundPlayer;

		void Start() {
			Box.OnDestroyed += OnBoxDestroyed;
		}

		void OnCollisionEnter2D(Collision2D _) {
			CollisionSoundPlayer.Play();
		}

		void OnBoxDestroyed(BaseBox box) {
			DeathSoundPlayer.Play();
			if ( box ) {
				box.OnDestroyed -= OnBoxDestroyed;
			}
		}
	}
}
