//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class FailedWordPackageServiceException : Xeption
    {
        public FailedWordPackageServiceException(Exception innerException)
            : base(message: "Failed word package service error occurred.", innerException)
        { }
    }
}
