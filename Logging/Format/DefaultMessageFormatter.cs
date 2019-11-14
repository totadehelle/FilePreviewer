using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Logging.Format
{
    public class DefaultMessageFormatter : IMessageFormatter
    {
        private readonly Dictionary<LogLevel, string> _shortLevels = new Dictionary<LogLevel, string>
        {
            {LogLevel.Verbose, "VRB" },
            {LogLevel.Debug, "DBG" },
            {LogLevel.Information, "INF" },
            {LogLevel.Warning, "WRN" },
            {LogLevel.Error, "ERR" },
            {LogLevel.Fatal, "FAT" },
        };

        public string Format(ILogMessage message, Exception ex)
        {
            var timestamp = DateTime.UtcNow;
            var shortLevel = _shortLevels[message.LevelOfSeverity];
            var formattedMessage = $"[{shortLevel}]"
                                   + $"[UTC {timestamp}]"
                                   + $"[Message: {message.Text}]";
            if (message.EventData != null)
            {
                var jsonData = JsonConvert.SerializeObject(message.EventData);
                formattedMessage += $"[Data: {jsonData}]";
            }
            formattedMessage += ex;
            return formattedMessage;
        }

        public string Format(ILogMessage message)
        {
            var timestamp = DateTime.UtcNow;
            var shortLevel = _shortLevels[message.LevelOfSeverity];
            var formattedMessage = $"[{shortLevel}]"
                                   + $"[UTC {timestamp}]"
                                   + $"[Message: {message.Text}]";
            if (message.EventData != null)
            {
                var jsonData = JsonConvert.SerializeObject(message.EventData);
                formattedMessage += $"[Data: {jsonData}]";
            }
            return formattedMessage;
        }
    }
}