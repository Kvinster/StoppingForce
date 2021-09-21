using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using SF.Controllers;
using SF.Managers;
using SF.Services;
using SF.Utils;

using DG.Tweening;
using TMPro;

namespace SF.Gameplay.UI {
	public sealed class LevelUiManager : MonoBehaviour {
		const string BoxesLeftTextTemplate = "Boxes left: <b><size=125%>{0}</size></b>";

		[Header("Parameters")]
		public float GoalAnimSpeed = 1f;
		[Header("Dependencies")]
		public Canvas Canvas;
		public WinWindow WinWindow;
		[Header("Top UI")]
		public List<Image> GoalFillImages;
		public TMP_Text   BoxesLeftText;
		public GameObject FinishLevelButtonRoot;
		public Button     FinishLevelButton;
		public GameObject TimerRoot;
		public Image      TimerImage;
		public TMP_Text   TimerText;
		public Button     RestartButton;
		public Button     MenuButton;

		PauseManager _pauseManager;
		LevelManager _levelManager;

		float[] _curViewProgresses;
		Tween[] _goalAnims;

		void Start() {
			Canvas.worldCamera = CameraUtility.Instance.MainCamera;
			WinWindow.Hide();
		}

		void OnDestroy() {
			if ( _levelManager != null ) {
				_levelManager.OnProgressSourceProgressChanged -= OnCurProgressChanged;
			}
			foreach ( var goalAnim in _goalAnims ) {
				goalAnim?.Kill();
			}
		}

		void Update() {
			if ( _levelManager.CanFinishLevel ) {
				FinishLevelButtonRoot.SetActive(true);
				TimerRoot.SetActive(false);
			} else {
				FinishLevelButtonRoot.SetActive(false);
				if ( _levelManager.IsGoalAchieved && _levelManager.AllBoxesStationary ) {
					TimerRoot.SetActive(true);
					TimerText.text        = _levelManager.StationaryTimeLeft.ToString("F1");
					TimerImage.fillAmount = _levelManager.StationaryProgress;
				} else {
					TimerRoot.SetActive(false);
				}
			}
			if ( LevelController.Instance.IsLevelActive && Input.GetKeyDown(KeyCode.R) ) {
				OnRestartLevelClick();
			}
		}

		public void Init(GameStarter starter) {
			_pauseManager = starter.PauseManager;
			_levelManager = starter.LevelManager;

			_levelManager.OnTotalBoxesLeftChanged += OnTotalBoxesLeftChanged;
			OnTotalBoxesLeftChanged(_levelManager.TotalBoxesLeft);

			_levelManager.OnProgressSourceProgressChanged += OnCurProgressChanged;

			_curViewProgresses = new float[_levelManager.TotalProgressSources];
			_goalAnims         = new Tween[_levelManager.TotalProgressSources];
			int i;
			for ( i = 0; i < _levelManager.TotalProgressSources; ++i ) {
				GoalFillImages[i].gameObject.SetActive(true);
				_curViewProgresses[i]        = 0f;
				_goalAnims[i]                = null;
				GoalFillImages[i].fillAmount = 0f;
			}
			for ( ; i < GoalFillImages.Count; ++i ) {
				GoalFillImages[i].gameObject.SetActive(false);
			}

			FinishLevelButton.onClick.AddListener(OnFinishLevelClick);
			RestartButton.onClick.AddListener(OnRestartLevelClick);
			MenuButton.onClick.AddListener(OnMenuClick);
		}

		void OnCurProgressChanged(int progressSourceIndex, float curProgress) {
			var goalAnim = _goalAnims[progressSourceIndex];
			goalAnim?.Kill(false);
			_goalAnims[progressSourceIndex] = DOTween.To(() => _curViewProgresses[progressSourceIndex], x => {
				_curViewProgresses[progressSourceIndex]        = x;
				GoalFillImages[progressSourceIndex].fillAmount = x;
			}, curProgress, Mathf.Abs(curProgress - _curViewProgresses[progressSourceIndex]) / GoalAnimSpeed);
		}

		void OnTotalBoxesLeftChanged(int boxesLeft) {
			BoxesLeftText.text = string.Format(BoxesLeftTextTemplate, boxesLeft);
		}

		void OnFinishLevelClick() {
			foreach ( var goalAnim in _goalAnims ) {
				goalAnim?.Kill(true);
			}
			_pauseManager.PauseBy(this);
			_levelManager.FinishLevel(true);
			WinWindow.Show(_levelManager);
		}

		void OnRestartLevelClick() {
			foreach ( var goalAnim in _goalAnims ) {
				goalAnim?.Kill(true);
			}
			_pauseManager.PauseBy(this);
			_levelManager.FinishLevel(false);
			LevelController.Instance.StartLevel(_levelManager.LevelIndex);
			SceneService.LoadLevel(_levelManager.LevelIndex);
		}

		void OnMenuClick() {
			foreach ( var goalAnim in _goalAnims ) {
				goalAnim?.Kill(true);
			}
			_pauseManager.PauseBy(this);
			_levelManager.FinishLevel(false);
			SceneService.LoadMainMenu();
		}
	}
}
