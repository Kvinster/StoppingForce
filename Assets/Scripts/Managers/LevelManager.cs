using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections.Generic;
using System.Linq;

using SF.Controllers;
using SF.Gameplay;
using SF.Utils;

namespace SF.Managers {
	public sealed class LevelManager {
		const float StationaryBoxesTime = 2f;

		public readonly int LevelIndex;

		readonly List<IProgressSource> _progressSources = new List<IProgressSource>();
		readonly List<IBoxesSource>    _boxesSources    = new List<IBoxesSource>();

		float _curProgress;
		float _stationaryBoxesTimer;

		public bool AllBoxesStationary { get; private set; }

		public float StationaryTimeLeft => Mathf.Max(StationaryBoxesTime - _stationaryBoxesTimer, 0f);
		public float StationaryProgress => Mathf.Clamp01(_stationaryBoxesTimer / StationaryBoxesTime);

		public int TotalProgressSources => _progressSources.Count;

		public bool IsGoalAchieved =>
			_progressSources.All(progressSource => progressSource.CurProgress >= progressSource.Goal);

		public bool CanFinishLevel {
			get {
				if ( !IsGoalAchieved ) {
					return false;
				}
				return (_stationaryBoxesTimer > StationaryBoxesTime);
			}
		}

		public int TotalBoxesLeft {
			get {
				var res = 0;
				foreach ( var boxesSource in _boxesSources ) {
					res += boxesSource.BoxesLeft;
				}
				return res;
			}
		}

		public int TotalBoxesUsed { get; private set; }

		public float TotalSourcesProgress {
			get {
				var res = 0f;
				foreach ( var progressSource in _progressSources ) {
					res += progressSource.CurProgress;
				}
				return res;
			}
		}

		public float TotalUnusedBoxesProgress {
			get {
				var res = 0f;
				foreach ( var boxesSource in _boxesSources ) {
					res += boxesSource.BoxesLeft * boxesSource.TotalBoxHp;
				}
				return res;
			}
		}

		public float TotalProgress => TotalSourcesProgress + TotalUnusedBoxesProgress;

		public event Action<int>        OnTotalBoxesLeftChanged;
		public event Action<int, float> OnProgressSourceProgressChanged;
		public event Action<bool>       OnLevelFinished;

		public LevelManager(int levelIndex) {
			LevelIndex = levelIndex;

			UnityContext.Instance.AddUpdateCallback(Update);
		}

		public void Deinit() {
			if ( UnityContext.Exists ) {
				UnityContext.Instance.RemoveUpdateCallback(Update);
			}
		}

		public void RegisterProgressSource(IProgressSource progressSource) {
			Assert.IsNotNull(progressSource);
			Assert.IsFalse(_progressSources.Contains(progressSource));
			_progressSources.Add(progressSource);
		}

		public float GetProgressSourceProgress(int progressSourceIndex) {
			if ( (progressSourceIndex < 0) || (progressSourceIndex >= _progressSources.Count) ) {
				Debug.LogErrorFormat("LevelManager.GetProgressSourceProgress: invalid progress source index '{0}'",
					progressSourceIndex);
				return -1;
			}
			var progressSource = _progressSources[progressSourceIndex];
			return Mathf.Clamp01(progressSource.CurProgress / progressSource.Goal);
		}

		public void OnCurProgressChanged(IProgressSource progressSource, float curProgress) {
			var progressSourceIndex = _progressSources.IndexOf(progressSource);
			Assert.IsTrue(progressSourceIndex >= 0);
			OnProgressSourceProgressChanged?.Invoke(progressSourceIndex, GetProgressSourceProgress(progressSourceIndex));
		}

		public void RegisterBoxesSource(IBoxesSource boxesSource) {
			Assert.IsNotNull(boxesSource);
			Assert.IsFalse(_boxesSources.Contains(boxesSource));
			_boxesSources.Add(boxesSource);
		}

		public void OnBoxSpawned() {
			++TotalBoxesUsed;
			OnTotalBoxesLeftChanged?.Invoke(TotalBoxesLeft);
		}

		public void FinishLevel(bool win) {
			if ( win ) {
				LevelController.Instance.OnLevelWon();
			} else {
				LevelController.Instance.OnLevelLost();
			}
			OnLevelFinished?.Invoke(win);
		}

		void Update() {
			AllBoxesStationary = true;
			foreach ( var box in GameplayBox.Instances ) {
				if ( !box.IsStationary ) {
					AllBoxesStationary = false;
				}
			}
			if ( AllBoxesStationary ) {
				_stationaryBoxesTimer += Time.deltaTime;
			} else {
				_stationaryBoxesTimer = 0f;
			}
		}
	}
}
