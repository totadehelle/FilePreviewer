using System;

namespace Logging
{
    public class LoggerConfiguringForbiddenException : Exception
    {
        public LoggerConfiguringForbiddenException(string message)
            : base(message)
        {
            
        }
    }
}