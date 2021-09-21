using UnityEngine;
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

		public static int GetLevelIndexFromSceneName() {
			var sceneName = SceneManager.GetActiveScene().name;
			if ( !sceneName.StartsWith(LevelSceneNamePrefix) ) {
				Debug.LogErrorFormat("SceneService.GetLevelIndexFromSceneName: invalid scene name '{0}'", sceneName);
				return -1;
			}
			return int.Parse(sceneName.Substring(LevelSceneNamePrefix.Length));
		}
	}
}
