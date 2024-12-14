using System;
namespace sws.BLL
{
	public class BusinessLogicException : Exception
	{
		public BusinessLogicException(string message, Exception? innerException = null)
			: base(message, innerException)
		{
		}
	}
}

