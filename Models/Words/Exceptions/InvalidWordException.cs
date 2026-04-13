//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class InvalidWordException : Xeption
    {
        public InvalidWordException()
            : base(message: "Word is invalid.")
        { }
    }
}
