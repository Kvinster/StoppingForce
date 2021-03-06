using UnityEngine;

using DG.Tweening;
using TMPro;

namespace SF.Gameplay.UI {
	public sealed class GoalAreaText : MonoBehaviour {
		const string TextTemplate = "{0}/{1}";

		[Header("Parameters")]
		public float GoalAnimSpeed = 3f; // time to anim full goal
		[Header("Dependencies")]
		public TMP_Text Text;
		public GoalArea GoalArea;

		float _curViewProgress;

		Tween _goalAnim;

		void Start() {
			GoalArea.OnCurProgressChanged += OnCurProgressChanged;
			OnCurProgressChanged(GoalArea.CurProgress);
		}

		void OnCurProgressChanged(float curProgress) {
			_goalAnim?.Kill(false);
			_goalAnim = DOTween.To(() => _curViewProgress,
				x => {
					_curViewProgress = x;
					Text.text = string.Format(TextTemplate,
						Mathf.CeilToInt(_curViewProgress), GoalArea.Goal);
				}, curProgress,
				Mathf.Min(Mathf.Abs(curProgress - _curViewProgress), GoalArea.Goal) *
				GoalAnimSpeed / GoalArea.Goal).SetEase(Ease.Linear);
		}
	}
}
