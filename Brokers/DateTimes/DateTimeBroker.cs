//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Brokers.DateTimes
{
    public class DateTimeBroker : IDateTimeBroker
    {
        public DateTime GetCurrentDateTime() => DateTime.Now;
    }
}
