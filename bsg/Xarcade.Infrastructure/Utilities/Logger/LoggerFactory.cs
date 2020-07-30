using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Xarcade.Infrastructure.Utilities.Logger
{
    public enum Logger
    {
        Dummy
    }

    public static class LoggerFactory
    {
        public static IConfiguration Configuration {private get; set;}

        private static Dictionary<Logger, ILogger> _loggers = new Dictionary<Logger, ILogger>()
        {
            {Logger.Dummy, new DummyLogger()},
        };

        public static ILogger GetLogger (Logger logger)
        {
            _loggers[logger].Configuration = Configuration;
            return _loggers[logger];
        }
    }
}