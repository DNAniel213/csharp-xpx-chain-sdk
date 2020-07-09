using System;
using System.Runtime.CompilerServices;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace Xarcade.Infrastructure.Utilities.Logger
{
    public class BlockchainLogger: ILogger
    {
        public IConfiguration Configuration
        {
            get {return configuration;}
            set
            {
                configuration = value;
                this.SetLoggerConfiguration(configuration);
            }

        }

        private IConfiguration configuration;
        private Serilog.ILogger logger;

        public void LogError(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int lineNumber = 0)
        {
            this.logger.Error($"[{caller}][{lineNumber}] {message}");
        }

        public void LogInfo(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int lineNumber = 0)
        {
            this.logger.Information($"[{caller}][{lineNumber}] {message}");
        }

        public void LogWarning(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int lineNumber = 0)
        {
            this.logger.Warning($"[{caller}][{lineNumber}] {message}");
        }

        private void SetLoggerConfiguration(IConfiguration configuration)
        {
            if (this.logger != null)
            {
                return;
            }

            var logPath = configuration["BlockchainPortalLogFile"];
            var logFileCount = Int32.Parse(configuration["BlockchainPortalLogFileCount"]);
            var fileSizeLimit = Int32.Parse(configuration["BlockchainPortalFileSizeLimt"]);

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel
                            .Information()
                            .WriteTo.File(logPath,
                                          rollingInterval: RollingInterval.Day,
                                          rollOnFileSizeLimit: true,
                                          fileSizeLimitBytes: fileSizeLimit,
                                          retainedFileCountLimit: logFileCount)
                            .CreateLogger();
                            
            this.logger = Log.Logger;
        }
        
    }
}