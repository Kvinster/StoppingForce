using UnityEngine;

namespace SF.MainMenu {
	public sealed class LoadingWindow : MonoBehaviour {
		[Header("Parameters")]
		public float AnimSpeed = 90f;
		[Header("Dependencies")]
		public Transform IconTransform;

		void Update() {
			IconTransform.rotation =
				Quaternion.Euler(0f, 0f, IconTransform.rotation.eulerAngles.z + AnimSpeed * Time.deltaTime);
		}

		public void Show() {
			gameObject.SetActive(true);
			IconTransform.rotation = Quaternion.identity;
		}

		public void Hide() {
			gameObject.SetActive(false);
		}
	}
}
