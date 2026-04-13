//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(Exception exception);
        void LogCritical(Exception exception);
    }
}
