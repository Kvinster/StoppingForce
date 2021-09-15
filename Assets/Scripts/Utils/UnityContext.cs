using UnityEngine.Assertions;

using System;
using System.Collections.Generic;

namespace SF.Utils {
	public sealed class UnityContext : BehaviourSingleton<UnityContext> {
		readonly List<Action> _updateCallbacks = new List<Action>();

		void Update() {
			foreach ( var callback in _updateCallbacks ) {
				callback();
			}
		}

		public void AddUpdateCallback(Action callback) {
			Assert.IsNotNull(callback);
			_updateCallbacks.Add(callback);
		}

		public void RemoveUpdateCallback(Action callback) {
			_updateCallbacks.Remove(callback);
		}
	}
}
