using UnityEngine;
using UnityEngine.Assertions;

namespace SF.Gameplay {
	public sealed class LevelCameraSettings : MonoBehaviour, ILevelCameraSettings {
		public static LevelCameraSettings Instance { get; private set; }

		public float   StartCameraSize      = 5f;
		public float   MinCameraSize        = 5f;
		public float   MaxCameraSize        = 5f;
		public Vector2 CameraHorizontalZone = Vector2.zero;
		public Vector2 CameraVerticalZone   = Vector2.zero;
		public Vector2 CameraCentralPos     = Vector2.zero;
		public Vector2 CameraStartPos       = Vector2.zero;

		public float   StartSize      => StartCameraSize;
		public float   MinSize        => MinCameraSize;
		public float   MaxSize        => MaxCameraSize;
		public Vector2 HorizontalZone => CameraHorizontalZone;
		public Vector2 VerticalZone   => CameraVerticalZone;
		public Vector2 CentralPos     => CameraCentralPos;
		public Vector2 StartPos       => CameraStartPos;

		void OnEnable() {
			Assert.IsFalse(Instance);
			Instance = this;
		}

		void OnDisable() {
			Assert.AreEqual(Instance, this);
			Instance = null;
		}
	}
}
