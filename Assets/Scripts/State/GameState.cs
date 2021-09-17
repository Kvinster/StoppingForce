using UnityEngine;

using SF.Utils;

namespace SF.State {
	public sealed class GameState : Singleton<GameState> {
		const string NextLevelIndexKey = "_NextLevelIndex";
		const string MaxLevelIndexKey  = "_MaxLevelIndex";

		public int NextLevelIndex {
			get => PlayerPrefs.GetInt(NextLevelIndexKey);
			set {
				PlayerPrefs.SetInt(NextLevelIndexKey, value);
				if ( value > MaxLevelIndex ) {
					MaxLevelIndex = value;
				}
			}
		}

		public int MaxLevelIndex {
			get => PlayerPrefs.GetInt(MaxLevelIndexKey);
			set => PlayerPrefs.SetInt(MaxLevelIndexKey, value);
		}

		public void Reset() {
			PlayerPrefs.DeleteAll();
		}
	}
}
