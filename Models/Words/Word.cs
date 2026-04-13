//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Models.Words
{
    public class Word
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long UserTelegramId { get; set; }
        public Guid PackageId { get; set; }
        public string Original { get; set; } = default!;
        public string Translation { get; set; } = default!;
        public bool IsLearned { get; set; }
        public int RepeatCount { get; set; }
        public DateTime? LastAskedTime { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
