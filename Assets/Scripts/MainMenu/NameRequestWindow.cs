using UnityEngine;
using UnityEngine.UI;

using SF.Services;

using TMPro;

namespace SF.MainMenu {
	public sealed class NameRequestWindow : MonoBehaviour {
		public TMP_InputField InputField;
		public Button         CancelButton;
		public Button         AcceptButton;
		[Space]
		public LoadingWindow LoadingWindow;

		void Start() {
			AcceptButton.onClick.AddListener(OnAcceptClick);
			CancelButton.onClick.AddListener(OnCancelClick);
		}

		public void Show() {
			InputField.text = PlayFabService.DisplayName;

			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}

		void OnAcceptClick() {
			if ( string.IsNullOrEmpty(InputField.text) ) {
				return;
			}
			LoadingWindow.Show();
			PlayFabService.TryChangeDisplayName(InputField.text)
				.Then(() => {
					LoadingWindow.Hide();
					Hide();
				})
				.Catch(exception => {
					Debug.LogException(exception);
					LoadingWindow.Hide();
					Hide();
				});
		}

		void OnCancelClick() {
			Hide();
		}
	}
}
