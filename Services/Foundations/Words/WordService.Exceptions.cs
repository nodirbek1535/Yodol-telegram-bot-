//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Words;
using Yodol_telegram_bot_.Models.Words.Exceptions;
using Xeptions;

namespace Yodol_telegram_bot_.Services.Foundations.Words
{
    public partial class WordService
    {
        private delegate ValueTask<Word> ReturningWordFunction();
        private delegate ValueTask<Word?> ReturningNullableWordFunction();
        private delegate ValueTask<List<Word>> ReturningWordListFunction();

        private async ValueTask<Word> TryCatch(
            ReturningWordFunction returningWordFunction)
        {
            try
            {
                return await returningWordFunction();
            }
            catch (NullWordException nullWordException)
            {
                throw CreateAndLogValidationException(nullWordException);
            }
            catch (InvalidWordException invalidWordException)
            {
                throw CreateAndLogValidationException(invalidWordException);
            }
            catch (IOException ioException)
            {
                var failedWordStorageException =
                    new FailedWordStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedWordStorageException);
            }
            catch (Exception exception)
            {
                var failedWordServiceException =
                    new FailedWordServiceException(exception);

                throw CreateAndLogServiceException(failedWordServiceException);
            }
        }

        private async ValueTask<Word?> TryCatchNullable(
            ReturningNullableWordFunction returningNullableWordFunction)
        {
            try
            {
                return await returningNullableWordFunction();
            }
            catch (IOException ioException)
            {
                var failedWordStorageException =
                    new FailedWordStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedWordStorageException);
            }
            catch (Exception exception)
            {
                var failedWordServiceException =
                    new FailedWordServiceException(exception);

                throw CreateAndLogServiceException(failedWordServiceException);
            }
        }

        private async ValueTask<List<Word>> TryCatchList(
            ReturningWordListFunction returningWordListFunction)
        {
            try
            {
                return await returningWordListFunction();
            }
            catch (IOException ioException)
            {
                var failedWordStorageException =
                    new FailedWordStorageException(ioException);

                throw CreateAndLogCriticalDependencyException(
                    failedWordStorageException);
            }
            catch (Exception exception)
            {
                var failedWordServiceException =
                    new FailedWordServiceException(exception);

                throw CreateAndLogServiceException(failedWordServiceException);
            }
        }

        private WordValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var wordValidationException =
                new WordValidationException(exception);

            this.loggingBroker.LogError(wordValidationException);

            return wordValidationException;
        }

        private WordDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var wordDependencyException =
                new WordDependencyException(exception);

            this.loggingBroker.LogCritical(wordDependencyException);

            return wordDependencyException;
        }

        private WordServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var wordServiceException =
                new WordServiceException(exception);

            this.loggingBroker.LogError(wordServiceException);

            return wordServiceException;
        }
    }
}
