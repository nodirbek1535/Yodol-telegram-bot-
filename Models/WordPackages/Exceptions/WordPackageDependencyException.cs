//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class WordPackageDependencyException : Xeption
    {
        public WordPackageDependencyException(Xeption innerException)
            : base(message: "WordPackage dependency error occurred.", innerException)
        { }
    }
}
