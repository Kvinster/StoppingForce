using SF.Gameplay;
using SF.State;

namespace SF.Tutorial {
	public abstract class BaseTutorial : GameComponent {
		public string Id;

		protected override void Init(GameStarter starter) {
			if ( GameState.Instance.IsTutorialShown(Id) ) {
				Hide();
				return;
			}
			InitInternal(starter);
		}

		protected abstract void InitInternal(GameStarter starter);

		protected void SetShown() {
			GameState.Instance.SetTutorialShown(Id);
		}

		protected virtual void Hide() {
			Destroy(gameObject);
		}
	}
}
