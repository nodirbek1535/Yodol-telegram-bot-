//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Reminders;
using Yodol_telegram_bot_.Models.Reminders.Exceptions;

namespace Yodol_telegram_bot_.Services.Foundations.Reminders
{
    public partial class ReminderService
    {
        private void ValidateReminderOnAdd(Reminder reminder)
        {
            ValidateReminderIsNotNull(reminder);

            Validate(
                (Rule: IsInvalid(reminder.UserTelegramId),
                    Parameter: nameof(Reminder.UserTelegramId)),

                (Rule: IsInvalid(reminder.PackageId),
                    Parameter: nameof(Reminder.PackageId)),

                (Rule: IsInvalid(reminder.Interval),
                    Parameter: nameof(Reminder.Interval)),

                (Rule: IsInvalid(reminder.EndDateTime),
                    Parameter: nameof(Reminder.EndDateTime)));
        }

        private void ValidateReminderOnModify(Reminder reminder)
        {
            ValidateReminderIsNotNull(reminder);

            Validate(
                (Rule: IsInvalid(reminder.Id),
                    Parameter: nameof(Reminder.Id)),

                (Rule: IsInvalid(reminder.UserTelegramId),
                    Parameter: nameof(Reminder.UserTelegramId)),

                (Rule: IsInvalid(reminder.PackageId),
                    Parameter: nameof(Reminder.PackageId)),

                (Rule: IsInvalid(reminder.Interval),
                    Parameter: nameof(Reminder.Interval)),

                (Rule: IsInvalidDefault(reminder.EndDateTime),
                    Parameter: nameof(Reminder.EndDateTime)));
        }

        private static void ValidateReminderIsNotNull(Reminder reminder)
        {
            if (reminder is null)
            {
                throw new NullReminderException();
            }
        }

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

        private dynamic IsInvalid(TimeSpan value) => new
        {
            Condition = value <= TimeSpan.Zero,
            Message = "Interval must be greater than zero"
        };

        private dynamic IsInvalid(DateTime value) => new
        {
            Condition = value <= this.dateTimeBroker.GetCurrentDateTime(),
            Message = "Date must be in the future"
        };

        private static dynamic IsInvalidDefault(DateTime value) => new
        {
            Condition = value == default,
            Message = "Date is required"
        };

        private static void Validate(
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidException = new InvalidReminderException();

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
