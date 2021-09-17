using System;
using System.Collections.Generic;

using SF.Common;

namespace SF.Gameplay {
	public sealed class GameplayBox : BaseBox {
		const float MaxStationaryBoxSpeed = 0.25f;

		public static readonly HashSet<GameplayBox> Instances = new HashSet<GameplayBox>();

		public bool IsStationary { get; private set; }

		public event Action<GameplayBox> OnDestroyed;

		void OnEnable() {
			Instances.Add(this);
		}

		void OnDisable() {
			Instances.Remove(this);
		}

		void OnDestroy() {
			OnDestroyed?.Invoke(this);
		}

		void FixedUpdate() {
			IsStationary = Rigidbody.velocity.magnitude <= MaxStationaryBoxSpeed;
		}

		public void Init(float startHp) => InitInternal(startHp);
	}
}
