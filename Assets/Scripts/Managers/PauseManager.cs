using UnityEngine;
using UnityEngine.Assertions;

using System.Collections.Generic;

namespace SF.Managers {
	public sealed class PauseManager {
		readonly HashSet<object> _pauseHolders = new HashSet<object>();

		public bool IsPaused { get; private set; }

		public void PauseBy(object holder) {
			Assert.IsNotNull(holder);
			_pauseHolders.Add(holder);
			UpdatePaused();
		}

		public void UnpauseBy(object holder) {
			Assert.IsNotNull(holder);
			_pauseHolders.Remove(holder);
			UpdatePaused();
		}

		public void Deinit() {
			_pauseHolders.Clear();
			Time.timeScale = 1f;
		}

		void UpdatePaused() {
			IsPaused = _pauseHolders.Count > 0;
			Time.timeScale = IsPaused ? 0f : 1f;
		}
	}
}
