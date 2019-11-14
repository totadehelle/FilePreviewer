using System;
using System.Collections.Generic;
using Logging.Format;

namespace Logging
{
    public class Logger : ILogger
	{
		internal LogLevel MinimumLevel = LogLevel.Information;
		internal List<IOutputModule> OutputModules = new List<IOutputModule>();
        internal IMessageFormatter Formatter = new DefaultMessageFormatter();

        internal Logger()
        {
            
        }
        
        public void Write(ILogMessage message)
		{
			if(message.LevelOfSeverity < MinimumLevel)
				return;
            var formattedMessage = Formatter.Format(message);
            foreach (var module in OutputModules)
			{
				module.Write(formattedMessage);
			}
		}

        public void Write(ILogMessage message, Exception ex)
        {
            if (message.LevelOfSeverity < MinimumLevel)
                return;
            var formattedMessage = Formatter.Format(message, ex);
            foreach (var module in OutputModules)
            {
                module.Write(formattedMessage);
            }
        }

        public void Flush()
        {
            foreach (var module in OutputModules)
            {
                if (module is IDisposable disposableModule)
                {
                    disposableModule.Dispose();
                }
            }
            OutputModules.Clear();
        }
    }
}
