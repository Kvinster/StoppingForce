using UnityEngine;
using UnityEngine.UI;

using SF.Services;
using SF.Services.Exceptions;

using PlayFab;

using TMPro;

namespace SF.MainMenu {
	public sealed class NameRequestWindow : MonoBehaviour {
		const string NameNotAvailableErrorText = "Name is already taken";

		public TMP_InputField InputField;
		public GameObject     ErrorTextRoot;
		public TMP_Text       ErrorText;
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

			InputField.onValueChanged.AddListener(OnInputFieldValueChanged);

			ErrorTextRoot.SetActive(false);

			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);

			InputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
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
				.Catch(e => {
					LoadingWindow.Hide();
					if ( e is DisplayNameChangeFailException exception ) {
						if ( exception.ErrorCode == PlayFabErrorCode.NameNotAvailable ) {
							ErrorTextRoot.SetActive(true);
							ErrorText.text = NameNotAvailableErrorText;
						} else {
							Debug.LogException(e);
							Hide();
						}
					} else {
						Debug.LogException(e);
						Hide();
					}
				});
		}

		void OnCancelClick() {
			Hide();
		}

		void OnInputFieldValueChanged(string _) {
			ErrorTextRoot.SetActive(false);
		}
	}
}
