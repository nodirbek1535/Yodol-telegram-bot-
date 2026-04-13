//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.WordPackages.Exceptions
{
    public class NotFoundWordPackageException : Xeption
    {
        public NotFoundWordPackageException(Guid packageId)
            : base(message: $"WordPackage not found with id: {packageId}.")
        { }
    }
}
