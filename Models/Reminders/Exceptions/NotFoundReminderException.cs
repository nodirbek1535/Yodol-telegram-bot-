//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class NotFoundReminderException : Xeption
    {
        public NotFoundReminderException(Guid reminderId)
            : base(message: $"Reminder not found with id: {reminderId}.")
        { }
    }
}
