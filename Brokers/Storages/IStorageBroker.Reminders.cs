//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Reminders;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Reminder> InsertReminderAsync(Reminder reminder);
        ValueTask<List<Reminder>> SelectAllRemindersAsync();
        ValueTask<Reminder?> SelectReminderByIdAsync(Guid reminderId);
        ValueTask<List<Reminder>> SelectActiveRemindersAsync();
        ValueTask<List<Reminder>> SelectRemindersByUserTelegramIdAsync(long userTelegramId);
        ValueTask<Reminder> UpdateReminderAsync(Reminder reminder);
        ValueTask<Reminder> DeleteReminderAsync(Reminder reminder);
    }
}
