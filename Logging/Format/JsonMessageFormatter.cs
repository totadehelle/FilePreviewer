using System;
using Newtonsoft.Json;

namespace Logging.Format
{
    public class JsonMessageFormatter : IMessageFormatter
    {
        public string Format(ILogMessage message)
        {
            object messageObject = new
            {
                level = message.LevelOfSeverity,
                timestamp = DateTime.UtcNow,
                message=message.Text,
                eventData= message.EventData
            }; 
            var formattedMessage = JsonConvert.SerializeObject(messageObject);
            return formattedMessage;
        }

        public string Format(ILogMessage message, Exception ex)
        {
            object messageObject = new
            {
                level = message.LevelOfSeverity,
                timestamp = DateTime.UtcNow,
                message = message.Text,
                eventData = message.EventData,
                exception = ex
            };
            var formattedMessage = JsonConvert.SerializeObject(messageObject);
            return formattedMessage;
        }
    }
}