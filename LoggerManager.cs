using CompanyEmployees;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;

namespace CompanyEmployees
{
 
    public class LoggerManager : ILoggerManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public LoggerManager()
        {
            
        }
        public void LogDebug(string message)
        {

            logger.Debug(message);
            
            
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void LogWarn(string message)
        {
            logger.Warn(message);
         
        }
    }
}
