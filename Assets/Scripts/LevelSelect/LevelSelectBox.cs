using System;

using SF.Common;

namespace SF.LevelSelect {
	public sealed class LevelSelectBox : BaseBox {
		bool _isIndestructible;

		public event Action OnDestroyed;

		public void Init(float startHp, bool isIndestructible) {
			_isIndestructible = isIndestructible;

			InitInternal(startHp);
		}

		public override void TakeDamage(float damage) {
			if ( _isIndestructible ) {
				return;
			}
			base.TakeDamage(damage);
		}

		public override void Die() {
			base.Die();
			OnDestroyed?.Invoke();
		}

		protected override void UpdateText() {
			if ( !_isIndestructible ) {
				base.UpdateText();
				return;
			}
			Text.text = "∞";
		}
	}
}
