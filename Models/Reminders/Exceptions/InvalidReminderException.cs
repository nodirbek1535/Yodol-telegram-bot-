//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class InvalidReminderException : Xeption
    {
        public InvalidReminderException()
            : base(message: "Reminder is invalid.")
        { }
    }
}
