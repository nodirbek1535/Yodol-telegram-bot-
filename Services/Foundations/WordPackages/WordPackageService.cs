//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Brokers.DateTimes;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Storages;
using Yodol_telegram_bot_.Models.WordPackages;

namespace Yodol_telegram_bot_.Services.Foundations.WordPackages
{
    public partial class WordPackageService : IWordPackageService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public WordPackageService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<WordPackage> AddWordPackageAsync(
            WordPackage wordPackage) =>
        TryCatch(async () =>
        {
            ValidateWordPackage(wordPackage);

            this.loggingBroker.LogInformation(
                $"Adding word package. Name: {wordPackage.Name}, " +
                $"WordCount: {wordPackage.WordCount}, " +
                $"UserTelegramId: {wordPackage.UserTelegramId}...");

            WordPackage addedPackage =
                await this.storageBroker.InsertWordPackageAsync(wordPackage);

            this.loggingBroker.LogInformation(
                $"Word package added successfully. Id: {addedPackage.Id}, " +
                $"Name: {addedPackage.Name}.");

            return addedPackage;
        });

        public ValueTask<List<WordPackage>> RetrieveWordPackagesByUserTelegramIdAsync(
            long userTelegramId) =>
        TryCatchList(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving word packages by " +
                $"UserTelegramId: {userTelegramId}...");

            List<WordPackage> packages =
                await this.storageBroker
                    .SelectWordPackagesByUserTelegramIdAsync(userTelegramId);

            this.loggingBroker.LogInformation(
                $"Retrieved {packages.Count} packages for " +
                $"UserTelegramId: {userTelegramId}.");

            return packages;
        });

        public ValueTask<WordPackage?> RetrieveWordPackageByIdAsync(
            Guid packageId) =>
        TryCatchNullable(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Retrieving word package by Id: {packageId}...");

            WordPackage? package =
                await this.storageBroker.SelectWordPackageByIdAsync(packageId);

            this.loggingBroker.LogInformation(
                package is not null
                    ? $"Word package found. Id: {packageId}, Name: {package.Name}."
                    : $"Word package not found. Id: {packageId}.");

            return package;
        });

        public ValueTask<WordPackage> ModifyWordPackageAsync(
            WordPackage wordPackage) =>
        TryCatch(async () =>
        {
            ValidateWordPackage(wordPackage);

            this.loggingBroker.LogInformation(
                $"Modifying word package. Id: {wordPackage.Id}, " +
                $"Name: {wordPackage.Name}...");

            WordPackage modifiedPackage =
                await this.storageBroker.UpdateWordPackageAsync(wordPackage);

            this.loggingBroker.LogInformation(
                $"Word package modified successfully. " +
                $"Id: {modifiedPackage.Id}.");

            return modifiedPackage;
        });

        public ValueTask<WordPackage> RemoveWordPackageAsync(
            WordPackage wordPackage) =>
        TryCatch(async () =>
        {
            this.loggingBroker.LogInformation(
                $"Removing word package. Id: {wordPackage.Id}, " +
                $"Name: {wordPackage.Name}...");

            WordPackage removedPackage =
                await this.storageBroker.DeleteWordPackageAsync(wordPackage);

            this.loggingBroker.LogInformation(
                $"Word package removed successfully. " +
                $"Id: {removedPackage.Id}.");

            return removedPackage;
        });
    }
}
