using UnityEngine;

using SF.Services;
using SF.Utils;

namespace SF.State {
	public sealed class GameState : Singleton<GameState> {
		const string NextLevelIndexKey   = "_NextLevelIndex";
		const string MaxLevelIndexKey    = "_MaxLevelIndex";
		const string TutorialKeyTemplate = "_Tutorial_{0}";

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

		public bool IsTutorialShown(string tutorialId) {
			return PlayerPrefs.HasKey(string.Format(TutorialKeyTemplate, tutorialId));
		}

		public void SetTutorialShown(string tutorialId) {
			PlayerPrefs.SetInt(string.Format(TutorialKeyTemplate, tutorialId), 1);
		}

		public void Reset() {
			var loginGuid = PlayerPrefs.HasKey(PlayFabService.GuidKey)
				? PlayerPrefs.GetString(PlayFabService.GuidKey)
				: null;
			PlayerPrefs.DeleteAll();
			if ( !string.IsNullOrEmpty(loginGuid) ) {
				PlayerPrefs.SetString(PlayFabService.GuidKey, loginGuid);
			}
		}
	}
}
