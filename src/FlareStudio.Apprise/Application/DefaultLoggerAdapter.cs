using System;

namespace FlareStudio.Apprise.Application
{
    internal class DefaultLoggerAdapter : ILoggerPort
    {
        public void LogException(string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception.ToString());
        }

        public void LogMessage(string message, params object[] values)
        {
            Console.WriteLine(string.Format(message, values));
        }
    }
}
