//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class InvalidWordPackageException : Xeption
    {
        public InvalidWordPackageException()
            : base(message: "WordPackage is invalid.")
        { }
    }
}
