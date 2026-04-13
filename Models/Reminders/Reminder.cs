//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Models.Reminders
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long UserTelegramId { get; set; }
        public Guid PackageId { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime LastSentAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
