//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class NullWordException : Xeption
    {
        public NullWordException()
            : base(message: "Word is null.")
        { }
    }
}
