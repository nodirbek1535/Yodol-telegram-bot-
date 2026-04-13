//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Users;
using Yodol_telegram_bot_.Models.Users.Exceptions;

namespace Yodol_telegram_bot_.Services.Foundations.Users
{
    public partial class UserService
    {
        private static void ValidateUser(User user)
        {
            ValidateUserIsNotNull(user);

            Validate(
                (Rule: IsInvalid(user.TelegramId),
                    Parameter: nameof(User.TelegramId)));
        }

        private static void ValidateUserIsNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }

        private static dynamic IsInvalid(long value) => new
        {
            Condition = value == default,
            Message = "Value is required"
        };

        private static void Validate(
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
