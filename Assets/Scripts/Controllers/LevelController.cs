using UnityEngine;
using UnityEngine.Assertions;

using SF.Configs;
using SF.State;
using SF.Utils;

namespace SF.Controllers {
	public sealed class LevelController : Singleton<LevelController> {
		public int CurLevelIndex { get; private set; } = -1;

		public bool IsLevelActive => (CurLevelIndex != -1);

		public void StartLevel(int levelIndex) {
			Assert.IsFalse(IsLevelActive);
			CurLevelIndex = levelIndex;
		}

		public void FinishLevel(bool win) {
			Assert.IsTrue(IsLevelActive);
			if ( win ) {
				GameState.Instance.NextLevelIndex = Mathf.Clamp(GameState.Instance.NextLevelIndex + 1, 0,
					LevelsConfig.Instance.TotalLevels - 1);
			}
			CurLevelIndex = -1;
		}

		public bool HasLevel(int levelIndex) {
			if ( levelIndex < 0 ) {
				Debug.LogErrorFormat("LevelController.HasLevel: invalid level index '{0}'", levelIndex);
				return false;
			}
			return (levelIndex < LevelsConfig.Instance.TotalLevels);
		}
	}
}
