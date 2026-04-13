//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class ReminderDependencyException : Xeption
    {
        public ReminderDependencyException(Xeption innerException)
            : base(message: "Reminder dependency error occurred.", innerException)
        { }
    }
}
