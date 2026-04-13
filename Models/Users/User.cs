//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Models.Users
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long TelegramId { get; set; }
        public string FirstName { get; set; } = default!;
        public UserState State { get; set; } = UserState.None;
        public int PendingWordCount { get; set; }
        public Guid PendingPackageId { get; set; }
        public TimeSpan PendingInterval { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
