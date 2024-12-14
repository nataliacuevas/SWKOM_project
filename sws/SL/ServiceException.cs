using System;
namespace sws.SL
{
	public class ServiceException : Exception
	{
		public ServiceException(string message, Exception? innerException = null)
			: base(message, innerException)
		{
		}
	}
}

