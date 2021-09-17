using UnityEngine;

namespace SF.LevelSelect {
	public sealed class ConnectionRenderer : MonoBehaviour {
		public Transform    StartPos;
		public Transform    EndPos;
		public LineRenderer LineRenderer;

		void Start() {
			LineRenderer.positionCount = 2;
			UpdatePositions();
		}

		void Update() {
			if ( !StartPos || !EndPos ) {
				LineRenderer.enabled = false;
				return;
			}
			LineRenderer.enabled = true;
			UpdatePositions();
		}

		void UpdatePositions() {
			LineRenderer.SetPosition(0, StartPos.position);
			LineRenderer.SetPosition(1, EndPos.position);
		}
	}
}
