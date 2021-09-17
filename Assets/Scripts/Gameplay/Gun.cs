using UnityEngine;
using UnityEngine.EventSystems;

using SF.Common;
using SF.LevelSelect;
using SF.Managers;
using SF.Utils;

namespace SF.Gameplay {
	public sealed class Gun : GameComponent {
		public Transform BarrelTransform;
		public Transform BarrelEndTransform;
		[Space]
		public Rigidbody2D Rigidbody;
		[Header("Parameters")]
		public float PushForce;
		public float Dps = 10;
		public bool  CanMove;
		public float PushMovementForce = 1f;
		public float ManualMovementForce = 3f;
		[Header("Line")]
		public LineRenderer LineRenderer;
		public Color HitColor   = Color.green;
		public Color NoHitColor = Color.yellow;

		Camera _camera;

		PauseManager _pauseManager;

		float _hor;

		RaycastHit2D? _hit;

		protected override void Init(GameStarter starter) {
			_camera                    = CameraUtility.Instance.MainCamera;
			_pauseManager              = starter.PauseManager;
			LineRenderer.positionCount = 2;
		}

		public void Init(LevelSelectStarter starter) {
			_camera                    = CameraUtility.Instance.MainCamera;
			_pauseManager              = starter.PauseManager;
			LineRenderer.positionCount = 2;
		}

		void FixedUpdate() {
			UpdateRaycast();
			TryMove();
		}

		void Update() {
			if ( _pauseManager?.IsPaused ?? false ) {
				return;
			}

			if ( CanMove ) {
				_hor = Input.GetAxisRaw("Horizontal");
			}

			var mousePos = (Vector3) (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition);
			var diff     = mousePos - BarrelTransform.position;
			diff.Normalize();

			var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			BarrelTransform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
		}

		void LateUpdate() {
			if ( _hit.HasValue ) {
				var hit = _hit.Value;
				LineRenderer.enabled = true;
				var barrelPos = BarrelTransform.position;
				var mousePos  = (Vector3) (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition);
				if ( !hit.rigidbody || (hit.rigidbody == Rigidbody) ) {
					LineRenderer.startColor = LineRenderer.endColor = NoHitColor;
					if ( hit.collider ) {
						LineRenderer.SetPositions(new[] { barrelPos, (Vector3) hit.point });
					} else {
						LineRenderer.SetPositions(new[] {
							barrelPos, barrelPos + (mousePos - barrelPos).normalized * 100f
						});
					}
					return;
				}
				LineRenderer.startColor = LineRenderer.endColor = HitColor;
				LineRenderer.SetPositions(new[] { barrelPos, (Vector3) hit.point });
			} else {
				LineRenderer.enabled = false;
			}
		}

		void UpdateRaycast() {
			if ( Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() ) {
				var barrelPos = BarrelTransform.position;
				var mousePos  = (Vector3) (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition);
				if ( Vector2.Distance(mousePos, barrelPos) <= 1f ) {
					_hit = null;
					return;
				}
				_hit = Physics2D.Raycast(barrelPos, (mousePos - barrelPos).normalized, 1000f,
					LayerMask.GetMask("Box", "Default"));
				var hit = _hit.Value;
				if ( !hit.rigidbody ) {
					if ( CanMove && Rigidbody && hit.collider ) {
						Rigidbody.AddForceAtPosition((Rigidbody.position - hit.point).normalized * PushMovementForce,
							Rigidbody.ClosestPoint(hit.point));
					}
					return;
				}

				hit.rigidbody.AddForceAtPosition((hit.point - (Vector2) barrelPos).normalized * PushForce, hit.point,
					ForceMode2D.Force);
				var box = hit.rigidbody.gameObject.GetComponent<BaseBox>();
				if ( box ) {
					box.TakeDamage(Dps * Time.fixedDeltaTime);
				}
			} else {
				_hit = null;
			}
		}

		void TryMove() {
			if ( !CanMove || !Rigidbody || Mathf.Approximately(_hor, 0f) ) {
				return;
			}
			Rigidbody.AddForce(new Vector2(_hor, 0) * ManualMovementForce, ForceMode2D.Impulse);
			_hor = 0f;
		}
	}
}
