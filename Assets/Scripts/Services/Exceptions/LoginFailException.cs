using System;

namespace SF.Services.Exceptions {
	public sealed class LoginFailException : Exception {
		public LoginFailException(string message) : base(message) { }
	}
}
