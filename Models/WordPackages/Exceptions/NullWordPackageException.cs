//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class NullWordPackageException : Xeption
    {
        public NullWordPackageException()
            : base(message: "WordPackage is null.")
        { }
    }
}
