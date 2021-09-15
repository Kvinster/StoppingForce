using UnityEngine;

using System;
using System.Collections.Generic;

using TMPro;

namespace SF.Gameplay {
	public sealed class Box : MonoBehaviour {
		const float MaxStationaryBoxSpeed = 0.25f;

		public static readonly HashSet<Box> Instances = new HashSet<Box>();

		[Header("Parameters")]
		public Color OkColor   = Color.white;
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

		public bool IsStationary { get; private set; }

		public event Action<float> OnCurHpChanged;
		public event Action<Box>   OnDestroyed;

		void OnEnable() {
			Instances.Add(this);
		}

		void OnDisable() {
			Instances.Remove(this);
		}

		void OnDestroy() {
			OnDestroyed?.Invoke(this);
		}

		void FixedUpdate() {
			IsStationary = Rigidbody.velocity.magnitude <= MaxStationaryBoxSpeed;
		}

		public void Init(float startHp) {
			_startHp = startHp;
			CurHp    = _startHp;
			UpdateView();
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0f);
			if ( Mathf.Approximately(CurHp, 0f) ) {
				Die();
			} else {
				UpdateView();
			}
		}

		public void Die() {
			Destroy(gameObject);
		}

		void UpdateView() {
			UpdateColor();
			UpdateText();
		}

		void UpdateColor() {
			SpriteRenderer.color = Color.Lerp(OkColor, DeadColor, (_startHp - CurHp) / _startHp);
		}

		void UpdateText() {
			Text.text = Mathf.CeilToInt(CurHp).ToString();
		}
	}
}
