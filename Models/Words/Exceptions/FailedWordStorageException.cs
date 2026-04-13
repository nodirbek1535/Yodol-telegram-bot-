//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class FailedWordStorageException : Xeption
    {
        public FailedWordStorageException(Exception innerException)
            : base(message: "Failed word storage error occurred.", innerException)
        { }
    }
}
