//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class ReminderValidationException : Xeption
    {
        public ReminderValidationException(Xeption innerException)
            : base(message: "Reminder validation error occurred.", innerException)
        { }
    }
}
