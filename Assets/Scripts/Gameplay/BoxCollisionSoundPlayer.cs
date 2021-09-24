using UnityEngine;

using SF.Common;
using SF.Utils;

namespace SF.Gameplay {
	[RequireComponent(typeof(BaseBox))]
	[RequireComponent(typeof(Collider2D))]
	public sealed class BoxCollisionSoundPlayer : MonoBehaviour {
		public RandomSoundPlayer SoundPlayer;

		void OnCollisionEnter2D(Collision2D other) {
			SoundPlayer.Play();
		}
	}
}
