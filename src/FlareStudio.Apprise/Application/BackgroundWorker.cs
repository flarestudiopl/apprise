using System;
using System.Timers;

namespace FlareStudio.Apprise.Application
{
    internal interface IBackgroundWorker : IDisposable
    {
        void Start();
        void Stop();
    }

    internal class BackgroundWorker : IBackgroundWorker
    {
        private readonly Timer _timer = new Timer(5000) { AutoReset = false };
        private readonly ILoggerPort _loggerPort;
        private bool _stopping = false;

        public BackgroundWorker(IMessageQueueProcessor messageQueueProcessor, ILoggerPort loggerPort)
        {
            _loggerPort = loggerPort;

            _timer.Elapsed += (_, __) =>
            {
                try
                {
                    messageQueueProcessor.Process();
                }
                catch (Exception exception)
                {
                    _loggerPort.LogException("MessageQueueProcessor failed.", exception);
                }

                if (!_stopping)
                {
                    _timer.Start();
                }
            };
        }

        public void Start()
        {
            _loggerPort.LogMessage("Starting notifications background worker...");
            _timer.Start();
        }

        public void Stop()
        {
            _loggerPort.LogMessage("Stopping notifications background worker...");
            _timer.Stop();
            _stopping = true;
        }

        public void Dispose()
        {
            Stop();
            _timer.Dispose();
        }
    }
}
