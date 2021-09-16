using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections.Generic;

using RSG;

namespace SF.Utils {
	public sealed class UnityContext : BehaviourSingleton<UnityContext> {
		readonly List<Action> _updateCallbacks = new List<Action>();

		readonly List<PromiseTimer> _promiseTimers = new List<PromiseTimer>();
		readonly List<PromiseTimer> _toRemove = new List<PromiseTimer>();

		void Update() {
			foreach ( var callback in _updateCallbacks ) {
				callback();
			}
			foreach ( var promiseTimer in _promiseTimers ) {
				promiseTimer.Update(Time.unscaledDeltaTime);
			}
			_promiseTimers.RemoveAll(x => _toRemove.Contains(x));
			_toRemove.Clear();
		}

		public void AddUpdateCallback(Action callback) {
			Assert.IsNotNull(callback);
			_updateCallbacks.Add(callback);
		}

		public void RemoveUpdateCallback(Action callback) {
			_updateCallbacks.Remove(callback);
		}

		public IPromise Wait(float time) {
			var promiseTimer = new PromiseTimer();
			_promiseTimers.Add(promiseTimer);
			return promiseTimer.WaitFor(time).Then(() => _toRemove.Add(promiseTimer));
		}
	}
}
