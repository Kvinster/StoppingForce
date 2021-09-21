using UnityEngine;

namespace SF.Utils {
	public static class CameraAspectHelper {
		const float DefaultCameraAspect = 16f / 9f;

		public static void SetAdjustedSize(Camera camera, float size) {
			SetAdjustedSize(camera, size, camera.aspect);
		}

		public static void SetAdjustedSize(Camera camera, float size, float aspect) {
			camera.orthographicSize = aspect < DefaultCameraAspect
				? size * DefaultCameraAspect / aspect
				: size;
		}

		public static float GetAdjustedSize(float size, float aspect) {
			return aspect < DefaultCameraAspect
				? size * DefaultCameraAspect / aspect
				: size;
		}
	}
}
