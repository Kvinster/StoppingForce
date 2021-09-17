using UnityEngine;
using UnityEngine.UI;

using SF.Controllers;
using SF.Services;
using SF.State;

using TMPro;

namespace SF.MainMenu {
	public sealed class MainScreen : MonoBehaviour {
		const string VersionTextTemplate = "Version:{0}";

		[Header("Parameters")]
		public float LoadingAnimSpeed;
		[Header("Dependencies")]
		public Button StartGameButton;
		public Button   LevelSelectButton;
		public Button   ExitButton;
		public Button   ResetProgressButton;
		public Button   ChangeDisplayNameButton;
		public TMP_Text VersionText;
		[Space]
		public LoadingWindow LoadingWindow;
		[Space]
		public NameRequestWindow NameRequestWindow;

		void Start() {
			StartGameButton.onClick.AddListener(OnStartGameClick);
			LevelSelectButton.onClick.AddListener(OnLevelSelectClick);
			ExitButton.onClick.AddListener(OnExitClick);
			ResetProgressButton.onClick.AddListener(OnResetProgressClick);
			ChangeDisplayNameButton.onClick.AddListener(OnChangeDisplayNameClick);

			NameRequestWindow.Hide();
			LoadingWindow.Hide();

			VersionText.text = string.Format(VersionTextTemplate, Application.version);

			Application.targetFrameRate = 60;

			if ( !PlayFabService.IsLoggedIn ) {
				ChangeDisplayNameButton.gameObject.SetActive(false);
				LoadingWindow.Show();
				PlayFabService.TryLogin()
					.Then(() => {
						LoadingWindow.Hide();
						if ( string.IsNullOrEmpty(PlayFabService.DisplayName) ) {
							NameRequestWindow.Show();
						}
						ChangeDisplayNameButton.gameObject.SetActive(true);
					})
					.Catch(exception => {
						Debug.LogErrorFormat("MainScreen.Start: login fail\n{0}", exception.Message);
						LoadingWindow.Hide();
					});
			}
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

		void OnChangeDisplayNameClick() {
			NameRequestWindow.Show();
		}
	}
}
