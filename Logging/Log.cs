using System;
using System.Threading;

namespace Logging
{
	public static class Log
    {
        static Log()
        {
            
        }

        internal static ILogger Logger => GlobalLoggerContext.Instance;

        /// <summary>
        /// Start logger configuration process.
        /// </summary>
        /// <returns></returns>
        public static LoggerConfiguration GetBuilder()
        {
            if (GlobalLoggerContext.IsConfigured)
                throw new LoggerConfiguringForbiddenException("Logger may be configured once only.");
            return new LoggerConfiguration();
        }

        /// <summary>
        /// Log message with Verbose level of severity
        /// </summary>
        public static void Verbose(string message, object data = null)
        {
            var logMessage = new Message(message, LogLevel.Verbose, data);
            Logger.Write(logMessage);
        }

        /// <summary>
        /// Log message with Debug level of severity
        /// </summary>
        public static void Debug(string message, object data = null)
        {
            var logMessage = new Message(message, LogLevel.Debug, data);
            Logger.Write(logMessage);
        }

        /// <summary>
        /// Log message with Information level of severity
        /// </summary>
        public static void Information(string message, object data = null)
        {
            var logMessage = new Message(message, LogLevel.Information, data);
            Logger.Write(logMessage);
        }

        /// <summary>
        /// Log message with Warning level of severity
        /// </summary>
        public static void Warning(string message, object data = null)
        {
            var logMessage = new Message(message, LogLevel.Warning, data);
            Logger.Write(logMessage);
        }

        /// <summary>
        /// Log message with Error level of severity
        /// </summary>
        public static void Error(string message, Exception exception, object data = null)
        {
            var logMessage = new Message(message, LogLevel.Error, data);
            Logger.Write(logMessage, exception);
        }

        /// <summary>
        /// Log message with Fatal level of severity
        /// </summary>
        public static void Fatal(string message, Exception exception, object data = null)
		{
            var logMessage = new Message(message, LogLevel.Fatal, data);
            Logger.Write(logMessage, exception);
        }

        /// <summary>
        /// Free all disposable resources and delete links to output modules from logger.
        /// </summary>
        public static void Close()
        {
            var oldLogger = Interlocked.Exchange(ref GlobalLoggerContext.Instance, new DefaultLogger());
            oldLogger.Flush();
        }
	}
}