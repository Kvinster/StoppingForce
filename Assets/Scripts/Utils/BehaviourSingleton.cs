using UnityEngine;

namespace SF.Utils {
	public abstract class BehaviourSingleton<T> : MonoBehaviour where T : BehaviourSingleton<T> {
		protected static T _instance;

		public static T Instance {
			get {
				if ( !_instance ) {
					var go = new GameObject($"[{typeof(T).Name}]");
					_instance = go.AddComponent<T>();
					_instance.TryInit();
				}
				return _instance;
			}
		}

		public static bool Exists => _instance;

		bool _isInit;

		void TryInit() {
			if ( _isInit ) {
				return;
			}
			Init();
			_isInit = true;
		}

		protected virtual void Init() { }
	}
}
