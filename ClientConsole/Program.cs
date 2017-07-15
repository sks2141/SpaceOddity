using System;
using Utilities;

namespace ClientConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            BuildAndResolveDependecies();

            Console.ReadLine();
        }

        private static void BuildAndResolveDependecies()
        {
            DependencyBuilder builder = new DependencyBuilder();
            builder.LoadDependencyGraph();
            builder.Build();
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception) e.ExceptionObject;
            SLogger.LogFatalFormat(ex, "Unhandled exception while running ClientConsole. Details:{0}; IsTerminating:{1}", 
                ex.Message, e.IsTerminating);
        }
    }
}