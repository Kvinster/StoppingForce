using UnityEngine;
using UnityEngine.Assertions;

using System.Collections;

using SF.Managers;

using Random = UnityEngine.Random;

namespace SF.Gameplay {
	public sealed class BoxSpawner : GameComponent, IBoxesSource {
		[Header("Parameters")]
		public int   TotalBoxesSerialized;
		public float PushForce;
		public float RotationForce;
		public float BoxStartHp = 100;
		public float IgnoreCollisionTime = 1f;
		[Header("Dependencies")]
		public GameObject BoxPrefab;
		public Transform  SpawnOrigin;
		public Transform  PushDirection;
		public Transform  ViewTransform;
		public Collider2D BarrierCollider;

		int _boxCount;

		LevelManager _levelManager;

		public int   TotalBoxes => TotalBoxesSerialized;
		public int   BoxesLeft  => TotalBoxes - _boxCount;
		public float TotalBoxHp => BoxStartHp;

		protected override void Init(GameStarter starter) {
			_levelManager = starter.LevelManager;
			_levelManager.RegisterBoxesSource(this);

			var diff = (PushDirection.position - SpawnOrigin.position).normalized;
			var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			ViewTransform.rotation = Quaternion.Euler(0, 0, rotZ - 90);
		}

		void Update() {
			if ( (_boxCount < TotalBoxes) && Input.GetKeyDown(KeyCode.Space) ) {
				SpawnBox();
			}
		}

		void SpawnBox() {
			var boxGo = Instantiate(BoxPrefab, null, false);
			boxGo.transform.SetPositionAndRotation(SpawnOrigin.position,
				Quaternion.Euler(0f, 0f, Random.Range(0f, 2 * Mathf.PI)));
			var boxRb = boxGo.GetComponent<Rigidbody2D>();
			Assert.IsTrue(boxRb);
			boxRb.AddForce((PushDirection.position - SpawnOrigin.position).normalized * PushForce, ForceMode2D.Impulse);
			boxRb.AddTorque(RotationForce, ForceMode2D.Impulse);

			var box = boxGo.GetComponent<GameplayBox>();
			Assert.IsTrue(box);
			box.Init(BoxStartHp);
			++_boxCount;

			var boxCollider = boxGo.GetComponent<Collider2D>();
			Assert.IsTrue(boxCollider);
			Physics2D.IgnoreCollision(BarrierCollider, boxCollider, true);
			StartCoroutine(StopIgnoreCollisionCoro(boxCollider));

			_levelManager.OnBoxSpawned();
		}

		IEnumerator StopIgnoreCollisionCoro(Collider2D boxCollider) {
			yield return new WaitForSeconds(IgnoreCollisionTime);
			if ( boxCollider ) {
				Physics2D.IgnoreCollision(BarrierCollider, boxCollider, false);
			}
		}

		void OnDrawGizmos() {
			if ( !SpawnOrigin || !PushDirection ) {
				return;
			}
			Gizmos.DrawLine(SpawnOrigin.position, PushDirection.position);
		}
	}
}
