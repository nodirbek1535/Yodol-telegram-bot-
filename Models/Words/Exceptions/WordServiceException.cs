//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class WordServiceException : Xeption
    {
        public WordServiceException(Xeption innerException)
            : base(message: "Word service error occurred.", innerException)
        { }
    }
}
