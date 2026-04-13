//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Xeptions;

namespace Yodol_telegram_bot_.Models.Words.Exceptions
{
    public class NotFoundWordException : Xeption
    {
        public NotFoundWordException(Guid wordId)
            : base(message: $"Word not found with id: {wordId}.")
        { }
    }
}
