//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Brokers.DateTimes;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Storages;
using Yodol_telegram_bot_.Models.Words;

namespace Yodol_telegram_bot_.Services.Foundations.Words
{
    public partial class WordService : IWordService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public WordService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Word> AddWordAsync(Word word) =>
        TryCatch(async () =>
        {
            ValidateWord(word);

            this.loggingBroker.LogInformation(
                $"Adding word. Original: {word.Original}, " +
                $"Translation: {word.Translation}, " +
                $"PackageId: {word.PackageId}, " +
                $"UserTelegramId: {word.UserTelegramId}...");

            Word addedWord = await this.storageBroker.InsertWordAsync(word);

            this.loggingBroker.LogInformation(
                $"Word added successfully. Id: {addedWord.Id}, " +
                $"Original: {addedWord.Original}.");

            return addedWord;
        });

        public ValueTask<List<Word>> RetrieveWordsByPackageIdAsync(
            Guid packageId) =>
        TryCatchList(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving words by PackageId: {packageId}...");

            List<Word> words =
                await this.storageBroker.SelectWordsByPackageIdAsync(packageId);

            this.loggingBroker.LogInformation(
                $"Retrieved {words.Count} words for PackageId: {packageId}.");

            return words;
        });

        public ValueTask<List<Word>> RetrieveWordsByUserTelegramIdAsync(
            long userTelegramId) =>
        TryCatchList(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving words by UserTelegramId: {userTelegramId}...");

            List<Word> words =
                await this.storageBroker.SelectWordsByUserTelegramIdAsync(
                    userTelegramId);

            this.loggingBroker.LogInformation(
                $"Retrieved {words.Count} words for " +
                $"UserTelegramId: {userTelegramId}.");

            return words;
        });

        public ValueTask<Word?> RetrieveWordByIdAsync(Guid wordId) =>
        TryCatchNullable(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving word by Id: {wordId}...");

            Word? word = await this.storageBroker.SelectWordByIdAsync(wordId);

            this.loggingBroker.LogInformation(
                word is not null
                    ? $"Word found. Id: {wordId}, Original: {word.Original}."
                    : $"Word not found. Id: {wordId}.");

            return word;
        });

        public ValueTask<Word> ModifyWordAsync(Word word) =>
        TryCatch(async () =>
        {
            ValidateWord(word);

            this.loggingBroker.LogInformation(
                $"Modifying word. Id: {word.Id}, " +
                $"Original: {word.Original}...");

            Word modifiedWord = await this.storageBroker.UpdateWordAsync(word);

            this.loggingBroker.LogInformation(
                $"Word modified successfully. Id: {modifiedWord.Id}.");

            return modifiedWord;
        });

        public ValueTask<Word> RemoveWordAsync(Word word) =>
        TryCatch(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Removing word. Id: {word.Id}, " +
                $"Original: {word.Original}...");

            Word removedWord = await this.storageBroker.DeleteWordAsync(word);

            this.loggingBroker.LogInformation(
                $"Word removed successfully. Id: {removedWord.Id}.");

            return removedWord;
        });
    }
}
