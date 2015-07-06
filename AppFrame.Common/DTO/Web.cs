using System;

namespace AppFrame.Common.DTO
{
	public class WebPageStatus
	{
		public const string MessageTypeError = "Error";
		public const string MessageTypeInfo = "Info";
		public const string MessageTypeSuccess = "Success";
		public const string MessageTypeWarning = "Warning";

		public string RedirectUrl { get; set; }
		public string Message { get; set; }
		public string MessageType { get;set;}
	}
}

