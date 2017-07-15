using System;
using System.Diagnostics;
using System.Transactions;

namespace Utilities
{
    public class Logger : ILogger
    {
        private readonly log4net.ILog logger;

        public Logger(string name)
        {
            this.logger = log4net.LogManager.GetLogger(name);
        }

        public Logger()
            : this(GetNameOfCallingClass())
        {
        }

        public bool IsDebugEnabled
        {
            get
            {
                return this.logger.IsDebugEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return this.logger.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return this.logger.IsWarnEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return this.logger.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return this.logger.IsFatalEnabled;
            }
        }

        public void LogDebug(string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Debug(msg + "\n");
            }
        }

        public void LogDebug(Exception e, string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Debug(msg + "\n", e);
            }
        }

        public void LogDebugFormat(string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogDebug(string.Format(msg, args));
            }
        }

        public void LogDebugFormat(Exception e, string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogDebug(e, string.Format(msg, args));
            }
        }
        public void LogInfo(string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Info(msg + "\n");
            }
        }

        public void LogInfo(Exception e, string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Info(msg + "\n", e);
            }
        }

        public void LogInfoFormat(string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogInfo(string.Format(msg, args));
            }
        }

        public void LogInfoFormat(Exception e, string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogInfo(e, string.Format(msg, args));
            }
        }
        public void LogWarn(string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Warn(msg + "\n");
            }
        }

        public void LogWarn(Exception e, string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Warn(msg + "\n", e);
            }
        }

        public void LogWarnFormat(string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogWarn(string.Format(msg, args));
            }
        }

        public void LogWarnFormat(Exception e, string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogWarn(e, string.Format(msg, args));
            }
        }
        public void LogError(string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Error(msg + "\n");
            }
        }

        public void LogError(Exception e, string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Error(msg + "\n", e);
            }
        }

        public void LogErrorFormat(string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogError(string.Format(msg, args));
            }
        }

        public void LogErrorFormat(Exception e, string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogError(e, string.Format(msg, args));
            }
        }

        public void LogFatal(string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Fatal(msg + "\n");
            }
        }

        public void LogFatal(Exception e, string msg)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.logger.Fatal(msg + "\n", e);
            }
        }

        public void LogFatalFormat(string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogFatal(string.Format(msg, args));
            }
        }

        public void LogFatalFormat(Exception e, string msg, params object[] args)
        {
            SetMethodProperty();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                this.LogFatal(e, string.Format(msg, args));
            }
        }

        public void Preamble(string context)
        {
            string CalledFrom = GetNameOfCallingMethod();

            string preamble = string.Format("{0} Invoked. ContextID : {1} ", CalledFrom, context);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                logger.Info(preamble + "\n");
            }
        }

        private static void SetMethodProperty()
        {
            log4net.LogicalThreadContext.Properties["methodname"] = GetNameOfCallingMethod();
        }

        private static string GetNameOfCallingMethod()
        {
            StackTrace st = new StackTrace(true);
            string methodName = "N/A";

            for (int i = 0; i < st.FrameCount; i++)
            {
                Type t = st.GetFrame(i).GetMethod().DeclaringType;
                if (t != typeof(Logger) && t != typeof(SLogger))
                {
                    methodName = st.GetFrame(i).GetMethod().Name;
                    break;
                }
            }

            return methodName;
        }

        private static string GetNameOfCallingClass()
        {
            StackTrace st = new StackTrace(true);
            string className = "N/A";

            for (int i = 0; i < st.FrameCount; i++)
            {
                Type t = st.GetFrame(i).GetMethod().DeclaringType;
                if (t != typeof(Logger) && t != typeof(SLogger))
                {
                    StackFrame frame = st.GetFrame(i);
                    System.Reflection.MethodBase mb = frame.GetMethod();
                    Type declaringType = mb.DeclaringType;

                    if (declaringType != null)
                    {
                        className = declaringType.Name;
                        break;
                    }
                }
            }

            return className;
        }
    }
}