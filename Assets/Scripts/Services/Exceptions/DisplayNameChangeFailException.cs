using System;

namespace SF.Services.Exceptions {
	public sealed class DisplayNameChangeFailException : Exception {
		public DisplayNameChangeFailException(string message) : base(message) { }
	}
}
