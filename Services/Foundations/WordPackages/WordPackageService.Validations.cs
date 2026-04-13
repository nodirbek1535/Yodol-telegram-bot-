//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.WordPackages;
using Yodol_telegram_bot_.Models.WordPackages.Exceptions;

namespace Yodol_telegram_bot_.Services.Foundations.WordPackages
{
    public partial class WordPackageService
    {
        private static void ValidateWordPackage(WordPackage wordPackage)
        {
            ValidateWordPackageIsNotNull(wordPackage);

            Validate(
                (Rule: IsInvalid(wordPackage.Name),
                    Parameter: nameof(WordPackage.Name)),

                (Rule: IsInvalid(wordPackage.UserTelegramId),
                    Parameter: nameof(WordPackage.UserTelegramId)),

                (Rule: IsInvalid(wordPackage.WordCount),
                    Parameter: nameof(WordPackage.WordCount)));
        }

        private static void ValidateWordPackageIsNotNull(WordPackage wordPackage)
        {
            if (wordPackage is null)
            {
                throw new NullWordPackageException();
            }
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(long value) => new
        {
            Condition = value == default,
            Message = "Value is required"
        };

        private static dynamic IsInvalid(int value) => new
        {
            Condition = value <= 0,
            Message = "Value must be greater than zero"
        };

        private static void Validate(
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidException = new InvalidWordPackageException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidException.ThrowIfContainsErrors();
        }
    }
}
