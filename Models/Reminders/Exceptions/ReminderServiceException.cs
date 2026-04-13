//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class ReminderServiceException : Xeption
    {
        public ReminderServiceException(Xeption innerException)
            : base(message: "Reminder service error occurred.", innerException)
        { }
    }
}
