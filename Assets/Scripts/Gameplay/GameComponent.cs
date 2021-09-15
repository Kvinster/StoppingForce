using UnityEngine;

using System.Collections.Generic;

namespace SF.Gameplay {
	public abstract class GameComponent : MonoBehaviour {
		public static readonly List<GameComponent> Instances = new List<GameComponent>();

		bool _isInit;

		protected virtual void OnEnable() {
			Instances.Add(this);
		}

		protected virtual void OnDisable() {
			Instances.Remove(this);
		}

		public void TryInit(GameStarter starter) {
			if ( _isInit ) {
				return;
			}
			Init(starter);
			_isInit = true;
		}

		protected abstract void Init(GameStarter starter);
	}
}
