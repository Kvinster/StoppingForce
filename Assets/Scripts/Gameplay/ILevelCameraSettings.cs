using UnityEngine;

namespace SF.Gameplay {
	public interface ILevelCameraSettings {
		float   StartSize      { get; }
		float   MinSize        { get; }
		float   MaxSize        { get; }
		Vector2 HorizontalZone { get; }
		Vector2 VerticalZone   { get; }
		Vector2 CentralPos     { get; }
		Vector2 StartPos       { get; }
		bool    PlayAnim       { get; }
	}
}
