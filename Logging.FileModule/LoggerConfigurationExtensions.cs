namespace Logging.FileOutput
{
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Add thread-safe writing log events to file/>.
        /// </summary>
        public static LoggerConfiguration WriteToFile(this LoggerConfiguration configuration, string path)
        {
            configuration.AddOutputModule(new FileModule(path));
            return configuration;
        }
    }
}