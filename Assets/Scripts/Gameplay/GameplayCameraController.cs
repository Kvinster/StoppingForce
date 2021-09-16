using UnityEngine;

namespace SF.Gameplay {
	public sealed class GameplayCameraController : MonoBehaviour {
		const float DefaultCameraSize   = 5f;
		const float DefaultCameraAspect = 16f / 9f;

		public Camera Camera;

		void Start() {
			var aspect = Camera.aspect;
			Camera.orthographicSize = aspect < DefaultCameraAspect
				? DefaultCameraSize * DefaultCameraAspect / aspect
				: DefaultCameraSize;
		}
	}
}
