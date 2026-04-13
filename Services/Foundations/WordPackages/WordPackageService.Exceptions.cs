//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.WordPackages;
using Yodol_telegram_bot_.Models.WordPackages.Exceptions;
using Xeptions;

namespace Yodol_telegram_bot_.Services.Foundations.WordPackages
{
    public partial class WordPackageService
    {
        private delegate ValueTask<WordPackage> ReturningWordPackageFunction();
        private delegate ValueTask<WordPackage?> ReturningNullableWordPackageFunction();
        private delegate ValueTask<List<WordPackage>> ReturningWordPackageListFunction();

        private async ValueTask<WordPackage> TryCatch(
            ReturningWordPackageFunction returningWordPackageFunction)
        {
            try
            {
                return await returningWordPackageFunction();
            }
            catch (NullWordPackageException nullWordPackageException)
            {
                throw CreateAndLogValidationException(nullWordPackageException);
            }
            catch (InvalidWordPackageException invalidWordPackageException)
            {
                throw CreateAndLogValidationException(invalidWordPackageException);
            }
            catch (IOException ioException)
            {
                var failedStorageException =
                    new FailedWordPackageStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedWordPackageServiceException(exception);

                throw CreateAndLogServiceException(failedServiceException);
            }
        }

        private async ValueTask<WordPackage?> TryCatchNullable(
            ReturningNullableWordPackageFunction returningFunction)
        {
            try
            {
                return await returningFunction();
            }
            catch (IOException ioException)
            {
                var failedStorageException =
                    new FailedWordPackageStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedWordPackageServiceException(exception);

                throw CreateAndLogServiceException(failedServiceException);
            }
        }

        private async ValueTask<List<WordPackage>> TryCatchList(
            ReturningWordPackageListFunction returningFunction)
        {
            try
            {
                return await returningFunction();
            }
            catch (IOException ioException)
            {
                var failedStorageException =
                    new FailedWordPackageStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceException =
                    new FailedWordPackageServiceException(exception);

                throw CreateAndLogServiceException(failedServiceException);
            }
        }

        private WordPackageValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var validationException =
                new WordPackageValidationException(exception);

            this.loggingBroker.LogError(validationException);

            return validationException;
        }

        private WordPackageDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var dependencyException =
                new WordPackageDependencyException(exception);

            this.loggingBroker.LogCritical(dependencyException);

            return dependencyException;
        }

        private WordPackageServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var serviceException =
                new WordPackageServiceException(exception);

            this.loggingBroker.LogError(serviceException);

            return serviceException;
        }
    }
}
