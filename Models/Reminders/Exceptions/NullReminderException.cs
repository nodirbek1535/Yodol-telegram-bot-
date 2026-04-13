//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class NullReminderException : Xeption
    {
        public NullReminderException()
            : base(message: "Reminder is null.")
        { }
    }
}
