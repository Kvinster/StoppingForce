using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

using SF.Managers;
using SF.Utils;

namespace SF.Gameplay {
	public sealed class Gun : GameComponent {
		public Transform BarrelTransform;
		public Transform BarrelEndTransform;
		[Space] [Header("Parameters")]
		public float PushForce;
		public float Dps = 10;
		[Space] [Header("Line")]
		public LineRenderer LineRenderer;
		public Color HitColor   = Color.green;
		public Color NoHitColor = Color.yellow;

		Camera _camera;

		PauseManager _pauseManager;

		protected override void Init(GameStarter starter) {
			_camera       = CameraUtility.Instance.MainCamera;
			_pauseManager = starter.PauseManager;
		}

		void Update() {
			if ( _pauseManager.IsPaused ) {
				return;
			}
			var mousePos = (Vector3) (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition);
			var diff     = mousePos - BarrelTransform.position;
			diff.Normalize();

			var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			BarrelTransform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);


			UpdateRaycast();
		}

		void UpdateRaycast() {
			if ( Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() ) {
				LineRenderer.enabled       = true;
				LineRenderer.positionCount = 2;

				var barrelPos = BarrelEndTransform.position;
				var mousePos  = (Vector3) (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition);
				var hit = Physics2D.Raycast(barrelPos, (mousePos - barrelPos).normalized, 1000f,
					LayerMask.GetMask("Box", "Default"));
				if ( !hit.rigidbody ) {
					LineRenderer.startColor = LineRenderer.endColor = NoHitColor;
					LineRenderer.SetPositions(new[] {
						barrelPos, barrelPos + (mousePos - barrelPos).normalized * 100f
					});
					return;
				}

				hit.rigidbody.AddForceAtPosition((hit.point - (Vector2) barrelPos).normalized * PushForce, hit.point,
					ForceMode2D.Force);
				var box = hit.rigidbody.gameObject.GetComponent<Box>();
				Assert.IsTrue(box);
				box.TakeDamage(Dps * Time.deltaTime);

				LineRenderer.startColor = LineRenderer.endColor = HitColor;
				LineRenderer.SetPositions(new[] { barrelPos, (Vector3) hit.point });
			} else {
				LineRenderer.enabled = false;
			}
		}
	}
}
