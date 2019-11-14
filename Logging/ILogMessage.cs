namespace Logging
{
    public interface ILogMessage
    {
        string Text { get; set; }
        LogLevel LevelOfSeverity { get; set; }
        object EventData { get; set; }
    }
}