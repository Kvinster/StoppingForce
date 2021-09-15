using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using SF.Controllers;
using SF.Managers;
using SF.Services;

using TMPro;

namespace SF.Gameplay.UI {
	public sealed class WinWindow : MonoBehaviour {
		const string DescTextTemplate = "Boxes used: {0}\nTotal score: {1}";

		public TMP_Text   DescText;
		public Button     ExitButton;
		public GameObject GameCompleteRoot;
		public GameObject NextLevelButtonRoot;
		public Button     NextLevelButton;

		LevelManager _levelManager;

		public void Show(LevelManager levelManager) {
			_levelManager = levelManager;

			DescText.text = string.Format(DescTextTemplate, levelManager.TotalBoxesUsed,
				Mathf.CeilToInt(levelManager.TotalProgress));

			ExitButton.onClick.AddListener(OnExitClick);
			if ( LevelController.Instance.HasLevel(levelManager.LevelIndex + 1) ) {
				GameCompleteRoot.SetActive(false);
				NextLevelButtonRoot.SetActive(true);
				NextLevelButton.onClick.AddListener(OnNextLevelClick);
			} else {
				GameCompleteRoot.SetActive(true);
				NextLevelButtonRoot.SetActive(false);
			}

			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void OnExitClick() {
			SceneService.LoadMainMenu();
		}

		void OnNextLevelClick() {
			var nextLevelIndex = _levelManager.LevelIndex + 1;
			Assert.IsTrue(LevelController.Instance.HasLevel(nextLevelIndex));
			LevelController.Instance.StartLevel(nextLevelIndex);
			SceneService.LoadLevel(nextLevelIndex);
		}
	}
}
