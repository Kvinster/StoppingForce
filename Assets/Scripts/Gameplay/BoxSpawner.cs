using UnityEngine;
using UnityEngine.Assertions;

using SF.Managers;

using Random = UnityEngine.Random;

namespace SF.Gameplay {
	public sealed class BoxSpawner : GameComponent, IBoxesSource {
		[Header("Parameters")]
		public int   TotalBoxesSerialized;
		public float PushForce;
		public float RotationForce;
		public float BoxStartHp = 100;
		[Header("Dependencies")]
		public GameObject BoxPrefab;
		public Transform SpawnOrigin;
		public Transform PushDirection;

		int _boxCount;

		LevelManager _levelManager;

		public int TotalBoxes => TotalBoxesSerialized;
		public int BoxesLeft  => TotalBoxes - _boxCount;

		protected override void Init(GameStarter starter) {
			_levelManager = starter.LevelManager;
			_levelManager.RegisterBoxesSource(this);
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

			var box = boxGo.GetComponent<Box>();
			Assert.IsTrue(box);
			box.Init(BoxStartHp);
			++_boxCount;

			_levelManager.OnBoxSpawned();
		}

		void OnDrawGizmos() {
			if ( !SpawnOrigin || !PushDirection ) {
				return;
			}
			Gizmos.DrawLine(SpawnOrigin.position, PushDirection.position);
		}
	}
}
