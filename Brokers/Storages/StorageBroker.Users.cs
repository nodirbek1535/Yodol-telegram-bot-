//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Users;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial class StorageBroker
    {
        private const string UsersFile = "users.json";

        public async ValueTask<User> InsertUserAsync(User user)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var users = await ReadAllAsync<User>(UsersFile);
                users.Add(user);
                await WriteAllAsync(UsersFile, users);

                return user;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<List<User>> SelectAllUsersAsync() =>
            await ReadAllAsync<User>(UsersFile);

        public async ValueTask<User?> SelectUserByTelegramIdAsync(long telegramId)
        {
            var users = await ReadAllAsync<User>(UsersFile);

            return users.FirstOrDefault(user =>
                user.TelegramId == telegramId);
        }

        public async ValueTask<User> UpdateUserAsync(User user)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var users = await ReadAllAsync<User>(UsersFile);

                int index = users.FindIndex(storedUser =>
                    storedUser.TelegramId == user.TelegramId);

                if (index >= 0)
                {
                    users[index] = user;
                }

                await WriteAllAsync(UsersFile, users);

                return user;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<User> DeleteUserAsync(User user)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var users = await ReadAllAsync<User>(UsersFile);

                users.RemoveAll(storedUser =>
                    storedUser.TelegramId == user.TelegramId);

                await WriteAllAsync(UsersFile, users);

                return user;
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
