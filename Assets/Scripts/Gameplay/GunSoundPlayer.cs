using UnityEngine;

using SF.Services;

namespace SF.Gameplay {
	public sealed class GunSoundPlayer : MonoBehaviour {
		[Header("Parameters")]
		public float VolumeScale = 1f;
		[Header("Dependencies")]
		public Gun Gun;
		public AudioClip Clip;

		void OnDestroy() {
			if ( Gun ) {
				Gun.OnIsShootingChanged -= OnGunIsShootingChanged;
			}
			AudioService.TryStopInPool(this);
		}

		void Start() {
			Gun.OnIsShootingChanged += OnGunIsShootingChanged;
			OnGunIsShootingChanged(Gun.IsShooting);
		}

		void OnGunIsShootingChanged(bool isShooting) {
			if ( isShooting ) {
				AudioService.PlayInPool(this, Clip, true, VolumeScale);
			} else {
				AudioService.TryStopInPool(this);
			}
		}
	}
}
