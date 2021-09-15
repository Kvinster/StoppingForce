using UnityEngine;
using UnityEngine.Assertions;

namespace SF.Configs {
	[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Custom/LevelsConfig")]
	public sealed class LevelsConfig : ScriptableObject {
		const string ConfigPath = "Configs/LevelsConfig";

		static LevelsConfig _instance;

		public static LevelsConfig Instance {
			get {
				if ( !_instance ) {
					_instance = Resources.Load<LevelsConfig>(ConfigPath);
					Assert.IsTrue(_instance);
				}
				return _instance;
			}
		}

		public int TotalLevels;
	}
}
