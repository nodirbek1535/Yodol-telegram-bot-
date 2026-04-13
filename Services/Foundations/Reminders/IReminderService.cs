//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Reminders;

namespace Yodol_telegram_bot_.Services.Foundations.Reminders
{
    public interface IReminderService
    {
        ValueTask<Reminder> AddReminderAsync(Reminder reminder);
        ValueTask<List<Reminder>> RetrieveActiveRemindersAsync();
        ValueTask<List<Reminder>> RetrieveRemindersByUserTelegramIdAsync(long userTelegramId);
        ValueTask<Reminder?> RetrieveReminderByIdAsync(Guid reminderId);
        ValueTask<Reminder> ModifyReminderAsync(Reminder reminder);
        ValueTask<Reminder> RemoveReminderAsync(Reminder reminder);
    }
}
