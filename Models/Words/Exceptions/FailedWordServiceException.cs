//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class FailedWordServiceException : Xeption
    {
        public FailedWordServiceException(Exception innerException)
            : base(message: "Failed word service error occurred.", innerException)
        { }
    }
}
