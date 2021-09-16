namespace SF.Gameplay {
	public interface IBoxesSource {
		int   TotalBoxes { get; }
		int   BoxesLeft  { get; }
		float TotalBoxHp { get; }
	}
}
