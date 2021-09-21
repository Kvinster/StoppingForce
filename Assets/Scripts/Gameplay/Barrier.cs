using UnityEngine;

using System.Collections.Generic;

using DG.Tweening;

namespace SF.Gameplay {
	[RequireComponent(typeof(Collider2D))]
	[ExecuteInEditMode]
	public sealed class Barrier : MonoBehaviour {
		static readonly int Visibility = Shader.PropertyToID("_Visibility"); // float
		static readonly int ShowPoint  = Shader.PropertyToID("_ShowPoint"); // Vector3

		[Header("Parameters")]
		public float ShowTime = 0.5f;
		public float Radius = 5f;
		[Header("Dependencies")]
		public SpriteRenderer SpriteRenderer;

		readonly HashSet<GameplayBox> _activeBoxes = new HashSet<GameplayBox>();

		Tween _showAnim;

		MaterialPropertyBlock _materialPropertyBlock;

		MaterialPropertyBlock MaterialPropertyBlock {
			get {
				if ( _materialPropertyBlock == null ) {
					_materialPropertyBlock = new MaterialPropertyBlock();
				}
				return _materialPropertyBlock;
			}
		}

		void OnDestroy() {
			_showAnim?.Kill();
		}

		void Update() {
#if UNITY_EDITOR
			if ( !Application.isPlaying ) {
				var mpb = new MaterialPropertyBlock();
				SpriteRenderer.GetPropertyBlock(mpb);
				if ( !Mathf.Approximately(transform.localScale.x, Radius) ) {
					transform.localScale = new Vector3(Radius, Radius, 1f);
					UnityEditor.EditorUtility.SetDirty(this);
					mpb.SetFloat(Visibility, 1f);
				}
				if ( (Vector3)mpb.GetVector(ShowPoint) != transform.position ) {
					mpb.SetVector(ShowPoint, transform.position);
				}
				SpriteRenderer.SetPropertyBlock(mpb);
			}
#endif
		}

		void Start() {
			if ( !Application.isPlaying ) {
				var mpb = new MaterialPropertyBlock();
				SpriteRenderer.GetPropertyBlock(mpb);
				mpb.SetFloat(Visibility, 1f);
				mpb.SetVector(ShowPoint, transform.position);
				SpriteRenderer.SetPropertyBlock(mpb);
				return;
			}
			SpriteRenderer.GetPropertyBlock(MaterialPropertyBlock);
			MaterialPropertyBlock.SetFloat(Visibility, 0f);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
		}

		void OnCollisionEnter2D(Collision2D other) {
			var box = other.gameObject.GetComponent<GameplayBox>();
			if ( !box ) {
				return;
			}
			_activeBoxes.Add(box);
			SpriteRenderer.GetPropertyBlock(MaterialPropertyBlock);
			MaterialPropertyBlock.SetVector(ShowPoint, other.contacts[0].point);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
			_showAnim?.Kill(true);
			var visibility = 0f;
			_showAnim = DOTween.Sequence()
				.Append(DOTween.To(() => visibility, x => {
					visibility = x;
					MaterialPropertyBlock.SetFloat(Visibility, x);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 1f, ShowTime / 2f)).SetEase(Ease.InSine)
				.OnComplete(TryHide);
		}

		void OnCollisionStay(Collision other) {
			var box = other.gameObject.GetComponent<GameplayBox>();
			if ( !box ) {
				return;
			}
			SpriteRenderer.GetPropertyBlock(MaterialPropertyBlock);
			MaterialPropertyBlock.SetVector(ShowPoint, other.contacts[0].point);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
		}

		void OnCollisionExit2D(Collision2D other) {
			var box = other.gameObject.GetComponent<GameplayBox>();
			if ( !box ) {
				return;
			}
			_activeBoxes.Remove(box);
			TryHide();
		}

		void TryHide() {
			if ( (_activeBoxes.Count > 0) || ((_showAnim != null) && _showAnim.IsActive() && _showAnim.IsPlaying()) ) {
				return;
			}
			var visibility = 1f;
			_showAnim = DOTween.Sequence()
				.Append(DOTween.To(() => visibility, x => {
					visibility = x;
					MaterialPropertyBlock.SetFloat(Visibility, x);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 0f, ShowTime / 2f)).SetEase(Ease.OutSine);
		}
	}
}
