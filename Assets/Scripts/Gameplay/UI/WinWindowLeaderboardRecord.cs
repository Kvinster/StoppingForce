using UnityEngine;

using TMPro;

namespace SF.Gameplay.UI {
	public sealed class WinWindowLeaderboardRecord : MonoBehaviour {
		public TMP_Text IdText;
		public TMP_Text ScoreText;

		public void Init(int index, string id, string score) {
			IdText.text    = $"{index}. {id}";
			ScoreText.text = score;
		}
	}
}
