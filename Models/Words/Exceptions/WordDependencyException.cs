//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class WordDependencyException : Xeption
    {
        public WordDependencyException(Xeption innerException)
            : base(message: "Word dependency error occurred.", innerException)
        { }
    }
}
