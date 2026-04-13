//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Reminders.Exceptions
{
    public class FailedReminderServiceException : Xeption
    {
        public FailedReminderServiceException(Exception innerException)
            : base(message: "Failed reminder service error occurred.", innerException)
        { }
    }
}
