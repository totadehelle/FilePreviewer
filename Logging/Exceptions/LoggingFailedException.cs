using System;

namespace Logging
{
    public class LoggingFailedException : Exception
    {
        public LoggingFailedException(string message)
            : base(message)
        {
        }
    }
}