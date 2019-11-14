using System;
using System.Threading;
using Logging.Format;

namespace Logging
{
	public class LoggerConfiguration
	{
		private readonly Logger _logger;

		internal LoggerConfiguration()
		{
            _logger = new Logger();
		}

        /// <summary>
        /// Set minimum severity level of events to be logged. A message will be logged if it
        /// has the same or higher level of severity. Otherwise it will be ignored.
        /// Default level is LogLevel.Information.
        /// </summary>
        public LoggerConfiguration SetMinimumLevel(LogLevel level)
		{
			_logger.MinimumLevel = level;
			return this;
        }

        /// <summary>
        /// Set message formatter with specific message template. Default template looks like:
        /// [INF][UTC 12/1/2000 00:00:00][Message: Your log message text][Data: {"clientId":3947}].
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public LoggerConfiguration SetFormatter(IMessageFormatter formatter)
        {
            _logger.Formatter = formatter;
            return this;
        }

        /// <summary>
        /// Add new output module for logger.
        /// </summary>
        public LoggerConfiguration AddOutputModule(IOutputModule module)
        {
            _logger.OutputModules.Add(module);
            return this;
        }

		/// <summary>
        /// Finish logger configuration process and save all the settings.
        /// </summary>
        public void BuildLogger()
		{
            if (GlobalLoggerContext.IsConfigured)
                throw new LoggerConfiguringForbiddenException("Logger may be configured once only.");
            GlobalLoggerContext.IsConfigured = true;
            Volatile.Write(ref GlobalLoggerContext.Instance, _logger);
        }
	}
}