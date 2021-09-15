namespace SF.Gameplay {
	public interface IProgressSource {
		float Goal        { get; }
		float CurProgress { get; }
	}
}
