//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class WordValidationException : Xeption
    {
        public WordValidationException(Xeption innerException)
            : base(message: "Word validation error occurred.", innerException)
        { }
    }
}
