//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class FailedReminderStorageException : Xeption
    {
        public FailedReminderStorageException(Exception innerException)
            : base(message: "Failed reminder storage error occurred.", innerException)
        { }
    }
}
