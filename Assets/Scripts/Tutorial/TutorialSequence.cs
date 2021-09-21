using UnityEngine;

using System;

using SF.Gameplay;
using SF.Managers;

namespace SF.Tutorial {
	public sealed class TutorialSequence : BaseTutorial {
		[Serializable]
		public sealed class TutorialStepInfo {
			public GameObject     Root;
			public GameplayButton Button;
			public GoalArea       GoalArea;
		}

		public TutorialStepInfo[] StepInfos;

		LevelManager _levelManager;

		int _curStepIndex;

		void OnDestroy() {
			if ( _levelManager != null ) {
				_levelManager.OnLevelFinished -= OnLevelFinished;
			}
		}

		protected override void InitInternal(GameStarter starter) {
			_levelManager                 =  starter.LevelManager;
			_levelManager.OnLevelFinished += OnLevelFinished;

			foreach ( var stepInfo in StepInfos ) {
				stepInfo.Root.SetActive(false);
			}
			InitStep();
		}

		protected override void Hide() {
			if ( _levelManager != null ) {
				_levelManager.OnLevelFinished -= OnLevelFinished;
			}
			base.Hide();
		}

		void NextStep() {
			HideStep();
			++_curStepIndex;
			if ( _curStepIndex >= StepInfos.Length ) {
				// SetShown();
				Hide();
			} else {
				InitStep();
			}
		}

		void HideStep() {
			var stepInfo = StepInfos[_curStepIndex];
			stepInfo.Root.SetActive(false);
			if ( stepInfo.Button ) {
				stepInfo.Button.OnPressed -= OnGameplayButtonPressed;
			}
			if ( stepInfo.GoalArea ) {
				stepInfo.GoalArea.OnCurProgressChanged -= OnGoalAreaCurProgressChanged;
			}
		}

		void InitStep() {
			var stepInfo = StepInfos[_curStepIndex];
			stepInfo.Root.SetActive(true);
			if ( stepInfo.Button ) {
				stepInfo.Button.OnPressed += OnGameplayButtonPressed;
			}
			if ( stepInfo.GoalArea ) {
				stepInfo.GoalArea.OnCurProgressChanged += OnGoalAreaCurProgressChanged;
			}
		}

		void OnGameplayButtonPressed() {
			StepInfos[_curStepIndex].Button.OnPressed -= OnGameplayButtonPressed;
			NextStep();
		}

		void OnGoalAreaCurProgressChanged(float _) {
			StepInfos[_curStepIndex].GoalArea.OnCurProgressChanged -= OnGoalAreaCurProgressChanged;
			NextStep();
		}

		void OnLevelFinished(bool _) {
			if ( (_curStepIndex < 0) || (_curStepIndex >= StepInfos.Length) ) {
				return;
			}
			var stepInfo = StepInfos[_curStepIndex];
			if ( stepInfo.Button ) {
				stepInfo.Button.OnPressed -= OnGameplayButtonPressed;
			}
			if ( stepInfo.GoalArea ) {
				stepInfo.GoalArea.OnCurProgressChanged -= OnGoalAreaCurProgressChanged;
			}
		}
	}
}
