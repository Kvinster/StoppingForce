using UnityEngine;

using SF.Controllers;
using SF.Gameplay.UI;
using SF.Managers;

namespace SF.Gameplay {
	public sealed class GameStarter : MonoBehaviour {
		public LevelUiManager LevelUiManager;

		public PauseManager PauseManager { get; private set; }
		public LevelManager LevelManager { get; private set; }

		void Start() {
			PauseManager = new PauseManager();
			LevelManager = new LevelManager(LevelController.Instance.CurLevelIndex);

			foreach ( var comp in GameComponent.Instances ) {
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
