//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class WordPackageValidationException : Xeption
    {
        public WordPackageValidationException(Xeption innerException)
            : base(message: "WordPackage validation error occurred.", innerException)
        { }
    }
}
