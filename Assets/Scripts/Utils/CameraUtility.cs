using UnityEngine;

namespace SF.Utils {
	public sealed class CameraUtility : BehaviourSingleton<CameraUtility> {
		public Camera MainCamera { get; private set; }

		void Update() {
			UpdateCamera();
		}

		protected override void Init() {
			UpdateCamera();
		}

		void UpdateCamera() {
			if ( !MainCamera ) {
				MainCamera = Camera.main;
			}
		}
	}
}
