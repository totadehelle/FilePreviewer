namespace Logging
{
    internal static class GlobalLoggerContext
    {
        static GlobalLoggerContext()
        {
            
        }
        
        internal static ILogger Instance = new DefaultLogger();
        internal static bool IsConfigured = false;
    }
}