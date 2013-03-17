using System;
using Caliburn.Micro;

namespace CashMoney.UI.Utils
{
    internal class Log4netLogger : ILog
    {
        private readonly log4net.ILog _logger;

        public Log4netLogger(Type type)
        {
            _logger = log4net.LogManager.GetLogger(type);
        }

        public void Error(Exception exception)
        {
            if (exception != null)
            {
                _logger.Error(exception.Message, exception);
            }
        }

        public void Info(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }
    }
}