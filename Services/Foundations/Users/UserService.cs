//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Brokers.DateTimes;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Storages;
using Yodol_telegram_bot_.Models.Users;

namespace Yodol_telegram_bot_.Services.Foundations.Users
{
    public partial class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public UserService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<User> AddUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUser(user);

            this.loggingBroker.LogInformation(
                $"Adding user with TelegramId: {user.TelegramId}...");

            User addedUser = await this.storageBroker.InsertUserAsync(user);

            this.loggingBroker.LogInformation(
                $"User added successfully. TelegramId: {addedUser.TelegramId}.");

            return addedUser;
        });

        public ValueTask<User> RetrieveOrCreateUserAsync(
            long telegramId, string firstName) =>
        TryCatch(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving or creating user. TelegramId: {telegramId}...");

            User? existingUser =
                await this.storageBroker.SelectUserByTelegramIdAsync(telegramId);

            if (existingUser is not null)
            {
                this.loggingBroker.LogInformation(
                    $"Existing user found. TelegramId: {telegramId}, " +
                    $"State: {existingUser.State}.");

                return existingUser;
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                TelegramId = telegramId,
                FirstName = firstName,
                State = UserState.None,
                CreatedDate = this.dateTimeBroker.GetCurrentDateTime()
            };

            User createdUser = await this.storageBroker.InsertUserAsync(newUser);

            this.loggingBroker.LogInformation(
                $"New user created. TelegramId: {telegramId}, " +
                $"FirstName: {firstName}.");

            return createdUser;
        });

        public ValueTask<User> ModifyUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUser(user);

            this.loggingBroker.LogInformation(
                $"Modifying user. TelegramId: {user.TelegramId}, " +
                $"State: {user.State}...");

            User modifiedUser = await this.storageBroker.UpdateUserAsync(user);

            this.loggingBroker.LogInformation(
                $"User modified successfully. TelegramId: {modifiedUser.TelegramId}, " +
                $"State: {modifiedUser.State}.");

            return modifiedUser;
        });

        public ValueTask<User> ResetUserStateAsync(User user) =>
        TryCatch(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Resetting user state. TelegramId: {user.TelegramId}, " +
                $"OldState: {user.State}...");

            user.State = UserState.None;
            user.PendingWordCount = 0;
            user.PendingPackageId = Guid.Empty;
            user.PendingInterval = TimeSpan.Zero;

            User resetUser = await this.storageBroker.UpdateUserAsync(user);

            this.loggingBroker.LogInformation(
                $"User state reset to None. TelegramId: {resetUser.TelegramId}.");

            return resetUser;
        });
    }
}
