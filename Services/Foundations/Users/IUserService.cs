//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Users;

namespace Yodol_telegram_bot_.Services.Foundations.Users
{
    public interface IUserService
    {
        ValueTask<User> AddUserAsync(User user);
        ValueTask<User> RetrieveOrCreateUserAsync(long telegramId, string firstName);
        ValueTask<User> ModifyUserAsync(User user);
        ValueTask<User> ResetUserStateAsync(User user);
    }
}
