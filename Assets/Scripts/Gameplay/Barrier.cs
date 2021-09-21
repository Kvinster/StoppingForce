using UnityEngine;

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
				if ( !Mathf.Approximately(transform.localScale.x, Radius) ) {
					transform.localScale = new Vector3(Radius, Radius, 1f);
					UnityEditor.EditorUtility.SetDirty(this);
					var mpb = new MaterialPropertyBlock();
					SpriteRenderer.GetPropertyBlock(mpb);
					mpb.SetFloat(Visibility, 1f);
					mpb.SetVector(ShowPoint, transform.position);
					SpriteRenderer.SetPropertyBlock(mpb);
				}
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
				.Append(DOTween.To(() => visibility, x => {
					visibility = x;
					MaterialPropertyBlock.SetFloat(Visibility, x);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 0f, ShowTime / 2f)).SetEase(Ease.OutSine);
		}
	}
}
