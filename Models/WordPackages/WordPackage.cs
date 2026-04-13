//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Models.WordPackages
{
    public class WordPackage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long UserTelegramId { get; set; }
        public string Name { get; set; } = default!;
        public int WordCount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
