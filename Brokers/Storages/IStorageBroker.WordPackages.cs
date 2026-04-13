//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.WordPackages;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<WordPackage> InsertWordPackageAsync(WordPackage wordPackage);
        ValueTask<List<WordPackage>> SelectAllWordPackagesAsync();
        ValueTask<WordPackage?> SelectWordPackageByIdAsync(Guid packageId);
        ValueTask<List<WordPackage>> SelectWordPackagesByUserTelegramIdAsync(long userTelegramId);
        ValueTask<WordPackage> UpdateWordPackageAsync(WordPackage wordPackage);
        ValueTask<WordPackage> DeleteWordPackageAsync(WordPackage wordPackage);
    }
}
