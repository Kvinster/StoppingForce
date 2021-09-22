// ReSharper disable InconsistentNaming

using UnityEngine;
using UnityEngine.Assertions;

using SF.LevelSelect;
using SF.Managers;
using SF.Utils;

using DG.Tweening;

using JetBrains.Annotations;

using NaughtyAttributes;

namespace SF.Gameplay {
	public sealed class GameplayCameraController : GameComponent {
		readonly struct CameraSettings : ILevelCameraSettings {
			public float   StartSize      { get; }
			public float   MinSize        { get; }
			public float   MaxSize        { get; }
			public Vector2 HorizontalZone { get; }
			public Vector2 VerticalZone   { get; }
			public Vector2 CentralPos     { get; }
			public Vector2 StartPos       { get; }
			public bool    PlayAnim       { get; }

			public CameraSettings(float startSize, float minSize, float maxSize, Vector2 horizontalZone,
				Vector2 verticalZone, Vector2 centralPos, Vector2 startPos, bool playAnim) {
				StartSize      = startSize;
				MinSize        = minSize;
				MaxSize        = maxSize;
				HorizontalZone = horizontalZone;
				VerticalZone   = verticalZone;
				CentralPos     = centralPos;
				StartPos       = startPos;
				PlayAnim       = playAnim;
			}
		}

		const float CameraSizeAnimDuration = 1f;

		static CameraSettings DefaultCameraSettings =>
			new CameraSettings(5f, 5f, 5f, Vector2.zero, Vector2.zero, Vector2.zero,  Vector2.zero, true);

		public Camera Camera;
		public float  ScrollSpeed = 1f;

		ILevelCameraSettings _curSettings;

		float _adjustedMaxSize;
		float _adjustedMinSize;

		PauseManager _pauseManager;

		Vector2 _prevMousePosition;

		Tween _sizeAnim;

		void Update() {
			if ( _pauseManager.IsPaused || (_sizeAnim?.IsActive() ?? false) ) {
				return;
			}
			var mouseScrollDelta = Input.mouseScrollDelta;
			if ( !Mathf.Approximately(mouseScrollDelta.y, 0f) ) {
				Camera.orthographicSize =
					Mathf.Clamp(Camera.orthographicSize + mouseScrollDelta.y * ScrollSpeed * Time.deltaTime,
						_adjustedMinSize, _adjustedMaxSize);
			} else if ( Input.GetMouseButton(1) ) {
				var mouseWorldPos    = Camera.ScreenToWorldPoint(Input.mousePosition);
				var oldMouseWorldPos = Camera.ScreenToWorldPoint(_prevMousePosition);
				var cameraTransform  = Camera.transform;
				var cameraPos        = cameraTransform.position + (oldMouseWorldPos - mouseWorldPos);
				cameraPos.x = Mathf.Clamp(cameraPos.x, _curSettings.CentralPos.x + _curSettings.HorizontalZone.x,
					_curSettings.CentralPos.x + _curSettings.HorizontalZone.y);
				cameraPos.y = Mathf.Clamp(cameraPos.y, _curSettings.CentralPos.y + _curSettings.VerticalZone.x,
					_curSettings.CentralPos.y + _curSettings.VerticalZone.y);
				cameraTransform.position = cameraPos;
			}

			_prevMousePosition = Input.mousePosition;
		}

		protected override void Init(GameStarter starter) {
			InitInternal(starter.PauseManager);
		}

		public void Init(LevelSelectStarter starter) {
			InitInternal(starter.PauseManager);
		}

		void InitInternal(PauseManager pauseManager) {
			_pauseManager = pauseManager;

			var levelSettings = LevelCameraSettings.Instance;
			if ( levelSettings ) {
				_curSettings = levelSettings;
			} else {
				_curSettings = DefaultCameraSettings;
			}
			Setup(_curSettings);
			TryStartSizeAnim();

			_adjustedMaxSize = CameraAspectHelper.GetAdjustedSize(_curSettings.MaxSize, Camera.aspect);
			_adjustedMinSize = CameraAspectHelper.GetAdjustedSize(_curSettings.MinSize, Camera.aspect);
		}

		void Setup(ILevelCameraSettings settings) {
			Assert.IsNotNull(settings);
			Camera.transform.position = new Vector3(settings.StartPos.x, settings.StartPos.y, -10f);
			CameraAspectHelper.SetAdjustedSize(Camera, settings.StartSize);
		}

		void TryStartSizeAnim() {
			if ( !_curSettings.PlayAnim ) {
				return;
			}
			Camera.orthographicSize = 0.05f;
			_sizeAnim = Camera.DOOrthoSize(_curSettings.StartSize, CameraSizeAnimDuration)
				.SetEase(Ease.OutSine)
				.OnComplete(() => _sizeAnim = null);
		}

		#region Editor

		[Button("Setup 4:3")]
		[ContextMenu("Setup 4:3")]
		[UsedImplicitly]
		void Setup4x3InEditor() {
			SetupInEditor(4f / 3f);
		}

		[Button("Setup 16:9")]
		[ContextMenu("Setup 16:9")]
		[UsedImplicitly]
		void Setup16x9InEditor() {
			SetupInEditor(16f / 9f);
		}

		[Button("Setup 22:9")]
		[ContextMenu("Setup 22:9")]
		[UsedImplicitly]
		void Setup22x9InEditor() {
			SetupInEditor(22f / 9f);
		}

		void SetupInEditor(float aspect) {
#if UNITY_EDITOR
			var oldSize = Camera.orthographicSize;
#endif
			CameraAspectHelper.SetAdjustedSize(Camera, DefaultCameraSettings.StartSize, aspect);
#if UNITY_EDITOR
			if ( !Application.isPlaying && !Mathf.Approximately(oldSize, Camera.orthographicSize) ) {
				UnityEditor.EditorUtility.SetDirty(Camera);
			}
#endif
		}

		void OnDrawGizmos() {
			var                  levelSettings = FindObjectOfType<LevelCameraSettings>();
			ILevelCameraSettings settings;
			if ( levelSettings ) {
				settings = levelSettings;
			} else {
				settings = DefaultCameraSettings;
			}
			DrawBorders(settings.StartPos, settings.StartSize, Vector2.zero, Vector2.zero, Color.white);
			DrawBorders(settings.CentralPos, settings.StartSize, settings.HorizontalZone, settings.VerticalZone,
				Color.yellow);
			DrawBorders(settings.CentralPos, settings.MaxSize, settings.HorizontalZone, settings.VerticalZone,
				Color.green);
			DrawBorders(settings.CentralPos, settings.MinSize, settings.HorizontalZone, settings.VerticalZone,
				Color.red);
		}

		void DrawBorders(Vector2 startPos, float cameraSize, Vector2 horZone, Vector2 verZone, Color color) {
			var oldColor = Gizmos.color;
			Gizmos.color = color;
			var cameraHeight = cameraSize * 2f;
			var cameraWidth  = Camera.aspect * cameraHeight;
			var horShift     = (horZone.x + horZone.y) / 2f;
			var verShift     = (verZone.x + verZone.y) / 2f;
			var shift        = new Vector2(horShift, verShift);
			var horSize      = Mathf.Abs(horZone.y - horZone.x);
			var verSize      = Mathf.Abs(verZone.y - verZone.x);
			Gizmos.DrawWireCube(startPos + shift, new Vector3(cameraWidth + horSize, cameraHeight + verSize, 1f));
			Gizmos.color = oldColor;
		}

		#endregion
	}
}
