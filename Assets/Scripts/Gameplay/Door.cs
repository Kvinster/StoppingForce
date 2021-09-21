using UnityEngine;

using DG.Tweening;

namespace SF.Gameplay {
	public sealed class Door : GameComponent {
		[Header("Parameters")]
		public bool CloseOnRelease;
		public float AnimDuration = 1f;
		[Header("Dependencies")]
		public GameplayButton Button;
		public Transform OpenPos;
		public Transform ClosePos;
		public Transform MoveTransform;

		bool _isOpen;

		Tween _anim;

		void OnDestroy() {
			_anim?.Kill();
		}

		protected override void Init(GameStarter starter) {
			MoveTransform.position = ClosePos.position;

			Button.OnPressed += OnButtonPressed;
			if ( CloseOnRelease ) {
				Button.OnReleased += OnButtonReleased;
			}
		}

		void OnButtonPressed() {
			TryOpen();
		}

		void OnButtonReleased() {
			TryClose();
		}

		void TryOpen() {
			if ( _isOpen ) {
				return;
			}
			_isOpen = true;
			_anim?.Kill();
			var openPos = OpenPos.position;
			_anim = MoveTransform.DOMove(openPos,
				AnimDuration * Vector2.Distance(MoveTransform.position, openPos) /
				Vector2.Distance(openPos, ClosePos.position)).SetEase(Ease.Linear);
		}

		void TryClose() {
			if ( !_isOpen ) {
				return;
			}
			_isOpen = false;
			_anim?.Kill();
			var closePos = ClosePos.position;
			_anim = MoveTransform.DOMove(closePos,
				AnimDuration * Vector2.Distance(MoveTransform.position, closePos) /
				Vector2.Distance(OpenPos.position, closePos)).SetEase(Ease.Linear);
		}
	}
}
