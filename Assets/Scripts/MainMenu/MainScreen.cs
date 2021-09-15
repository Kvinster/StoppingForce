using UnityEngine;
using UnityEngine.UI;

using SF.Controllers;
using SF.Services;
using SF.State;

namespace SF.MainMenu {
	public sealed class MainScreen : MonoBehaviour {
		public Button StartGameButton;
		public Button LevelChoiceButton;
		public Button ExitButton;
		public Button ResetProgressButton;

		void Start() {
			StartGameButton.onClick.AddListener(OnStartGameClick);
			LevelChoiceButton.onClick.AddListener(OnLevelChoiceClick);
			ExitButton.onClick.AddListener(OnExitClick);
			ResetProgressButton.onClick.AddListener(OnResetProgressClick);
		}

		void OnStartGameClick() {
			var nextLevelIndex = GameState.Instance.NextLevelIndex;
			LevelController.Instance.StartLevel(nextLevelIndex);
			SceneService.LoadLevel(GameState.Instance.NextLevelIndex);
		}

		void OnLevelChoiceClick() {

		}

		void OnExitClick() {
			Application.Quit();
		}

		void OnResetProgressClick() {
			GameState.Instance.Reset();
		}
	}
}
