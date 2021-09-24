using UnityEngine;

using System;

using TMPro;

namespace SF.Common {
	public abstract class BaseBox : MonoBehaviour {
		[Header("Parameters")]
		public Color OkColor = Color.white;
		public Color DeadColor = Color.red;
		[Header("Dependencies")]
		public SpriteRenderer SpriteRenderer;
		public TMP_Text    Text;
		public Rigidbody2D Rigidbody;

		float _startHp;
		float _curHp;

		public float CurHp {
			get => _curHp;
			private set {
				_curHp = value;
				OnCurHpChanged?.Invoke(_curHp);
			}
		}

		public event Action<float>   OnCurHpChanged;
		public event Action<BaseBox> OnDestroyed;

		protected void InitInternal(float startHp) {
			_startHp = startHp;
			CurHp    = _startHp;
			UpdateView();
		}

		public virtual void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0f);
			if ( Mathf.Approximately(CurHp, 0f) ) {
				Die();
			} else {
				UpdateView();
			}
		}

		public virtual void Die() {
			OnDestroyed?.Invoke(this);
			Destroy(gameObject);
		}

		void UpdateView() {
			UpdateColor();
			UpdateText();
		}

		void UpdateColor() {
			SpriteRenderer.color = Color.Lerp(OkColor, DeadColor, (_startHp - CurHp) / _startHp);
		}

		protected virtual void UpdateText() {
			Text.text = Mathf.CeilToInt(CurHp).ToString();
		}
	}
}
