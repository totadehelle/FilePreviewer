using System;

namespace Logging.Format
{
    public interface IMessageFormatter
    {
        string Format(ILogMessage message);
        string Format(ILogMessage message, Exception ex);

    }
}