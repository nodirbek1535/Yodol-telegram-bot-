//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(long telegramId)
            : base(message: $"User not found with telegram id: {telegramId}.")
        { }
    }
}
