using UnityEngine;

using SF.Utils;

namespace SF.State {
	public sealed class GameState : Singleton<GameState> {
		const string NextLevelIndexKey = "_NextLevelIndex";

		public int NextLevelIndex {
			get => PlayerPrefs.GetInt(NextLevelIndexKey);
			set => PlayerPrefs.SetInt(NextLevelIndexKey, value);
		}

		public void Reset() {
			PlayerPrefs.DeleteAll();
		}
	}
}
