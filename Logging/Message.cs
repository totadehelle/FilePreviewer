namespace Logging
{
    public class Message : ILogMessage
    {
        public string Text { get; set; }
        public LogLevel LevelOfSeverity { get; set; }
        public object EventData { get; set; }

        public Message(string text, LogLevel level, object data)
        {
            Text = text;
            LevelOfSeverity = level;
            EventData = data;
        }
    }
}