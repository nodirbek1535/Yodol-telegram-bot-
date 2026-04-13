//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class WordPackageServiceException : Xeption
    {
        public WordPackageServiceException(Xeption innerException)
            : base(message: "WordPackage service error occurred.", innerException)
        { }
    }
}
