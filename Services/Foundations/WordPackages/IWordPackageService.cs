//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.WordPackages;

namespace Yodol_telegram_bot_.Services.Foundations.WordPackages
{
    public interface IWordPackageService
    {
        ValueTask<WordPackage> AddWordPackageAsync(WordPackage wordPackage);
        ValueTask<List<WordPackage>> RetrieveWordPackagesByUserTelegramIdAsync(long userTelegramId);
        ValueTask<WordPackage?> RetrieveWordPackageByIdAsync(Guid packageId);
        ValueTask<WordPackage> ModifyWordPackageAsync(WordPackage wordPackage);
        ValueTask<WordPackage> RemoveWordPackageAsync(WordPackage wordPackage);
    }
}
