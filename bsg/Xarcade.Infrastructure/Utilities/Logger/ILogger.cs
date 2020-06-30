using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace Xarcade.Infrastructure.Utilities.Logger
{
    public interface ILogger
    {
        IConfiguration Configuration {get; set;}

        void LogError(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int lineNumber = 0);
        void LogInfo(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int lineNumber = 0);
        void LogWarning(string message, [CallerMemberName] string caller = "", [CallerLineNumber] int lineNumber = 0);

    }
}

