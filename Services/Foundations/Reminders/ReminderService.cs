//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Brokers.DateTimes;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Storages;
using Yodol_telegram_bot_.Models.Reminders;

namespace Yodol_telegram_bot_.Services.Foundations.Reminders
{
    public partial class ReminderService : IReminderService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public ReminderService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Reminder> AddReminderAsync(Reminder reminder) =>
        TryCatch(async () =>
        {
            ValidateReminderOnAdd(reminder);

            this.loggingBroker.LogInformation(
                $"Adding reminder. UserTelegramId: {reminder.UserTelegramId}, " +
                $"PackageId: {reminder.PackageId}, " +
                $"Interval: {reminder.Interval}, " +
                $"EndDateTime: {reminder.EndDateTime:dd.MM.yyyy HH:mm}...");

            Reminder addedReminder =
                await this.storageBroker.InsertReminderAsync(reminder);

            this.loggingBroker.LogInformation(
                $"Reminder added successfully. Id: {addedReminder.Id}, " +
                $"UserTelegramId: {addedReminder.UserTelegramId}.");

            return addedReminder;
        });

        public ValueTask<List<Reminder>> RetrieveActiveRemindersAsync() =>
        TryCatchList(async () =>
        {
            this.loggingBroker.LogInformation(
                "Retrieving all active reminders...");

            List<Reminder> reminders =
                await this.storageBroker.SelectActiveRemindersAsync();

            this.loggingBroker.LogInformation(
                $"Retrieved {reminders.Count} active reminders.");

            return reminders;
        });

        public ValueTask<List<Reminder>> RetrieveRemindersByUserTelegramIdAsync(
            long userTelegramId) =>
        TryCatchList(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving reminders by " +
                $"UserTelegramId: {userTelegramId}...");

            List<Reminder> reminders =
                await this.storageBroker
                    .SelectRemindersByUserTelegramIdAsync(userTelegramId);

            this.loggingBroker.LogInformation(
                $"Retrieved {reminders.Count} reminders for " +
                $"UserTelegramId: {userTelegramId}.");

            return reminders;
        });

        public ValueTask<Reminder?> RetrieveReminderByIdAsync(
            Guid reminderId) =>
        TryCatchNullable(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving reminder by Id: {reminderId}...");

            Reminder? reminder =
                await this.storageBroker.SelectReminderByIdAsync(reminderId);

            this.loggingBroker.LogInformation(
                reminder is not null
                    ? $"Reminder found. Id: {reminderId}."
                    : $"Reminder not found. Id: {reminderId}.");

            return reminder;
        });

        public ValueTask<Reminder> ModifyReminderAsync(Reminder reminder) =>
        TryCatch(async () =>
        {
            ValidateReminderOnModify(reminder);

            this.loggingBroker.LogInformation(
                $"Modifying reminder. Id: {reminder.Id}, " +
                $"IsActive: {reminder.IsActive}, " +
                $"LastSentAt: {reminder.LastSentAt:dd.MM.yyyy HH:mm}...");

            Reminder modifiedReminder =
                await this.storageBroker.UpdateReminderAsync(reminder);

            this.loggingBroker.LogInformation(
                $"Reminder modified successfully. Id: {modifiedReminder.Id}.");

            return modifiedReminder;
        });

        public ValueTask<Reminder> RemoveReminderAsync(Reminder reminder) =>
        TryCatch(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Removing reminder. Id: {reminder.Id}...");

            Reminder removedReminder =
                await this.storageBroker.DeleteReminderAsync(reminder);

            this.loggingBroker.LogInformation(
                $"Reminder removed successfully. Id: {removedReminder.Id}.");

            return removedReminder;
        });
    }
}
