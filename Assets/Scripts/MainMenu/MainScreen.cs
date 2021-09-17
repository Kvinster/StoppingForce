using UnityEngine;
using UnityEngine.UI;

using SF.Controllers;
using SF.Services;
using SF.State;

using TMPro;

namespace SF.MainMenu {
	public sealed class MainScreen : MonoBehaviour {
		const string VersionTextTemplate = "Version:{0}";

		public Button   StartGameButton;
		public Button   LevelSelectButton;
		public Button   ExitButton;
		public Button   ResetProgressButton;
		public TMP_Text VersionText;

		void Start() {
			StartGameButton.onClick.AddListener(OnStartGameClick);
			LevelSelectButton.onClick.AddListener(OnLevelSelectClick);
			ExitButton.onClick.AddListener(OnExitClick);
			ResetProgressButton.onClick.AddListener(OnResetProgressClick);

			VersionText.text = string.Format(VersionTextTemplate, Application.version);

			Application.targetFrameRate = 60;

			PlayFabService.TryLogin();
		}

		void OnStartGameClick() {
			var nextLevelIndex = GameState.Instance.NextLevelIndex;
			LevelController.Instance.StartLevel(nextLevelIndex);
			SceneService.LoadLevel(GameState.Instance.NextLevelIndex);
		}

		void OnLevelSelectClick() {
			SceneService.LoadLevelSelect();
		}

		void OnExitClick() {
			if ( Application.isEditor ) {
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
			} else {
				Application.Quit();
			}
		}

		void OnResetProgressClick() {
			GameState.Instance.Reset();
		}
	}
}
