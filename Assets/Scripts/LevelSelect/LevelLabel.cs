using UnityEngine;

using TMPro;

namespace SF.LevelSelect {
	public sealed class LevelLabel : MonoBehaviour {
		[Header("Parameters")]
		public Color SpriteUnlockedColor = Color.white;
		public Color SpriteLockedColor = Color.white;
		public Color TextUnlockedColor = Color.white;
		public Color TextLockedColor   = Color.white;
		[Header("Dependencies")]
		public TMP_Text LevelNumberText;
		public SpriteRenderer SpriteRenderer;

		public void Init(int levelIndex, bool isLocked) {
			LevelNumberText.text  = (levelIndex + 1).ToString();
			LevelNumberText.color = isLocked ? TextLockedColor : TextUnlockedColor;
			SpriteRenderer.color  = isLocked ? SpriteLockedColor : SpriteUnlockedColor;
		}
	}
}
