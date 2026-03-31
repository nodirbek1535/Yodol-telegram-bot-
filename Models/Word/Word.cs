//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Models.Word
{
    public class Word
    {
        public Guid WordId { get; set; } = Guid.NewGuid();
        public long ChatId { get; set; }    
        public string English { get; set; } = string.Empty;
        public string Uzbek { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? Deadline { get; set; }
        public bool IsLearned { get; set; } = false;
        public int RepeatCount { get; set; }
        public DateTime? LastAskedTime { get; set; }
    }
}