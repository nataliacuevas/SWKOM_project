using System;
namespace sws.DAL
{
	public class DataAccessException: Exception
	{
		public DataAccessException(string message, Exception? innerException = null)
			: base(message, innerException)
		{
		}
	}
}

