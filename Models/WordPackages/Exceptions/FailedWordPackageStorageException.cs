//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class FailedWordPackageStorageException : Xeption
    {
        public FailedWordPackageStorageException(Exception innerException)
            : base(message: "Failed word package storage error occurred.", innerException)
        { }
    }
}
