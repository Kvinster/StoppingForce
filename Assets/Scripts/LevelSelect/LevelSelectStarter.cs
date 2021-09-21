using UnityEngine;
using UnityEngine.Assertions;

using System;

using SF.Configs;
using SF.Controllers;
using SF.Gameplay;
using SF.Managers;
using SF.Services;
using SF.State;

namespace SF.LevelSelect {
	public sealed class LevelSelectStarter : MonoBehaviour {
		[Header("Parameters")]
		public int MaxLevelsBeforeExpansion = 8;
		public float   CeilingMinWidth = 19;
		public float   RoomWidthStep;
		public Vector2 RightWallStartPos;
		public Vector2 LevelComplexStartLocalPos;
		public Vector2 LevelComplexStep;
		[Space]
		public float BoxStartHp = 100;
		[Header("Dependencies")]
		public Gun Gun;
		public GameplayCameraController GameplayCameraController;
		[Space]
		public Transform CeilingTransform;
		public Transform  FloorTransform;
		public Transform  RightWallTransform;
		public Transform  LevelComplexParent;
		public GameObject LevelComplexPrefab;

		public PauseManager PauseManager { get; private set; }

		void Start() {
			PauseManager = new PauseManager();

			var totalLevels   = LevelsConfig.Instance.TotalLevels;
			var maxLevelIndex = GameState.Instance.MaxLevelIndex;

			Gun.Init(this);
			GameplayCameraController.Init(this);

			if ( totalLevels > MaxLevelsBeforeExpansion ) {
				throw new NotImplementedException(); // TODO: implement, duh
			}

			for ( var i = 0; i < totalLevels; ++i ) {
				var levelComplexGo = Instantiate(LevelComplexPrefab, LevelComplexParent, false);
				levelComplexGo.transform.localPosition = LevelComplexStartLocalPos + i * LevelComplexStep;
				var levelComplex = levelComplexGo.GetComponent<LevelComplex>();
				Assert.IsTrue(levelComplex);
				levelComplex.Box.Init(BoxStartHp, i > maxLevelIndex);
				var levelIndex = i;
				levelComplex.Box.OnDestroyed += () => LoadLevel(levelIndex);
				levelComplex.LevelLabel.Init(i, i > maxLevelIndex);
				levelComplex.BoxJoint.connectedAnchor = levelComplex.ConnectionPos.position;
			}
		}

		void LoadLevel(int levelIndex) {
			LevelController.Instance.StartLevel(levelIndex);
			SceneService.LoadLevel(levelIndex);
		}
	}
}
