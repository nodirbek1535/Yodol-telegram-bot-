//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Reminders;
using Yodol_telegram_bot_.Models.Reminders.Exceptions;
using Xeptions;

namespace Yodol_telegram_bot_.Services.Foundations.Reminders
{
    public partial class ReminderService
    {
        private delegate ValueTask<Reminder> ReturningReminderFunction();
        private delegate ValueTask<Reminder?> ReturningNullableReminderFunction();
        private delegate ValueTask<List<Reminder>> ReturningReminderListFunction();

        private async ValueTask<Reminder> TryCatch(
            ReturningReminderFunction returningReminderFunction)
        {
            try
            {
                return await returningReminderFunction();
            }
            catch (NullReminderException nullReminderException)
            {
                throw CreateAndLogValidationException(nullReminderException);
            }
            catch (InvalidReminderException invalidReminderException)
            {
                throw CreateAndLogValidationException(invalidReminderException);
            }
            catch (IOException ioException)
            {
                var failedStorageException =
                    new FailedReminderStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedReminderServiceException(exception);

                throw CreateAndLogServiceException(failedServiceException);
            }
        }

        private async ValueTask<Reminder?> TryCatchNullable(
            ReturningNullableReminderFunction returningFunction)
        {
            try
            {
                return await returningFunction();
            }
            catch (IOException ioException)
            {
                var failedStorageException =
                    new FailedReminderStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedReminderServiceException(exception);

                throw CreateAndLogServiceException(failedServiceException);
            }
        }

        private async ValueTask<List<Reminder>> TryCatchList(
            ReturningReminderListFunction returningFunction)
        {
            try
            {
                return await returningFunction();
            }
            catch (IOException ioException)
            {
                var failedStorageException =
                    new FailedReminderStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedReminderServiceException(exception);

                throw CreateAndLogServiceException(failedServiceException);
            }
        }

        private ReminderValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var validationException =
                new ReminderValidationException(exception);

            this.loggingBroker.LogError(validationException);

            return validationException;
        }

        private ReminderDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var dependencyException =
                new ReminderDependencyException(exception);

            this.loggingBroker.LogCritical(dependencyException);

            return dependencyException;
        }

        private ReminderServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var serviceException =
                new ReminderServiceException(exception);

            this.loggingBroker.LogError(serviceException);

            return serviceException;
        }
    }
}
