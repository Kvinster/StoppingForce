using System;

namespace SF.Services.Exceptions {
	public sealed class SendScoreException : Exception {
		public SendScoreException(string message) : base(message) { }
	}
}
