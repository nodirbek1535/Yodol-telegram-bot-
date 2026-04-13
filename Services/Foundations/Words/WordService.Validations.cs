//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Words;
using Yodol_telegram_bot_.Models.Words.Exceptions;

namespace Yodol_telegram_bot_.Services.Foundations.Words
{
    public partial class WordService
    {
        private static void ValidateWord(Word word)
        {
            ValidateWordIsNotNull(word);

            Validate(
                (Rule: IsInvalid(word.Original),
                    Parameter: nameof(Word.Original)),

                (Rule: IsInvalid(word.Translation),
                    Parameter: nameof(Word.Translation)),

                (Rule: IsInvalid(word.UserTelegramId),
                    Parameter: nameof(Word.UserTelegramId)),

                (Rule: IsInvalid(word.PackageId),
                    Parameter: nameof(Word.PackageId)));
        }

        private static void ValidateWordIsNotNull(Word word)
        {
            if (word is null)
            {
                throw new NullWordException();
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

        private static dynamic IsInvalid(Guid value) => new
        {
            Condition = value == Guid.Empty,
            Message = "Id is required"
        };

        private static void Validate(
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidWordException = new InvalidWordException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidWordException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidWordException.ThrowIfContainsErrors();
        }
    }
}
