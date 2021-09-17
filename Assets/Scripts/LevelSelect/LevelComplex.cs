using UnityEngine;

namespace SF.LevelSelect {
	public sealed class LevelComplex : MonoBehaviour {
		public Transform      ConnectionPos;
		public LevelSelectBox Box;
		public SpringJoint2D  BoxJoint;
		public LevelLabel     LevelLabel;
	}
}
