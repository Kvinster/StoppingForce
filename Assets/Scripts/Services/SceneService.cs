using UnityEngine.SceneManagement;

namespace SF.Services {
	public static class SceneService {
		const string LevelSceneNamePrefix = "Level_";

		public static void LoadLevel(int levelIndex) {
			SceneManager.LoadScene(LevelSceneNamePrefix + levelIndex);
			SceneManager.LoadScene("Level_Common", LoadSceneMode.Additive);
		}

		public static void LoadLevelSelect() {
			SceneManager.LoadScene("LevelSelect");
		}

		public static void LoadMainMenu() {
			SceneManager.LoadScene("MainMenu");
		}
	}
}
