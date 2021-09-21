using UnityEngine;

using System.Collections.Generic;

using SF.Controllers;
using SF.Gameplay.UI;
using SF.Managers;
using SF.Services;
using SF.State;

namespace SF.Gameplay {
	public sealed class GameStarter : MonoBehaviour {
		public LevelUiManager LevelUiManager;

		public PauseManager PauseManager { get; private set; }
		public LevelManager LevelManager { get; private set; }

		void Start() {
#if UNITY_EDITOR
			if ( !LevelController.Instance.IsLevelActive ) {
				var levelIndex = SceneService.GetLevelIndexFromSceneName();
				if ( GameState.Instance.MaxLevelIndex < levelIndex ) {
					GameState.Instance.MaxLevelIndex = levelIndex;
				}
				LevelController.Instance.StartLevel(levelIndex);
			}
#endif
			PauseManager = new PauseManager();
			LevelManager = new LevelManager(LevelController.Instance.CurLevelIndex);

			var comps = new List<GameComponent>(GameComponent.Instances);
			foreach ( var comp in comps ) {
				comp.TryInit(this);
			}

			LevelUiManager.Init(this);
		}

		void OnDestroy() {
			PauseManager.Deinit();
			LevelManager.Deinit();
		}
	}
}
