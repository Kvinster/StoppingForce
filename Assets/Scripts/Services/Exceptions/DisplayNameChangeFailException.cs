using System;

using PlayFab;

namespace SF.Services.Exceptions {
	public sealed class DisplayNameChangeFailException : Exception {
		public readonly PlayFabErrorCode ErrorCode;

		public DisplayNameChangeFailException(PlayFabErrorCode errorCode, string message) : base(message) {
			ErrorCode = errorCode;
		}
	}
}
