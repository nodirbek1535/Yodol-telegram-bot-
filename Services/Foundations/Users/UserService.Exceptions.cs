//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Users;
using Yodol_telegram_bot_.Models.Users.Exceptions;
using Xeptions;

namespace Yodol_telegram_bot_.Services.Foundations.Users
{
    public partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();
        private async ValueTask<User> TryCatch(ReturningUserFunction returningUserFunction)
        {
            try
            {
                return await returningUserFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw CreateAndLogValidationException(nullUserException);
            }
            catch (InvalidUserException invalidUserException)
            {
                throw CreateAndLogValidationException(invalidUserException);
            }
            catch (NotFoundUserException notFoundUserException)
            {
                throw CreateAndLogValidationException(notFoundUserException);
            }
            catch (IOException ioException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedUserStorageException);
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(exception);

                throw CreateAndLogServiceException(failedUserServiceException);
            }
        }

        private UserValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var userValidationException =
                new UserValidationException(exception);

            this.loggingBroker.LogError(userValidationException);

            return userValidationException;
        }

        private UserDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var userDependencyException =
                new UserDependencyException(exception);

            this.loggingBroker.LogCritical(userDependencyException);

            return userDependencyException;
        }

        private UserServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var userServiceException =
                new UserServiceException(exception);

            this.loggingBroker.LogError(userServiceException);

            return userServiceException;
        }
    }
}
