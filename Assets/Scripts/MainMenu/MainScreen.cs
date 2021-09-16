using UnityEngine;
using UnityEngine.UI;

using SF.Controllers;
using SF.Services;
using SF.State;

namespace SF.MainMenu {
	public sealed class MainScreen : MonoBehaviour {
		public Button StartGameButton;
		public Button ExitButton;
		public Button ResetProgressButton;

		void Start() {
			StartGameButton.onClick.AddListener(OnStartGameClick);
			ExitButton.onClick.AddListener(OnExitClick);
			ResetProgressButton.onClick.AddListener(OnResetProgressClick);

			Application.targetFrameRate = 60;

			PlayFabService.TryLogin();
		}

		void OnStartGameClick() {
			var nextLevelIndex = GameState.Instance.NextLevelIndex;
			LevelController.Instance.StartLevel(nextLevelIndex);
			SceneService.LoadLevel(GameState.Instance.NextLevelIndex);
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
