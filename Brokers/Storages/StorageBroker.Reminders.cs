//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Reminders;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial class StorageBroker
    {
        private const string RemindersFile = "reminders.json";

        public async ValueTask<Reminder> InsertReminderAsync(Reminder reminder)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var reminders = await ReadAllAsync<Reminder>(RemindersFile);
                reminders.Add(reminder);
                await WriteAllAsync(RemindersFile, reminders);

                return reminder;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<List<Reminder>> SelectAllRemindersAsync() =>
            await ReadAllAsync<Reminder>(RemindersFile);

        public async ValueTask<Reminder?> SelectReminderByIdAsync(Guid reminderId)
        {
            var reminders = await ReadAllAsync<Reminder>(RemindersFile);

            return reminders.FirstOrDefault(reminder =>
                reminder.Id == reminderId);
        }

        public async ValueTask<List<Reminder>> SelectActiveRemindersAsync()
        {
            var reminders = await ReadAllAsync<Reminder>(RemindersFile);

            return reminders.Where(reminder => reminder.IsActive).ToList();
        }

        public async ValueTask<List<Reminder>> SelectRemindersByUserTelegramIdAsync(
            long userTelegramId)
        {
            var reminders = await ReadAllAsync<Reminder>(RemindersFile);

            return reminders.Where(reminder =>
                reminder.UserTelegramId == userTelegramId).ToList();
        }

        public async ValueTask<Reminder> UpdateReminderAsync(Reminder reminder)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var reminders = await ReadAllAsync<Reminder>(RemindersFile);

                int index = reminders.FindIndex(storedReminder =>
                    storedReminder.Id == reminder.Id);

                if (index >= 0)
                {
                    reminders[index] = reminder;
                }

                await WriteAllAsync(RemindersFile, reminders);

                return reminder;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<Reminder> DeleteReminderAsync(Reminder reminder)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var reminders = await ReadAllAsync<Reminder>(RemindersFile);

                reminders.RemoveAll(storedReminder =>
                    storedReminder.Id == reminder.Id);

                await WriteAllAsync(RemindersFile, reminders);

                return reminder;
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
