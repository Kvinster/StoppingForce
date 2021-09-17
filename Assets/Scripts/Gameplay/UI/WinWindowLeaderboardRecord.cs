using UnityEngine;

using TMPro;

namespace SF.Gameplay.UI {
	public sealed class WinWindowLeaderboardRecord : MonoBehaviour {
		[Header("Parameters")]
		public Color ThisPlayerTextColor   = Color.white;
		public Color OtherPlayersTextColor = Color.white;
		[Header("Dependencies")]
		public TMP_Text IdText;
		public TMP_Text ScoreText;

		public void Init(int index, string id, string score, bool thisPlayer) {
			IdText.text    = $"{index}. {id}";
			ScoreText.text = score;

			IdText.color = ScoreText.color = thisPlayer ? ThisPlayerTextColor : OtherPlayersTextColor;
		}
	}
}
