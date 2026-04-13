//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.WordPackages;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial class StorageBroker
    {
        private const string WordPackagesFile = "packages.json";

        public async ValueTask<WordPackage> InsertWordPackageAsync(
            WordPackage wordPackage)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var packages = await ReadAllAsync<WordPackage>(WordPackagesFile);
                packages.Add(wordPackage);
                await WriteAllAsync(WordPackagesFile, packages);

                return wordPackage;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<List<WordPackage>> SelectAllWordPackagesAsync() =>
            await ReadAllAsync<WordPackage>(WordPackagesFile);

        public async ValueTask<WordPackage?> SelectWordPackageByIdAsync(Guid packageId)
        {
            var packages = await ReadAllAsync<WordPackage>(WordPackagesFile);

            return packages.FirstOrDefault(package => package.Id == packageId);
        }

        public async ValueTask<List<WordPackage>> SelectWordPackagesByUserTelegramIdAsync(
            long userTelegramId)
        {
            var packages = await ReadAllAsync<WordPackage>(WordPackagesFile);

            return packages.Where(package =>
                package.UserTelegramId == userTelegramId).ToList();
        }

        public async ValueTask<WordPackage> UpdateWordPackageAsync(
            WordPackage wordPackage)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var packages = await ReadAllAsync<WordPackage>(WordPackagesFile);

                int index = packages.FindIndex(storedPackage =>
                    storedPackage.Id == wordPackage.Id);

                if (index >= 0)
                {
                    packages[index] = wordPackage;
                }

                await WriteAllAsync(WordPackagesFile, packages);

                return wordPackage;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<WordPackage> DeleteWordPackageAsync(
            WordPackage wordPackage)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var packages = await ReadAllAsync<WordPackage>(WordPackagesFile);

                packages.RemoveAll(storedPackage =>
                    storedPackage.Id == wordPackage.Id);

                await WriteAllAsync(WordPackagesFile, packages);

                return wordPackage;
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
