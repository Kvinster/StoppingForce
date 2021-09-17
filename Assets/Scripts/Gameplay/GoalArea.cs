using System;

using UnityEngine;

using System.Collections.Generic;

using SF.Managers;
using SF.Utils;

namespace SF.Gameplay {
	public sealed class GoalArea : GameComponent, IProgressSource {
		[Header("Parameters")]
		public float GoalSerialized;
		[Header("Dependencies")]
		public ColliderNotifier2D Notifier;

		LevelManager _levelManager;

		float _curTotalProgress;
		bool  _needUpdateTotalProgress;

		readonly HashSet<GameplayBox> _activeBoxes = new HashSet<GameplayBox>();

		public float Goal        => GoalSerialized;
		public float CurProgress {
			get => _curTotalProgress;
			private set {
				_curTotalProgress = Mathf.CeilToInt(value);
				OnCurProgressChanged?.Invoke(_curTotalProgress);
			}
		}

		public event Action<float> OnCurProgressChanged;

		protected override void Init(GameStarter starter) {
			_levelManager = starter.LevelManager;
			_levelManager.RegisterProgressSource(this);

			Notifier.OnTriggerEnter += OnBoxEnter;
			Notifier.OnTriggerExit  += OnBoxExit;
		}

		void OnDestroy() {
			if ( Notifier ) {
				Notifier.OnTriggerEnter -= OnBoxEnter;
				Notifier.OnTriggerExit  -= OnBoxExit;
			}
		}

		void LateUpdate() {
			if ( _needUpdateTotalProgress || (_activeBoxes.Count > 0) ) {
				var totalProgress = 0f;
				foreach ( var activeBox in _activeBoxes ) {
					totalProgress += activeBox.CurHp;
				}
				totalProgress = Mathf.CeilToInt(totalProgress);
				if ( !Mathf.Approximately(totalProgress, CurProgress) ) {
					CurProgress = totalProgress;
					_levelManager.OnCurProgressChanged(this, CurProgress);
				}
				_needUpdateTotalProgress = false;
			}
		}

		void OnBoxEnter(GameObject boxGo) {
			var box = boxGo.GetComponentInChildren<GameplayBox>();
			if ( !box || _activeBoxes.Contains(box) ) {
				return;
			}
			_activeBoxes.Add(box);
			box.OnCurHpChanged += OnBoxHpChanged;
			box.OnDestroyed    += OnBoxDestroyed;

			_needUpdateTotalProgress = true;
		}

		void OnBoxExit(GameObject boxGo) {
			var box = boxGo.GetComponentInChildren<GameplayBox>();
			if ( !box || !_activeBoxes.Contains(box) ) {
				return;
			}
			_activeBoxes.Remove(box);
			box.OnCurHpChanged -= OnBoxHpChanged;
			box.OnDestroyed    -= OnBoxDestroyed;

			_needUpdateTotalProgress = true;
		}

		void OnBoxDestroyed(GameplayBox box) {
			box.OnCurHpChanged -= OnBoxHpChanged;
			box.OnDestroyed    -= OnBoxDestroyed;
		}

		void OnBoxHpChanged(float boxHp) {
			_needUpdateTotalProgress = true;
		}
	}
}
