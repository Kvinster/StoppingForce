using UnityEngine;

namespace SF.Utils {
	public sealed class CursorChanger : MonoBehaviour {
		static bool _cursorSet;

		public Texture2D CursorTexture;
		public Vector2   Hotspot;

		void Start() {
			if ( !_cursorSet ) {
				Cursor.SetCursor(CursorTexture, Hotspot, CursorMode.Auto);
				_cursorSet = true;
			}
			Destroy(gameObject);
		}
	}
}
