using System;

namespace Logging
{
	public interface ILogger
	{
		void Write(ILogMessage message);
        void Write(ILogMessage message, Exception ex);
        void Flush();
    }
}