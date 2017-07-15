using System;

namespace Utilities
{
    public interface ILogger
    {
        void LogDebug(string msg);

        void LogDebug(Exception e, string msg);

        void LogDebugFormat(string msg, params object[] args);

        void LogDebugFormat(Exception e, string msg, params object[] args);

        void LogInfo(string msg);

        void LogInfo(Exception e, string msg);

        void LogInfoFormat(string msg, params object[] args);

        void LogInfoFormat(Exception e, string msg, params object[] args);

        void LogWarn(string msg);

        void LogWarn(Exception e, string msg);

        void LogWarnFormat(string msg, params object[] args);

        void LogWarnFormat(Exception e, string msg, params object[] args);

        void LogError(string msg);

        void LogError(Exception e, string msg);

        void LogErrorFormat(string msg, params object[] args);

        void LogErrorFormat(Exception e, string msg, params object[] args);

        void LogFatal(string msg);

        void LogFatal(Exception e, string msg);

        void LogFatalFormat(string msg, params object[] args);

        void LogFatalFormat(Exception e, string msg, params object[] args);

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }
    }
}