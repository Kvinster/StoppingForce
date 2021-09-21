using SF.Gameplay;
using SF.LevelSelect;
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

		public void Init(LevelSelectStarter starter) {
			if ( GameState.Instance.IsTutorialShown(Id) ) {
				Hide();
				return;
			}
			InitInternal(starter);
		}

		protected abstract void InitInternal(GameStarter starter);

		protected virtual void InitInternal(LevelSelectStarter starter) { }

		protected void SetShown() {
			GameState.Instance.SetTutorialShown(Id);
		}

		protected virtual void Hide() {
			Destroy(gameObject);
		}
	}
}
