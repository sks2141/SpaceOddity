using System;
using System.Diagnostics;

namespace Utilities
{
    /// <summary>
    /// Static Logger
    /// </summary>
    public class SLogger
    {
        private static readonly object initLock = new object();
        private static bool initialized = false;

        private static ILogger logger;

        public static void ConfigureXML()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SetGlobalProperty(string key, string value)
        {
            log4net.GlobalContext.Properties[key] = value;
        }

        public static void SetExternalLogger(ILogger injectedLogger)
        {
            logger = injectedLogger;
        }

        public static bool IsDebugEnabled
        {
            get
            {
                return GetLogger().IsDebugEnabled;
            }
        }

        public static bool IsInfoEnabled
        {
            get
            {
                return GetLogger().IsInfoEnabled;
            }
        }

        public static bool IsWarnEnabled
        {
            get
            {
                return GetLogger().IsWarnEnabled;
            }
        }

        public static bool IsErrorEnabled
        {
            get
            {
                return GetLogger().IsErrorEnabled;
            }
        }

        public static bool IsFatalEnabled
        {
            get
            {
                return GetLogger().IsFatalEnabled;
            }
        }

        public static void LogDebug(string msg)
        {
            GetLogger().LogDebug(msg);
        }

        public static void LogDebug(Exception e, string msg)
        {
            GetLogger().LogDebug(e, msg);
        }

        public static void LogDebugFormat(string msg, params object[] args)
        {
            GetLogger().LogDebugFormat(msg, args);
        }

        public static void LogDebugFormat(Exception e, string msg, params object[] args)
        {
            GetLogger().LogDebugFormat(e, msg, args);
        }

        public static void LogInfo(string msg)
        {
            GetLogger().LogInfo(msg);
        }

        public static void LogInfo(Exception e, string msg)
        {
            GetLogger().LogInfo(e, msg);
        }

        public static void LogInfoFormat(string msg, params object[] args)
        {
            GetLogger().LogInfoFormat(msg, args);
        }

        public static void LogInfoFormat(Exception e, string msg, params object[] args)
        {
            GetLogger().LogInfoFormat(e, msg, args);
        }

        public static void LogWarn(string msg)
        {
            GetLogger().LogWarn(msg);
        }

        public static void LogWarn(Exception e, string msg)
        {
            GetLogger().LogWarn(e, msg);
        }

        public static void LogWarnFormat(string msg, params object[] args)
        {
            GetLogger().LogWarnFormat(msg, args);
        }

        public static void LogWarnFormat(Exception e, string msg, params object[] args)
        {
            GetLogger().LogWarnFormat(e, msg, args);
        }

        public static void LogWarnFormat(string msg, Exception e)
        {
            GetLogger().LogWarn(e, msg);
        }

        public static void LogError(string msg)
        {
            GetLogger().LogError(msg);
        }

        public static void LogError(Exception e, string msg)
        {
            GetLogger().LogError(e, msg);
        }

        public static void LogError(Exception e, string msg, string config, Process process)
        {
            GetLogger().LogError(e, msg);
        }

        public static void LogErrorFormat(string msg, params object[] args)
        {
            GetLogger().LogErrorFormat(msg, args);
        }

        public static void LogErrorFormat(Exception e, string msg, params object[] args)
        {
            GetLogger().LogErrorFormat(e, msg, args);
        }

        public static void LogErrorFormat(Exception e, string msg, string config, Process process, params object[] args)
        {
            GetLogger().LogErrorFormat(e, msg, args);
        }

        public static void LogErrorFormat(string msg, Exception e)
        {
            GetLogger().LogError(e, msg);
        }

        public static void LogFatal(string msg)
        {
            GetLogger().LogFatal(msg);
        }

        public static void LogFatal(Exception e, string msg)
        {
            GetLogger().LogFatal(e, msg);
        }

        public static void LogFatal(Exception e, string msg, string config, Process process)
        {
            GetLogger().LogFatal(e, msg);
        }

        public static void LogFatalFormat(string msg, params object[] args)
        {
            GetLogger().LogFatalFormat(msg, args);
        }

        public static void LogFatalFormat(Exception e, string msg, params object[] args)
        {
            GetLogger().LogFatalFormat(e, msg, args);
        }

        public static void LogFatalFormat(Exception e, string msg, string config, Process process, params object[] args)
        {
            GetLogger().LogFatalFormat(e, msg, args);
        }

        public static void LogFatalFormat(string msg, Exception e)
        {
            GetLogger().LogFatal(e, msg);
        }

        private static ILogger GetLogger()
        {
            lock (initLock)
            {
                if (!initialized)
                {
                    ConfigureXML();
                    initialized = true;
                }
            }

            return logger ?? new Logger();
        }
    }
}